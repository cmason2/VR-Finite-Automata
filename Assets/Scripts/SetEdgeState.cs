using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetEdgeState : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Bezier edge;
    public void DisableCurves()
    {
        edge.enabled = false;
    }

    public void EnableCurves()
    {
        edge.enabled = true;
    }
}
