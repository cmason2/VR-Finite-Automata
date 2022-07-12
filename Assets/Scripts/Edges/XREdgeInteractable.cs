using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XREdgeInteractable : XRGrabInteractable
{
    private XRInteractorLineVisual lineVisual;
    private ActionBasedContinuousMoveProvider playerMovement;

    private void Start()
    {
        playerMovement = FindObjectOfType<ActionBasedContinuousMoveProvider>();
    }

    protected override void OnSelectEntering(SelectEnterEventArgs args)
    {
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
        base.OnSelectExiting(args);
    }
}
