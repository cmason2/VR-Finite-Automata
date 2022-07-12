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
