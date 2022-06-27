using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class EdgeControlDisplay : MonoBehaviour
{
    [SerializeField] BoxCollider symbolCollider;
    [SerializeField] XRSimpleInteractable interactableScript;
    [SerializeField] MeshRenderer controlMeshRenderer;
    [SerializeField] SphereCollider controlCollider;

    private Color initialColor;
    private Color finalColor;

    private void Start()
    {
        initialColor = controlMeshRenderer.material.color;
        finalColor = new Color(initialColor.r, initialColor.g, initialColor.b, 0f);
    }

    public void StartFadeOut()
    {
        symbolCollider.enabled = false;
        interactableScript.enabled = false;
        StopAllCoroutines();
        controlMeshRenderer.material.color = initialColor;
        StartCoroutine(ShowControl());
    }

    IEnumerator ShowControl()
    {
        controlMeshRenderer.enabled = true;
        controlCollider.enabled = true;

        yield return new WaitForSeconds(4);

        // Fade out
        float fadeTime = 1f;
        float elapsedTime = 0f;
        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            controlMeshRenderer.material.color = Color.Lerp(initialColor, finalColor, elapsedTime / fadeTime);
            yield return null;
        }

        symbolCollider.enabled = true;
        interactableScript.enabled = true;
        controlCollider.enabled = false;
        controlMeshRenderer.enabled = false;
    }

    public void SetInitialColor()
    {
        controlMeshRenderer.material.color = initialColor;
    }
}
