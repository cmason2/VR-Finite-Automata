using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class CreateState : MonoBehaviour
{

    public InputActionReference bButtonReference = null;
    public Transform controller;
    public Transform prefab;

    // Start is called before the first frame update
    private void Awake()
    {
        bButtonReference.action.started += SpawnState;
    }

    private void OnDestroy()
    {
        bButtonReference.action.started -= SpawnState;
    }

    private void SpawnState(InputAction.CallbackContext context)
    {
        Instantiate(prefab, controller.position, controller.rotation);
    }
}
