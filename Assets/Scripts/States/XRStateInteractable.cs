using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRStateInteractable : XRGrabInteractable
{
    [SerializeField] State stateScript;
    [SerializeField] StateOutline stateOutline;
    private XRInteractorLineVisual lineVisual;
    private ActionBasedContinuousMoveProvider playerMovement;
    private AutomataController automataController;

    private void Start()
    {
        automataController = FindObjectOfType<AutomataController>();
        playerMovement = FindObjectOfType<ActionBasedContinuousMoveProvider>();
    }

    protected override void OnHoverEntering(HoverEnterEventArgs args)
    {
        base.OnHoverEntering(args);
        if (automataController == null)
            automataController = FindObjectOfType<AutomataController>();

        if (!automataController.isStepping)
            stateOutline.enabled = true;
    }

    protected override void OnHoverExiting(HoverExitEventArgs args)
    {
        base.OnHoverExiting(args);
        if (automataController == null)
            automataController = FindObjectOfType<AutomataController>();

        if (!automataController.isStepping)
            stateOutline.enabled = false;
    }

    protected override void OnSelectEntering(SelectEnterEventArgs args)
    {
        stateScript.isGrabbed = true;
        automataController.RestrictInterations("ObjectGrabbed");
        playerMovement.enabled = false;
        lineVisual = args.interactorObject.transform.gameObject.GetComponent<XRInteractorLineVisual>();
        lineVisual.enabled = false;
        StartCoroutine(EdgeMovement());
        base.OnSelectEntering(args);
    }

    protected override void OnSelectExiting(SelectExitEventArgs args)
    {
        stateScript.isGrabbed = false;
        playerMovement.enabled = true;
        if (lineVisual != null)
        {
            lineVisual.enabled = true;
        }
        automataController.EnableAllInteractions();
        base.OnSelectExiting(args);
    }

    private IEnumerator EdgeMovement()
    {
        Vector3 currentPos;
        Vector3 moveVector;
        while (stateScript.isGrabbed)
        {
            currentPos = transform.position;
            yield return null;
            moveVector = (transform.position - currentPos);
            stateScript.MoveAttachedEdges(moveVector/2);
        }
        
    }
}
