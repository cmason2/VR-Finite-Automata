using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.InputSystem;
using System;
using UnityEngine.SceneManagement;

public class ChallengeUI : MonoBehaviour
{
    [SerializeField] TMP_Text speechText;
    [SerializeField] Button homeButton, verifyButton;
    [SerializeField] Transform robotTransform;
    [SerializeField] Animator robotAnimator;

    private AutomataController automataController;

    [SerializeField] GameObject errorContainer;
    private TMP_Text errorText;

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip correctClip;
    [SerializeField] AudioClip incorrectClip;

    [SerializeField] FadeOverlay fadeOverlay;

    private Challenge challenge;

    private void Start()
    {
        automataController = FindObjectOfType<AutomataController>();

        homeButton.onClick.AddListener(GoMainMenu);
        verifyButton.onClick.AddListener(VerifyClicked);

        homeButton.transform.localScale = Vector3.zero;
        verifyButton.transform.localScale = Vector3.zero;
        errorContainer.transform.localScale = Vector3.zero;

        errorText = errorContainer.GetComponentInChildren<TMP_Text>();

        // Show first UI
        transform.localScale = new Vector3(1,0,1);

        challenge = Challenges.GetCurrentChallenge();
        speechText.text = "<align=center><size=150%><b>Challenge</b></size></align>\n\nConstruct a DFA that accepts the language of:<color=#00FF00>\n\n" + challenge.description + "</color>\nInput alphabet, \u03A3 = " + challenge.alphabet;

        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(1f);
        seq.Append(transform.DOScaleY(1f, 0.5f).SetEase(Ease.OutBack));
        seq.Append(verifyButton.transform.DOScale(1f, 0.5f));
        seq.Insert(1.5f, homeButton.transform.DOScale(1f, 0.5f));
    }

    private void VerifyClicked()
    {
        StartCoroutine(VerifyAutomaton());
    }

    private void GoMainMenu()
    {
        fadeOverlay.FadeToBlack().OnComplete(() => SceneManager.LoadScene("Menu"));
    }

    private IEnumerator ChangeText(string textToDisplay)
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOScaleY(0f, 0.5f).OnComplete(() => speechText.text = textToDisplay));
        //seq.AppendInterval(1f);
        seq.Append(transform.DOScaleY(1f, 0.5f).SetEase(Ease.OutBack));
        yield return seq.WaitForCompletion();
    }

    public void HideUI()
    {
        transform.DOScaleY(0f, 0.5f);
    }

    public void ShowUI(string textToDisplay = "")
    {
        if (textToDisplay != "")
            speechText.text = textToDisplay;
        transform.DOScaleY(1f, 0.5f).SetEase(Ease.OutBack);
    }

    private IEnumerator RobotJump()
    {
        robotAnimator.SetTrigger("Love");
        Sequence seq = DOTween.Sequence();
        seq.Append(robotTransform.DOLocalRotate(new Vector3(0, 360, 0), 2f, RotateMode.FastBeyond360).SetRelative(true).SetEase(Ease.InOutBack));
        seq.Insert(0.5f, robotTransform.DOJump(robotTransform.position, 0.5f, 1, 1f));
        yield return seq.WaitForCompletion();
        robotAnimator.SetTrigger("Base");
    }

    private IEnumerator VerifyAutomaton()
    {
        robotAnimator.SetTrigger("Compute");

        verifyButton.transform.DOScale(0f, 0.5f);
        homeButton.transform.DOScale(0f, 0.5f);
        errorContainer.transform.DOScale(0f, 0.5f);

        int numStates = automataController.GetNumStates();

        yield return new WaitForSeconds(2f);
        
        (bool, string) result = automataController.CompareAutomata(challenge.automaton);
        if (result.Item1)
        {
            challenge.SetCompleted();
            audioSource.clip = correctClip;
            audioSource.Play();
            robotAnimator.SetTrigger("Love");

            string text = "";
            if (numStates <= challenge.minStates)
            {
                challenge.SetMinimal();
                text = "<align=center><size=150%><b>Well done!</b></size></align>\n\n" +
                "<color=#00FF00>You completed the challenge using the minimum number of states!</color>\n\n" +
                "Click the Home button below to return to the main menu and attempt a different challenge, or create your own automata in Sandbox mode!";
                yield return StartCoroutine(ChangeText(text));
                homeButton.transform.localPosition = new Vector3(0f, -38f, 0f);
            }
            else
            {
                text = "<align=center><size=150%><b>Well done!</b></size></align>\n\n" +
                "You completed the challenge...\n<color=#00E7FF>but your solution does not use the minimal number of states!</color>\n\n" +
                "You can attempt to optimise your solution to use only <color=#00E7FF>" + challenge.minStates + " states</color>, or click the Home button to return to the main menu.";
                yield return StartCoroutine(ChangeText(text));
                verifyButton.transform.DOScale(1f, 0.5f);
            }
            homeButton.transform.DOScale(1f, 0.5f);
        }
        else
        {
            audioSource.clip = incorrectClip;
            audioSource.Play();
            errorText.text = result.Item2;
            errorContainer.transform.localScale = Vector3.zero;
            errorContainer.transform.DOScale(1f, 0.5f);
            robotAnimator.SetTrigger("Error");
            yield return new WaitForSeconds(3f);
            robotAnimator.SetTrigger("Base");
            verifyButton.transform.DOScale(1f, 0.5f);
            homeButton.transform.DOScale(1f, 0.5f);
        }        
    }
}
