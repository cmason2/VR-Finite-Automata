using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class ToggleStateType : MonoBehaviour
{

    [SerializeField] XRRayInteractor rayInteractor;
    private State state;
    public InputActionReference rightSecondaryActionReference = null;
    public AutomataController automataController;
    [SerializeField] GameObject stateSelector;

    private IEnumerator coroutine;
    private char currentStateType = 'x';

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
        rayInteractor.raycastMask = LayerMask.GetMask("State"); // Target only States

        if (rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit raycastHit))
        {
            state = raycastHit.collider.GetComponentInParent<State>();
            stateSelector.transform.position = state.transform.position;
            stateSelector.SetActive(true);
        }
        coroutine = StateSelection();
        StartCoroutine(coroutine);
    }

    private void SetStateType(InputAction.CallbackContext context)
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

        stateSelector.SetActive(false);
    }

    private IEnumerator StateSelection()
    {
        rayInteractor.raycastMask = LayerMask.GetMask("StateSelectionUI");
        yield return null;
        while (true)
        {
            if (state != null && rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit raycastHit) && raycastHit.collider.transform.parent.name[0] != currentStateType)
            {
                Debug.Log(currentStateType);
                char stateType = raycastHit.collider.transform.parent.name[0];
                Debug.Log(raycastHit.collider.transform.parent.name);
                currentStateType = stateType;

                if (stateType != 'D' && stateType != '6')
                {
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
}
