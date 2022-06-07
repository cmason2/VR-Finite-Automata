using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRStateInteractable : XRGrabInteractable
{

    public State stateScript;
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnActivated(ActivateEventArgs args)
    {
        stateScript.OnActivated();
        base.OnActivated(args);
    }
}
