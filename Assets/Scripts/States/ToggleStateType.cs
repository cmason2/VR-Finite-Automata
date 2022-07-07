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

    private void Awake()
    {
        rightSecondaryActionReference.action.started += ShowSelectionUI;
        rightSecondaryActionReference.action.canceled += SetStateType;
    }

    private void OnDestroy()
    {
        rightSecondaryActionReference.action.started -= ShowSelectionUI;
    }

    private void ShowSelectionUI(InputAction.CallbackContext context)
    {
        rayInteractor.raycastMask = LayerMask.GetMask("State"); // Target only States

        if (rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit raycastHit))
        {
            state = raycastHit.collider.GetComponentInParent<State>();
            stateSelector.transform.position = state.transform.position;
            stateSelector.SetActive(true);
            rayInteractor.raycastMask = LayerMask.GetMask("StateSelectionUI");
        }        
    }

    private void SetStateType(InputAction.CallbackContext context)
    {
        if (state != null && rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit raycastHit))
        {
            char stateType = raycastHit.collider.transform.parent.name[0];
            if (stateType == 'D')
            {
                state.DeleteState();
            }
            else if (stateType == '6') // Toggle final state
            {
                state.SetFinalState(!state.IsFinalState());
            }
            else
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

        rayInteractor.raycastMask = ~0; // Target everything

        stateSelector.SetActive(false);
    }
}
