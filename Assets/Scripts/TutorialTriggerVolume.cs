using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTriggerVolume : MonoBehaviour
{

    public bool stateInside = false;

    private void OnTriggerStay(Collider other)
    {
        if ((other.transform.position - transform.position).magnitude <= (transform.localScale.x/2) - 0.15
            && !other.transform.parent.GetComponent<State>().isGrabbed)
        {
            stateInside = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        stateInside = false;
    }
}
