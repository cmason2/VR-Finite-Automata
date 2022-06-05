using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class CreateState : MonoBehaviour
{

    public InputActionReference rightPrimaryActionReference = null;
    public Transform controller;
    public Transform prefab;
    public AutomataController automataController;

    Transform newState = null;

    private void Awake()
    {
        rightPrimaryActionReference.action.started += SpawnState;
        rightPrimaryActionReference.action.canceled += ReleaseState;
    }

    private void OnDestroy()
    {
        rightPrimaryActionReference.action.started -= SpawnState;
    }

    private void SpawnState(InputAction.CallbackContext context)
    {
        newState = Instantiate(prefab, controller.position, controller.rotation);
        State stateScript = newState.GetComponent<State>();

        if (automataController.GetNumStates() == 0)
        {
            //Change colour and set start state
            stateScript.SetInitialState(true);
        }

        int id = automataController.GetNextStateID();
        stateScript.SetStateID(id);
        newState.name = "State " + id;
        newState.SetParent(controller);

        automataController.AddState(stateScript);
    }

    private void ReleaseState(InputAction.CallbackContext obj)
    {
        newState.parent = null;
        Debug.Log(automataController.CheckAutomataValidity());
    }
}
