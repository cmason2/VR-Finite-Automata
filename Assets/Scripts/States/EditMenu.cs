using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;
using DG.Tweening;

public class EditMenu : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] AudioClip validClip;
    [SerializeField] AudioClip invalidClip;

    [SerializeField] XRRayInteractor rayInteractor;
    private State state;
    private Bezier edge;
    public InputActionReference secondaryActionReference = null;
    private AutomataController automataController;
    [SerializeField] GameObject stateSelector;
    [SerializeField] GameObject highlightSprite;
    [SerializeField] GameObject edgeHighlightSprite;
    [SerializeField] GameObject edgeMenu;
    private SpriteRenderer highlightRenderer;
    private SpriteRenderer edgeHighlightRenderer;
    [SerializeField] Color normalHighlightColor;
    [SerializeField] Color deleteHighlightColor;
    [SerializeField] Color startHighlightColor;
    [SerializeField] Color finalHighlightColor;
    [SerializeField] Color edgeHighlightColor;
    private Camera mainCamera;
    private Collider[] statesInRange;
    private RaycastHit[] hitColliders;

    private IEnumerator coroutine;
    private char currentStateType = 'x';
    private string currentSelection = "";
    private string operation = "";

    private void Start()
    {
        highlightRenderer = highlightSprite.GetComponent<SpriteRenderer>();
        edgeHighlightRenderer = edgeHighlightSprite.GetComponent<SpriteRenderer>();
        edgeHighlightRenderer.color = edgeHighlightColor;
        automataController = FindObjectOfType<AutomataController>();
        mainCamera = FindObjectOfType<Camera>();
        audioSource = FindObjectOfType<AudioSource>();
    }

    private void OnEnable()
    {
        secondaryActionReference.action.started += ShowMenu;
        secondaryActionReference.action.canceled += CloseMenu;
    }

    private void OnDisable()
    {
        secondaryActionReference.action.started -= ShowMenu;
        secondaryActionReference.action.canceled -= CloseMenu;
    }

    private void ShowMenu(InputAction.CallbackContext context)
    {
        coroutine = null;
        rayInteractor.raycastMask = LayerMask.GetMask("State", "Edge"); // Target only States

        if (rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit raycastHit))
        {
            automataController.RestrictInterations("Edit");
            
            if (raycastHit.collider.gameObject.layer == LayerMask.NameToLayer("State"))
            {
                operation = "State";
                coroutine = StateSelection();

                state = raycastHit.collider.GetComponentInParent<State>();
            }
            else
            {
                operation = "Edge";
                coroutine = EdgeSelection();

                edge = raycastHit.transform.GetComponentInChildren<Bezier>();
                Vector3 offset = (raycastHit.transform.position.y >= mainCamera.transform.position.y) ? new Vector3(0, -0.25f, 0) : new Vector3(0, 0.25f, 0);
                edgeMenu.transform.position = raycastHit.transform.position + offset;
                edgeMenu.SetActive(true);
            }

            rayInteractor.raycastMask = 0; // Target nothing
            StartCoroutine(coroutine);
        }
        else
        {
            rayInteractor.raycastMask = ~0;
        }
    }

    private IEnumerator StateSelection()
    {
        state.HideEdges();
        HideStates();
        stateSelector.transform.position = state.transform.position;
        stateSelector.SetActive(true);
        stateSelector.transform.localScale = Vector3.zero;
        stateSelector.transform.DOKill();
        Tween grow = stateSelector.transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 0.3f);
        yield return grow.WaitForCompletion();
        rayInteractor.raycastMask = LayerMask.GetMask("StateSelectionUI");
        yield return null; // Frame delay for raycastMask to update
        while (true)
        {
            if (state != null)
            {
                if (rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit raycastHit))
                {
                    if (raycastHit.collider.transform.parent.name[0] != currentStateType)
                    {
                        highlightSprite.transform.position = raycastHit.collider.transform.position;
                        highlightSprite.SetActive(true);
                        char stateType = raycastHit.collider.transform.parent.name[0];
                        currentStateType = stateType;

                        if (stateType == 'D')
                        {
                            highlightRenderer.color = deleteHighlightColor;
                        }
                        else if (stateType == '6')
                        {
                            highlightRenderer.color = finalHighlightColor;
                        }
                        else
                        {
                            state.SetStateType(stateType - '0');

                            if (stateType == '0')
                            {
                                highlightRenderer.color = startHighlightColor;
                                state.SetInitialState(true);
                            }
                            else
                            {
                                highlightRenderer.color = normalHighlightColor;
                                state.SetInitialState(false);
                            }
                        } 
                    }
                }
                else // Raycast didn't hit anything
                {
                    highlightSprite.SetActive(false);
                    currentStateType = 'x';
                }
            }
            else if (state != null) // Raycast didn't hit anything
            {
                Debug.Log("In else if");
                highlightSprite.SetActive(false);
                currentStateType = 'x';
            }

            yield return null;
        }
    }

    private IEnumerator EdgeSelection()
    {
        edgeMenu.transform.localScale = Vector3.zero;
        edgeMenu.transform.DOKill();
        Tween grow = edgeMenu.transform.DOScale(new Vector3(1f, 1f, 1f), 0.3f);
        yield return grow.WaitForCompletion();
        rayInteractor.raycastMask = LayerMask.GetMask("StateSelectionUI");
        yield return null;
        while (true)
        {
            if (rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit raycastHit))
            {
                if (raycastHit.collider.transform.parent.name != currentSelection)
                {
                    edgeHighlightSprite.transform.position = raycastHit.collider.transform.position;
                    edgeHighlightSprite.SetActive(true);
                    currentSelection = raycastHit.collider.transform.parent.name; 
                }
            }
            else
            {
                edgeHighlightSprite.SetActive(false);
                currentSelection = "";
            }
            yield return null;
        }
    }

    private void CloseMenu(InputAction.CallbackContext context)
    {
        if (operation == "State")
        {
            if (currentStateType == 'D')
            {
                state.DeleteState();
            }
            else if (currentStateType == '6')
            {
                state.SetFinalState(!state.IsFinalState());
            }

            StopCoroutine(coroutine);

            rayInteractor.raycastMask = ~0; // Target everything

            state.ShowEdges();
            ShowStates();

            highlightSprite.SetActive(false);

            stateSelector.transform.DOScale(Vector3.zero, 0.3f).OnComplete(() => stateSelector.SetActive(false));

            // Reset variables
            currentStateType = 'x';
            state = null;

            automataController.EnableAllInteractions();
        }
        else if (operation == "Edge")
        {
            if (currentSelection == "Delete")
            {
                Destroy(edge.transform.parent.gameObject);
                rayInteractor.raycastMask = ~0; // Target everything
                automataController.EnableAllInteractions();
            }
            else if (currentSelection == "Edit")
            {
                StartCoroutine(EditEdge());
            }
            else
            {
                rayInteractor.raycastMask = ~0; // Target everything
                automataController.EnableAllInteractions();
            }

            StopCoroutine(coroutine);
            coroutine = null;

            edgeHighlightSprite.SetActive(false);
            edgeMenu.transform.DOKill();
            edgeMenu.transform.DOScale(Vector3.zero, 0.3f).OnComplete(() => edgeMenu.SetActive(false));
            currentSelection = "";
        }
        operation = "";
    }

    private IEnumerator EditEdge()
    {
        State s1 = edge.initialState.GetComponent<State>();
        State s2 = edge.targetState.GetComponent<State>();
        
        StartCoroutine(automataController.LoadKeyboard(false, s1, edge));
        while (automataController.edgeSymbols == "")
        {
            yield return null;
        }

        string newSymbols = automataController.edgeSymbols;
        automataController.edgeSymbols = "";

        if (newSymbols != "CANCELLED")
        {
            edge.SetSymbol(newSymbols);

            int s1ID = s1.GetStateID();
            string symbol = edge.GetSymbolText();
            int s2ID = s2.GetStateID();

            edge.name = "Edge " + s1ID + " " + symbol + " " + s2ID;

            audioSource.clip = validClip;
            audioSource.Play();
        }
        else
        {
            audioSource.clip = invalidClip;
            audioSource.Play();
        }

        edge = null;
    }

    private void HideStates()
    {
        Collider selfCollider = state.GetComponentInChildren<Collider>();

        Vector3 direction = selfCollider.transform.position - mainCamera.transform.position;
        float magnitude = direction.magnitude;
        hitColliders = Physics.SphereCastAll(mainCamera.transform.position, 0.4f, direction, magnitude + 10f, LayerMask.GetMask("State"));

        foreach (RaycastHit hit in hitColliders)
        {
            if (hit.collider != selfCollider)
                hit.collider.transform.parent.GetComponent<State>().DisableRenderers();
        }
    }

    private void ShowStates()
    {
        foreach (RaycastHit hit in hitColliders)
        {
            hit.collider.transform.parent.GetComponent<State>().EnableRenderers();
        }
    }
}
