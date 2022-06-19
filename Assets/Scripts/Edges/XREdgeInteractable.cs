using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XREdgeInteractable : XRGrabInteractable
{
    private XRInteractorLineVisual lineVisual;
    [SerializeField] EdgeControlDisplay edgeDisplayScript;

    protected override void OnSelectEntering(SelectEnterEventArgs args)
    {
        lineVisual = args.interactorObject.transform.gameObject.GetComponent<XRInteractorLineVisual>();
        lineVisual.enabled = false;
        base.OnSelectEntering(args);
    }

    protected override void OnSelectExiting(SelectExitEventArgs args)
    {
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
