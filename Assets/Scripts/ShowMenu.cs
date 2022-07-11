using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class ShowMenu : MonoBehaviour
{

    public XRInteractorLineVisual lineVisual;
    public XRRayInteractor rayInteractor;
    public InputActionReference leftPrimaryActionReference = null;

    public SkinnedMeshRenderer controllerRenderer;


    public GameObject menu;
    public GameObject keyboard;

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip menuToggleAudio;

    private void OnEnable()
    {
        leftPrimaryActionReference.action.started += ToggleMenu;
    }

    private void OnDisable()
    {
        leftPrimaryActionReference.action.started -= ToggleMenu;
    }

    private void ToggleMenu(InputAction.CallbackContext obj)
    {
        audioSource.clip = menuToggleAudio;

        if (menu.activeInHierarchy)
        {
            controllerRenderer.enabled = true;
            rayInteractor.enabled = true;
            //lineVisual.enabled = true;
            menu.SetActive(false);
        }
        else
        {
            controllerRenderer.enabled = false;
            rayInteractor.enabled = false;
            //lineVisual.enabled = false;
            menu.SetActive(true);
        }

        audioSource.Play();
    }
}
