using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTriggerVolume : MonoBehaviour
{

    public bool stateInside = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        if ((other.transform.position - transform.position).magnitude <= (transform.localScale.x/2) - 0.15)
        {
            stateInside = true;
        }
    }
}
