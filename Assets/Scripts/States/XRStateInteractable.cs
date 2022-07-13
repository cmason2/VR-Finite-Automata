using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRStateInteractable : XRGrabInteractable
{

    public State stateScript;
    private XRInteractorLineVisual lineVisual;
    private ActionBasedContinuousMoveProvider playerMovement;
    private AutomataController automataController;
    private bool grabbed = false;

    private void Start()
    {
        automataController = FindObjectOfType<AutomataController>();
        playerMovement = FindObjectOfType<ActionBasedContinuousMoveProvider>();
    }

    protected override void OnSelectEntering(SelectEnterEventArgs args)
    {
        automataController.RestrictInterations("ObjectGrabbed");
        playerMovement.enabled = false;
        lineVisual = args.interactorObject.transform.gameObject.GetComponent<XRInteractorLineVisual>();
        lineVisual.enabled = false;
        //stateScript.ParentEdges();
        grabbed = true;
        StartCoroutine(EdgeMovement());
        base.OnSelectEntering(args);
    }

    protected override void OnSelectExiting(SelectExitEventArgs args)
    {
        grabbed = false;
        playerMovement.enabled = true;
        if (lineVisual != null)
        {
            lineVisual.enabled = true;
        }
        automataController.EnableAllInteractions();
        //stateScript.UnparentEdges();
        base.OnSelectExiting(args);
    }

    private IEnumerator EdgeMovement()
    {
        Vector3 currentPos;
        Vector3 moveVector;
        while (grabbed)
        {
            currentPos = transform.position;
            yield return null;
            moveVector = (transform.position - currentPos);
            stateScript.MoveAttachedEdges(moveVector/2);
        }
        
    }
}
