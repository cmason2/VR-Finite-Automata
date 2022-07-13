using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

public class EditMenu : MonoBehaviour
{

    [SerializeField] XRRayInteractor rayInteractor;
    private State state;
    private Bezier edge;
    public InputActionReference rightSecondaryActionReference = null;
    private AutomataController automataController;
    [SerializeField] GameObject stateSelector;
    [SerializeField] GameObject highlightSprite;
    [SerializeField] GameObject edgeHighlightSprite;
    [SerializeField] GameObject edgeMenu;
    private SpriteRenderer highlightRenderer;
    private SpriteRenderer edgeHighlightRenderer;
    [SerializeField] Color normalHighlightColor;
    [SerializeField] Color deleteHighlightColor;
    [SerializeField] Color edgeHighlightColor;

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
    }

    private void OnEnable()
    {
        rightSecondaryActionReference.action.started += ShowMenu;
        rightSecondaryActionReference.action.canceled += CloseMenu;
    }

    private void OnDisable()
    {
        rightSecondaryActionReference.action.started -= ShowMenu;
        rightSecondaryActionReference.action.canceled -= CloseMenu;
    }

    private void ShowMenu(InputAction.CallbackContext context)
    {
        rayInteractor.raycastMask = LayerMask.GetMask("State", "Edge"); // Target only States

        if (rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit raycastHit))
        {
            if (raycastHit.collider.gameObject.layer == LayerMask.NameToLayer("State"))
            {
                state = raycastHit.collider.GetComponentInParent<State>();
                state.HideEdges();
                stateSelector.transform.position = state.transform.position;
                stateSelector.SetActive(true);

                operation = "State";
                coroutine = StateSelection();
            }
            else
            {
                edge = raycastHit.transform.GetComponentInChildren<Bezier>();
                edgeMenu.transform.position = raycastHit.transform.position + new Vector3(0, -0.25f, 0);
                edgeMenu.SetActive(true);

                operation = "Edge";
                coroutine = EdgeSelection();
            }

            StartCoroutine(coroutine);
        }
    }

    private IEnumerator StateSelection()
    {
        rayInteractor.raycastMask = LayerMask.GetMask("StateSelectionUI");
        yield return null;
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
                            highlightRenderer.color = normalHighlightColor;
                        }
                        else
                        {
                            highlightRenderer.color = normalHighlightColor;
                            state.SetStateType(stateType - '0');

                            if (stateType == '0')
                            {
                                state.SetInitialState(true);
                            }
                            else
                            {
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
        rayInteractor.raycastMask = LayerMask.GetMask("StateSelectionUI");
        yield return null;
        while (true)
        {
            if (rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit raycastHit) && raycastHit.collider.transform.parent.name != currentSelection)
            {
                edgeHighlightSprite.transform.position = raycastHit.collider.transform.position;
                edgeHighlightSprite.SetActive(true);
                currentSelection = raycastHit.collider.transform.parent.name;
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
            coroutine = null;

            rayInteractor.raycastMask = ~0; // Target everything

            state.ShowEdges();
            highlightSprite.SetActive(false);
            stateSelector.SetActive(false);

            // Reset variables
            currentStateType = 'x';
            state = null;
        }
        else if (operation == "Edge")
        {
            if (currentSelection == "Delete")
            {
                Destroy(edge.transform.parent.gameObject);
            }
            else if (currentSelection == "Edit")
            {

            }

            StopCoroutine(coroutine);
            coroutine = null;

            rayInteractor.raycastMask = ~0; // Target everything

            edgeHighlightSprite.SetActive(false);
            edgeMenu.SetActive(false);

            // Reset variables
            currentSelection = "";
            edge = null;
        }

        operation = "";
    }
}
