using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class FadeOverlay : MonoBehaviour
{
    private Image fadeOverlay;

    void Start()
    {
        fadeOverlay = GetComponent<Image>();
        fadeOverlay.enabled = true;
        fadeOverlay.color = Color.black;
        fadeOverlay.DOFade(0f, 2f);
    }

    public Tween FadeToBlack()
    {
        return fadeOverlay.DOFade(1f, 2f);
    }
}
