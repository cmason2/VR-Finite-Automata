using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoonOrbit : MonoBehaviour
{
    [SerializeField] Transform targetTransform;
    private Vector3 rotationVector = new Vector3(1,1,0);
    public float orbitSpeed = 180;

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(targetTransform.position, rotationVector.normalized, orbitSpeed * Time.deltaTime);
    }
}
