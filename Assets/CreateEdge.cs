using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class CreateEdge : MonoBehaviour
{

    [SerializeField] XRRayInteractor rayInteractor;
    public InputActionReference triggerActionReference = null;
    public GameObject edgePrefab;
    public AutomataController automataController;

    private GameObject state1;
    private GameObject state2;

    UnityEngine.TouchScreenKeyboard keyboard;
    public static string keyboardText = "";

    private void Awake()
    {
        triggerActionReference.action.started += ButtonPressed;
        triggerActionReference.action.canceled += ButtonReleased;
    }

    private void OnDestroy()
    {
        triggerActionReference.action.started -= ButtonPressed;
    }

    private void ButtonPressed(InputAction.CallbackContext context)
    {
        //rayInteractor.raycastMask = LayerMask.GetMask("State"); // Target only states

        if (rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit raycastHit))
        {
            state1 = raycastHit.collider.gameObject;
            if (state1.tag != "State")
            {
                Debug.Log("First selection was not a state");
                state1 = null;
                return;
            }
        }
    }

    private void ButtonReleased(InputAction.CallbackContext obj)
    {
        if (state1 != null && rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit raycastHit))
        {
            state2 = raycastHit.collider.gameObject;
            if (state2.tag != "State")
            {
                Debug.Log("Second selection was not a state");

                return;
            }
            else if (state2 == state1)
            {
                Debug.Log("Selected states are the same, make loop edge");
                return;
            }
        }
        else
        {
            return;
        }

        Vector3 midPoint = state1.transform.position + (state2.transform.position - state1.transform.position) / 2;
        GameObject edge = Instantiate(edgePrefab, midPoint, Quaternion.identity);
        //edge.transform.SetParent(state1.transform, true);
        Bezier curve = edge.GetComponentInChildren<Bezier>();
        curve.SetStates(state1.transform, state2.transform);

        // Add reference to edge in both states
        State state1Script = state1.GetComponentInParent<State>();
        State state2Script = state2.GetComponentInParent<State>();
        state1Script.AddEdge(edge);
        state2Script.AddEdge(edge);

        // Assign symbol to state
        keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, false, false, "Enter symbol", 0);
        keyboardText = keyboard.text;
        curve.SetSymbol("z");

        // Update AutomataController with new state information
        automataController.AddTransition(state1Script.GetStateID(), curve.GetSymbol(), state2Script.GetStateID());
    }
}
