using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XREdgeInteractable : XRGrabInteractable
{
    private XRInteractorLineVisual lineVisual;
    [SerializeField] EdgeControlDisplay edgeDisplayScript;
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

    protected override void OnHoverEntering(HoverEnterEventArgs args)
    {
        edgeDisplayScript.StopAllCoroutines();
        edgeDisplayScript.SetInitialColor();
        base.OnHoverEntering(args);
    }

    protected override void OnHoverExiting(HoverExitEventArgs args)
    {
        edgeDisplayScript.StartFadeOut();
        base.OnHoverExiting(args);
    }
}
