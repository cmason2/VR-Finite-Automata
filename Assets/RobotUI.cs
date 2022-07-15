using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

public class RobotUI : MonoBehaviour
{
    [SerializeField] TMP_Text speechText;

    [SerializeField] Button homeButton, startButton;
    [SerializeField] Transform robotTransform;

    [SerializeField] Animator robotAnimator;

    private AutomataController automataController;

    private void Start()
    {
        automataController = FindObjectOfType<AutomataController>();
        automataController.RestrictInterations("TutorialStart");

        homeButton.onClick.AddListener(GoMainMenu);
        startButton.onClick.AddListener(StartTutorial);

        homeButton.transform.localScale = Vector3.zero;
        startButton.transform.localScale = Vector3.zero;

        // Show first UI
        transform.localScale = new Vector3(1,0,1);

        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(1f);
        seq.Append(transform.DOScaleY(1f, 0.5f).SetEase(Ease.OutBack));
        seq.Append(startButton.transform.DOScale(1f, 0.5f));
        seq.Insert(1.5f, homeButton.transform.DOScale(1f, 0.5f));
    }

    public void ChangeText(string textToDisplay)
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOScaleY(0f, 0.5f).OnComplete(() => speechText.text = textToDisplay));
        seq.AppendInterval(1f);
        seq.Append(transform.DOScaleY(1f, 0.5f).SetEase(Ease.OutBack));
    }

    public void HideUI()
    {
        transform.DOScaleY(0f, 0.5f);
    }

    private void GoMainMenu()
    {

    }

    private void StartTutorial()
    {
        robotAnimator.SetTrigger("Love");

        Sequence seq = DOTween.Sequence().OnComplete(() => robotAnimator.SetTrigger("Base"));

        seq.Append(robotTransform.DOLocalRotate(new Vector3(0, 360, 0), 2f, RotateMode.FastBeyond360).SetRelative(true).SetEase(Ease.InOutBack));
        seq.Join(startButton.transform.DOScale(0f, 0.5f));
        seq.Join(homeButton.transform.DOScale(0f, 0.5f));
        seq.Insert(0.5f, robotTransform.DOJump(robotTransform.position, 0.5f, 1, 1f));
        seq.Insert(0.5f, transform.DOScaleY(0f, 0.5f));

        StartCoroutine(Tutorial());
    }

    private IEnumerator Tutorial()
    {
        automataController.EnableInteraction("CreateState");
        yield return null;
        
    }
}
