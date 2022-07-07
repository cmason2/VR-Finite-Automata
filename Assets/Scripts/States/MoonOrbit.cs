using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoonOrbit : MonoBehaviour
{
    private Transform targetTransform;
    private Vector3 rotationVector = new Vector3(1,1,0);
    public float orbitSpeed = 180;

    // Start is called before the first frame update
    void Start()
    {
        targetTransform = transform.parent.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(targetTransform.position, rotationVector.normalized, orbitSpeed * Time.deltaTime);
    }
}
