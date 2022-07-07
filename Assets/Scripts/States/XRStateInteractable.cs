using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRStateInteractable : XRGrabInteractable
{

    public State stateScript;
    private XRInteractorLineVisual lineVisual;

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

    protected override void OnActivated(ActivateEventArgs args)
    {
        stateScript.DeleteState();
        base.OnActivated(args);
    }
}
