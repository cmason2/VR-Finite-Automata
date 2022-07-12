using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

public class ToggleStateType : MonoBehaviour
{

    [SerializeField] XRRayInteractor rayInteractor;
    private State state;
    private Bezier edge;
    public InputActionReference rightSecondaryActionReference = null;
    public AutomataController automataController;
    [SerializeField] GameObject stateSelector;
    [SerializeField] GameObject highlightSprite;
    [SerializeField] GameObject edgeMenu;
    private SpriteRenderer highlightRenderer;
    [SerializeField] Color normalHighlightColor;
    [SerializeField] Color deleteHighlightColor;

    private IEnumerator coroutine;
    private char currentStateType = 'x';

    private void Start()
    {
        highlightRenderer = highlightSprite.GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        rightSecondaryActionReference.action.started += ShowSelectionUI;
        rightSecondaryActionReference.action.canceled += SetStateType;
    }

    private void OnDisable()
    {
        rightSecondaryActionReference.action.started -= ShowSelectionUI;
        rightSecondaryActionReference.action.canceled -= SetStateType;
    }

    private void ShowSelectionUI(InputAction.CallbackContext context)
    {
        rayInteractor.raycastMask = LayerMask.GetMask("State", "Edge"); // Target only States

        if (rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit raycastHit))
        {
            if (raycastHit.collider.gameObject.layer == LayerMask.NameToLayer("State"))
            {
                state = raycastHit.collider.GetComponentInParent<State>();
                stateSelector.transform.position = state.transform.position;
                stateSelector.SetActive(true);
                
                coroutine = StateSelection();
            }
            else
            {
                edge = raycastHit.transform.parent.GetComponentInChildren<Bezier>();
                edgeMenu.transform.position = raycastHit.transform.position;
                edgeMenu.SetActive(true);

                coroutine = EdgeSelection();
            }

            StartCoroutine(coroutine);
        }
    }

    private void SetStateType(InputAction.CallbackContext context)
    {
        if (coroutine != null)
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

            // Reset variables
            currentStateType = 'x';
            state = null;

            rayInteractor.raycastMask = ~0; // Target everything

            highlightSprite.SetActive(false);
            stateSelector.SetActive(false);
        }
    }

    private IEnumerator StateSelection()
    {
        rayInteractor.raycastMask = LayerMask.GetMask("StateSelectionUI");
        yield return null;
        while (true)
        {
            if (state != null && rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit raycastHit) && raycastHit.collider.transform.parent.name[0] != currentStateType)
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

            yield return null;
        }
    }

    private IEnumerator EdgeSelection()
    {
        yield return null;
    }
}
