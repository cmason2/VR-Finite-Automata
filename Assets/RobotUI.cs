using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class RobotUI : MonoBehaviour
{
    [SerializeField] TMP_Text speechText;

    [SerializeField] Button homeButton, startButton;
    [SerializeField] Transform robotTransform;

    private void Start()
    {
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
        // Hide buttons

        Sequence seq = DOTween.Sequence();

        seq.Append(robotTransform.DOLocalRotate(new Vector3(0, 360, 0), 2f, RotateMode.FastBeyond360).SetRelative(true).SetEase(Ease.InOutCubic));
        seq.Insert(0, startButton.transform.DOScale(0f, 0.5f));
        seq.Insert(0, homeButton.transform.DOScale(0f, 0.5f));
        seq.Append(transform.DOScaleY(0f, 0.5f));

    }
}
