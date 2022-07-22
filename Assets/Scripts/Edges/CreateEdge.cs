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

    private void OnEnable()
    {
        triggerActionReference.action.started += ButtonPressed;
        triggerActionReference.action.canceled += ButtonReleased;
    }

    private void OnDisable()
    {
        triggerActionReference.action.started -= ButtonPressed;
        triggerActionReference.action.canceled -= ButtonReleased;
    }

    private void ButtonPressed(InputAction.CallbackContext context)
    {
        automataController.RestrictInterations("CreateEdge");
        if (rayInteractor.TryGetCurrentRaycast(out RaycastHit? raycastHit, out int raycastHitIndex, out UnityEngine.EventSystems.RaycastResult? uiRaycastHit, out int uiRatcastHitIndex, out bool isUiHitClosest) && !isUiHitClosest)
        {
            Debug.Log("Inside if");
            state1 = raycastHit?.collider.gameObject;
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
            if (state2 == state1)
            {
                StartCoroutine(MakeEdge(true));
            }
            else
            {
                StartCoroutine(MakeEdge(false));
            }
        }
        else
        {
            automataController.EnableAllInteractions();
        }
    }

    IEnumerator MakeEdge(bool loop)
    {
        audioSource.clip = popAudio;
        audioSource.Play();

        GameObject edge;

        if (loop)
        {
            edge = Instantiate(edgePrefab, state1.transform.position + new Vector3(0f, 0.5f, 0f), Quaternion.identity);
            edge.GetComponentInChildren<LineRenderer>().useWorldSpace = false;
            edge.transform.parent = state1.transform.parent;
        }
        else
        {
            Vector3 midPoint = state1.transform.position + (state2.transform.position - state1.transform.position) / 2;
            edge = Instantiate(edgePrefab, midPoint, Quaternion.identity);
        }
        
        Bezier curve = edge.GetComponentInChildren<Bezier>();
        curve.SetLoop(loop);
        curve.SetStates(state1.transform.parent, state2.transform.parent);

        State state1Script = state1.GetComponentInParent<State>();
        State state2Script = state2.GetComponentInParent<State>();

        // Start keyboard coroutine to get input symbols
        StartCoroutine(automataController.LoadKeyboard(true, state1Script, curve));
        while (automataController.edgeSymbols == "")
        {
            yield return null;
        }

        string inputSymbols = automataController.edgeSymbols;
        automataController.edgeSymbols = "";

        if (inputSymbols != "CANCELLED")
        {
            curve.SetSymbol(inputSymbols);
            curve.enabled = false;
            
            state1Script.AddEdge(curve);
            if(!loop)
                state2Script.AddEdge(curve);

            int s1ID = state1Script.GetStateID();
            string symbol = curve.GetSymbolText();
            int s2ID = state2Script.GetStateID();

            edge.name = "Edge " + s1ID + " " + symbol + " " + s2ID;

            // Add new transition in AutomataController
            automataController.AddTransition(state1Script, curve, state2Script);
            audioSource.clip = secondSelectedAudio;
            audioSource.Play();
        }
        else
        {
            audioSource.clip = errorAudio;
            audioSource.Play();
            Destroy(edge);
        }
    }
}
