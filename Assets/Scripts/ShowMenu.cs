using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using DG.Tweening;

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
    [SerializeField] CreateState leftCreateStateScript;
    [SerializeField] EditMenu leftEditScript;

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
            menu.transform.DOScale(0.0f, 0.3f).OnComplete(HideMenu);
        }
        else
        {
            leftCreateStateScript.enabled = false;
            leftEditScript.enabled = false;
            rayInteractor.enabled = false;
            menu.transform.localScale = Vector3.zero;
            menu.SetActive(true);
            menu.transform.DOScale(0.3f, 0.3f);
        }

        audioSource.Play();
    }

    private void HideMenu()
    {
        menu.SetActive(false);
        rayInteractor.enabled = true;
        leftCreateStateScript.enabled = true;
        leftEditScript.enabled = true;
    }
}
