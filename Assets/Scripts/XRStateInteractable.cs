using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRStateInteractable : XRGrabInteractable
{
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnActivated(ActivateEventArgs args)
    {
        Destroy(gameObject);
        base.OnActivated(args);
    }
}
