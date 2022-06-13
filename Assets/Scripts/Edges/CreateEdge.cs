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

    [SerializeField] AudioClip firstSelectedAudio;
    [SerializeField] AudioClip secondSelectedAudio;
    [SerializeField] AudioClip errorAudio;
    [SerializeField] AudioClip popAudio;
    [SerializeField] AudioSource audioSource;

    private GameObject state1;
    private GameObject state2;

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
            if (state1.tag != "State")
            {
                state1 = null;
            }   
            else
            {
                audioSource.clip = firstSelectedAudio;
                audioSource.Play();
                rayInteractor.raycastMask = LayerMask.GetMask("State"); // Target only states
            } 
        }
    }

    private void ButtonReleased(InputAction.CallbackContext obj)
    {
        if (state1 != null && rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit raycastHit))
        {
            state2 = raycastHit.collider.gameObject;
            if (state2 != state1)
            {
                StartCoroutine(MakeState());
            }
            else
            {
                Debug.Log("Selected states are the same, make loop edge");
            }
        }
        rayInteractor.raycastMask = ~0; // Target everything after both states have been selected
    }

    IEnumerator MakeState()
    {
        audioSource.clip = popAudio;
        audioSource.Play();

        Vector3 midPoint = state1.transform.position + (state2.transform.position - state1.transform.position) / 2;
        GameObject edge = Instantiate(edgePrefab, midPoint, Quaternion.identity);
        //edge.transform.SetParent(state1.transform, true);
        Bezier curve = edge.GetComponentInChildren<Bezier>();
        curve.SetStates(state1.transform.parent, state2.transform.parent);

        State state1Script = state1.GetComponentInParent<State>();
        State state2Script = state2.GetComponentInParent<State>();

        // Start keyboard coroutine to get input symbols
        yield return StartCoroutine(automataController.LoadKeyboard(state1Script, curve));

        string inputSymbols = automataController.edgeSymbols;
        if (inputSymbols != "CANCELLED")
        {
            curve.SetSymbol(inputSymbols);
            
            state1Script.AddEdge(curve);
            state2Script.AddEdge(curve);

            int s1ID = state1Script.GetStateID();
            int s2ID = state2Script.GetStateID();

            edge.name = "Edge " + s1ID + " " + inputSymbols + " " + s2ID;

            // Add new transition in AutomataController
            automataController.AddTransition(s1ID, curve.GetSymbolList(), s2ID);
            audioSource.clip = secondSelectedAudio;
            audioSource.Play();
        }
        else
        {
            audioSource.clip = errorAudio;
            audioSource.Play();
            Debug.Log("Edges containing one or more of the selected symbols are already present in other transitions from this state!");
            Destroy(edge);
        }
    }
}
