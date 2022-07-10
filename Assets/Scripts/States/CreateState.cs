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
    public GameObject statePrefab;
    public AutomataController automataController;

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip spawnStateAudio;

    GameObject newState = null;

    private void OnEnable()
    {
        rightPrimaryActionReference.action.started += SpawnState;
        rightPrimaryActionReference.action.canceled += ReleaseState;
    }

    private void OnDisable()
    {
        rightPrimaryActionReference.action.started -= SpawnState;
        rightPrimaryActionReference.action.canceled -= ReleaseState;
    }

    private void SpawnState(InputAction.CallbackContext context)
    {
        lineVisual.enabled = false;

        audioSource.clip = spawnStateAudio;
        audioSource.Play();

        int id = automataController.GetNextStateID();

        newState = Instantiate(statePrefab, spawnPoint.position, Quaternion.identity);

        StartCoroutine(UpdateStatePosition(newState));
        newState.name = "State " + id;

        State stateScript = newState.GetComponent<State>();

        if (automataController.GetNumStates() == 0)
        {
            //Change colour and set start state
            stateScript.SetInitialState(true);
            stateScript.SetStateType(0);
        }
        else
        {
            stateScript.SetStateType(1);
        }
        
        stateScript.SetStateID(id);
        stateScript.SetMaterial();

        automataController.AddState(stateScript);
    }

    private void ReleaseState(InputAction.CallbackContext obj)
    {
        StopAllCoroutines();
        lineVisual.enabled = true;
        newState.transform.parent = null;
        Debug.Log(automataController.CheckAutomataValidity());
    }

    IEnumerator UpdateStatePosition(GameObject state)
    {
        while (true)
        {
            state.transform.position = spawnPoint.position;
            yield return null;
        }
    }
}
