using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XREdgeInteractable : XRGrabInteractable
{
    private XRInteractorLineVisual lineVisual;
    private ActionBasedContinuousMoveProvider playerMovement;
    private AutomataController automataController;
    public Bezier edge;

    private void Start()
    {
        automataController = FindObjectOfType<AutomataController>();
        playerMovement = FindObjectOfType<ActionBasedContinuousMoveProvider>();
    }

    protected override void OnHoverEntering(HoverEnterEventArgs args)
    {
        base.OnHoverEntering(args);
        if (!automataController.isStepping)
            edge.SetColour(Color.white);
    }

    protected override void OnHoverExiting(HoverExitEventArgs args)
    {
        base.OnHoverExiting(args);
        if (!automataController.isStepping)
            edge.SetColour(edge.edgeColour);
    }

    protected override void OnSelectEntering(SelectEnterEventArgs args)
    {
        automataController.RestrictInterations("ObjectGrabbed");
        playerMovement.enabled = false;
        lineVisual = args.interactorObject.transform.gameObject.GetComponent<XRInteractorLineVisual>();
        lineVisual.enabled = false;
        base.OnSelectEntering(args);
    }

    protected override void OnSelectExiting(SelectExitEventArgs args)
    {
        playerMovement.enabled = true;
        if (lineVisual != null)
        {
            lineVisual.enabled = true;
        }
        automataController.EnableAllInteractions();
        base.OnSelectExiting(args);
    }
}
