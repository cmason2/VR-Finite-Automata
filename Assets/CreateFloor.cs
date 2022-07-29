using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class CreateFloor : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        List<XRInputSubsystem> subsystems = new List<XRInputSubsystem>();
        SubsystemManager.GetInstances<XRInputSubsystem>(subsystems);
        if (subsystems.Count > 0)
        {
            List<Vector3> boundaryPoints = new List<Vector3>();
            Debug.Log(subsystems[0].TryGetBoundaryPoints(boundaryPoints));
        }
        else
        {
            Debug.Log("No subsystems found");
        }
    }
}
