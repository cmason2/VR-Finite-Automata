using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.InputSystem;
using System;

public class RobotUI : MonoBehaviour
{
    [SerializeField] TMP_Text speechText;

    [SerializeField] Button homeButton, startButton;
    [SerializeField] Transform robotTransform;

    [SerializeField] Animator robotAnimator;

    private AutomataController automataController;

    [SerializeField] InputActionReference leftGrab;
    [SerializeField] InputActionReference rightGrab;
    [SerializeField] InputActionReference rightCreateEdge;
    [SerializeField] InputActionReference leftCreateEdge;
    [SerializeField] InputActionReference createState;
    [SerializeField] InputActionReference toggleMenu;
    [SerializeField] InputActionReference editAction;
    private string triggered = "";

    [SerializeField] GameObject sphereVolume;
    [SerializeField] TutorialTriggerVolume sphereVolumeScript;

    private void Start()
    {
        automataController = FindObjectOfType<AutomataController>();
        //automataController.RestrictInterations("TutorialStart");

        leftGrab.action.Disable();
        rightGrab.action.Disable();
        rightCreateEdge.action.Disable();
        leftCreateEdge.action.Disable();
        createState.action.Disable();
        toggleMenu.action.Disable();
        editAction.action.Disable();

        createState.action.canceled += StateCreated;

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

    private void StateCreated(InputAction.CallbackContext context)
    {
        triggered = "StateCreated";
    }

    private IEnumerator ChangeText(string textToDisplay)
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOScaleY(0f, 0.5f).OnComplete(() => speechText.text = textToDisplay));
        seq.AppendInterval(1f);
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

    private void GoMainMenu()
    {

    }

    private void StartTutorial()
    {
        StartCoroutine(Tutorial());
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

    private IEnumerator Tutorial()
    {
        robotAnimator.SetTrigger("Love");

        Sequence seq = DOTween.Sequence().OnComplete(() => robotAnimator.SetTrigger("Base"));

        seq.Append(robotTransform.DOLocalRotate(new Vector3(0, 360, 0), 2f, RotateMode.FastBeyond360).SetRelative(true).SetEase(Ease.InOutBack));
        seq.Join(startButton.transform.DOScale(0f, 0.5f));
        seq.Join(homeButton.transform.DOScale(0f, 0.5f));
        seq.Insert(0.5f, robotTransform.DOJump(robotTransform.position, 0.5f, 1, 1f));
        seq.Insert(0.5f, transform.DOScaleY(0f, 0.5f));
        seq.AppendInterval(1f);

        yield return seq.WaitForCompletion();
        speechText.text = 
            "<size=150%><b><u>Creating States</u></b></size>\n\n" +
            "Hold down the \"A\" button on your right controller to create a state and release the button to place it.";
        
        Tween showText = transform.DOScaleY(1f, 0.5f).SetEase(Ease.OutBack);
        yield return showText.WaitForCompletion();

        createState.action.Enable();
        while(triggered != "StateCreated")
        {
            yield return null;
        }
        triggered = "";
        createState.action.Disable();

        StartCoroutine(RobotJump());

        string text = "<size=150%><b><u>Moving States</u></b></size>\n\n" +
            "Point at a state you wish to move and squeeze the \"Grip\" button on the side of the controller to grab the state.\n\n" +
            "Once grabbed, you can use the controller joystick to move the state towards or away from you.\n\n" +
            "Move the state into the green box to continue.";
        yield return StartCoroutine(ChangeText(text));

        leftGrab.action.Enable();
        rightGrab.action.Enable();

        while(!sphereVolumeScript.stateInside)
        {
            yield return null;
        }

        Tween hideSphere = sphereVolume.transform.DOScale(0f, 0.5f).SetEase(Ease.OutBack);
        yield return showText.WaitForCompletion();

        text = "<size=150%><b><u>Creating Edges</u></b></size>\n\n" +
            "To create an edge between states, point at the state you want the edge to start from and hold down one of the controller trigger buttons\n\n" +
            "Next, point at the second state where you want edge to finish and release the trigger button.\n\n";
        yield return StartCoroutine(ChangeText(text));

        leftCreateEdge.action.Enable();
        rightCreateEdge.action.Enable();

        Debug.Log("Tutorial Coroutine Finished");
        
    }
}
