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
        if (rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit raycastHit))
        {
            state1 = raycastHit.collider.gameObject;
        }
    }

    private void ButtonReleased(InputAction.CallbackContext obj)
    {
        if (rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit raycastHit))
        {
            state2 = raycastHit.collider.gameObject;
            if (state2 == state1)
            {
                Debug.Log("Selected states are the same, make loop edge");
                return;
            }
            else if (state2.tag != "State")
            {
                Debug.Log("Second selection was not a state");
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

        State state1Script = state1.GetComponentInParent<State>();
        state1Script.AddEdge(edge);

        keyboard = TouchScreenKeyboard.Open("");
        curve.SetSymbol(keyboard.text);
    }
}
