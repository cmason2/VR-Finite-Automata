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

    [SerializeField] Button homeButton, startButton, continueButton;
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
    private bool nextStep = false;

    [SerializeField] GameObject sphereVolume;
    private MeshRenderer sphereRenderer;
    [SerializeField] TutorialTriggerVolume sphereVolumeScript;

    private State state1;
    private State state2;

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
        leftCreateEdge.action.canceled += EdgeCreated;
        rightCreateEdge.action.canceled += EdgeCreated;

        homeButton.onClick.AddListener(GoMainMenu);
        startButton.onClick.AddListener(StartTutorial);
        continueButton.onClick.AddListener(ContinueClicked);

        homeButton.transform.localScale = Vector3.zero;
        startButton.transform.localScale = Vector3.zero;

        sphereVolume.SetActive(false);
        sphereRenderer = sphereVolume.GetComponent<MeshRenderer>();
        Color transparentSphere = sphereRenderer.material.color;
        transparentSphere.a = 0;
        sphereRenderer.material.color = transparentSphere;

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

    private void EdgeCreated(InputAction.CallbackContext context)
    {
        triggered = "EdgeCreated";
    }

    private void ContinueClicked()
    {
        nextStep = true;
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

        yield return seq.WaitForCompletion();
        
        
        // Create a state
        speechText.text = 
            "<size=150%><b>Creating States</b></size>\n\n" +
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

        state1 = automataController.GetStateByIndex(0);

        
        
        
        
        // Move the state into the volume
        string text = "<size=150%><b>Moving States</b></size>\n\n" +
            "Point at a state you wish to move and squeeze the \"Grip\" button on the side of the controller to grab the state.\n\n" +
            "Once grabbed, you can use the controller joystick to move the state towards or away from you.\n\n" +
            "Move the state inside the blue sphere to continue.";
        yield return StartCoroutine(ChangeText(text));

        leftGrab.action.Enable();
        rightGrab.action.Enable();

        sphereVolume.SetActive(true);
        Tween showSphere = sphereRenderer.material.DOColor(new Color32(0, 0, 0, 10), 0.5f).SetOptions(true);
        yield return showSphere.WaitForCompletion();

        while (!sphereVolumeScript.stateInside)
        {
            yield return null;
        }

        Tween hideSphere = sphereRenderer.material.DOColor(new Color32(0,0,0,0), 0.5f).SetOptions(true);
        yield return showText.WaitForCompletion();

        StartCoroutine(RobotJump());

        
        
        
        // Create Second State
        text = "<size=150%><b>Creating Edges</b></size>\n\n" +
            "Next we are going to create an edge between two states, but to do that we'll need another state!\n\n" +
            "Create another state with the \"A\" button and move it inside the blue sphere with the \"Grip\" button to continue.";
        yield return StartCoroutine(ChangeText(text));

        sphereVolume.transform.position += new Vector3(1f, 0, 0);
        showSphere = sphereRenderer.material.DOColor(new Color32(0, 0, 0, 10), 0.5f).SetOptions(true);
        yield return showSphere.WaitForCompletion();

        createState.action.Enable();
        while (triggered != "StateCreated")
        {
            yield return null;
        }
        triggered = "";
        createState.action.Disable();

        state2 = automataController.GetStateByIndex(1);

        while (!sphereVolumeScript.stateInside)
        {
            yield return null;
        }

        hideSphere = sphereRenderer.material.DOColor(new Color32(0, 0, 0, 0), 0.5f).SetOptions(true);
        yield return showText.WaitForCompletion();

        sphereVolume.SetActive(false);

        StartCoroutine(RobotJump());

        
        
        
        // Create a normal edge between the two states
        text = "<size=150%><b>Creating Edges</b></size>\n\n" +
            "To create an edge between states, point at the state you want the edge to start from and hold down one of the controller trigger buttons.\n\n" +
            "Next, point at the second state where you want edge to finish and release the trigger button.\n\n" +
            "Try creating an edge between the two states you created.";
        yield return StartCoroutine(ChangeText(text));

        leftCreateEdge.action.Enable();
        rightCreateEdge.action.Enable();

        while (GameObject.Find("Edge(Clone)") == null)
        {
            yield return null;
        }

        leftCreateEdge.action.Disable();
        rightCreateEdge.action.Disable();

        text = "<size=150%><b>Selecting Symbols</b></size>\n\n" +
            "Once the edge has been created, a keypad will appear on your left controller that will allow you to set the symbols for the edge.\n\n" +
            "Use the right controller's trigger button to select the desired symbols and confirm your choice by clicking the green button on the keypad";
        yield return StartCoroutine(ChangeText(text));

        while (state1.GetEdges().Count < 1 && state2.GetEdges().Count < 1)
        {
            yield return null;
        }

        StartCoroutine(RobotJump());

        
        
        
        // Create a loop edge
        text = "<size=150%><b>Creating Loop Edges</b></size>\n\n" +
            "You can also create edges that begin and end with the same state to create a loop.\n\n" +
            "To do this, simply point at the desired state and press and release the trigger button.\n\n" +
            "The keypad will appear on your left controller as before to enter a symbol for the edge.";
        yield return StartCoroutine(ChangeText(text));

        leftCreateEdge.action.Enable();
        rightCreateEdge.action.Enable();

        while (triggered != "EdgeCreated")
        {
            yield return null;
        }
        triggered = "";

        while (state1.GetEdges().Count == state2.GetEdges().Count)
        {
            yield return null;
        }

        StartCoroutine(RobotJump());
        
        
        
        
        // Move edges
        text = "<size=150%><b>Positioning Edges</b></size>\n\n" +
            "Edges can be repositioned by pointing at the edge's symbols and squeezing the \"Grip\" button.\n\n" +
            "In the same way as moving states, the joystick can be used to move the edge towards or away from you.\n\n" +
            "Click the button below to continue.";
        yield return StartCoroutine(ChangeText(text));

        continueButton.gameObject.SetActive(true);
        continueButton.transform.DOScale(1f, 0.5f);

        while (!nextStep)
        {
            yield return null;
        }
        nextStep = false;

        Debug.Log("Tutorial Coroutine Finished");
    }
}
