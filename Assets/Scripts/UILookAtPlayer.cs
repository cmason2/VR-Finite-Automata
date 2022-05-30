using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILookAtPlayer : MonoBehaviour
{
    Camera playerCamera;

    private void Start()
    {
        playerCamera = Camera.main;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.LookAt(this.transform.position - playerCamera.transform.position);
    }
}
