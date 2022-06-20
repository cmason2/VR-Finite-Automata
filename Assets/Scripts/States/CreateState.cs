using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class CreateState : MonoBehaviour
{
    public XRInteractorLineVisual lineVisual;
    public InputActionReference rightPrimaryActionReference = null;
    public Transform spawnPoint;
    public Transform prefab;
    public AutomataController automataController;

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip spawnStateAudio;

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
        lineVisual.enabled = false;
        newState = Instantiate(prefab, spawnPoint.position, Quaternion.identity);

        audioSource.clip = spawnStateAudio;
        audioSource.Play();

        State stateScript = newState.GetComponent<State>();

        if (automataController.GetNumStates() == 0)
        {
            //Change colour and set start state
            stateScript.SetInitialState(true);
        }

        int id = automataController.GetNextStateID();
        stateScript.SetStateID(id);
        newState.name = "State " + id;
        newState.SetParent(spawnPoint);

        automataController.AddState(stateScript);
    }

    private void ReleaseState(InputAction.CallbackContext obj)
    {
        lineVisual.enabled = true;
        newState.parent = null;
        Debug.Log(automataController.CheckAutomataValidity());
    }
}
