using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class ToggleStateType : MonoBehaviour
{

    [SerializeField] XRRayInteractor rayInteractor;
    private State state1;
    public InputActionReference rightSecondaryActionReference = null;
    public AutomataController automataController;

    private void Awake()
    {
        rightSecondaryActionReference.action.started += ToggleState;
    }

    private void OnDestroy()
    {
        rightSecondaryActionReference.action.started -= ToggleState;
    }

    private void ToggleState(InputAction.CallbackContext context)
    {
        rayInteractor.raycastMask = LayerMask.GetMask("State"); // Target only states

        if (rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit raycastHit))
        {
            state1 = raycastHit.collider.GetComponentInParent<State>();
            state1.SetFinalState(!state1.IsFinalState()); // Toggle isFinalState bool
        }
    }
}
