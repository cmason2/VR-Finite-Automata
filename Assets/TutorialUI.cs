using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.InputSystem;
using System;
using UnityEngine.SceneManagement;

public class TutorialUI : MonoBehaviour
{
    [SerializeField] TMP_Text speechText;
    [SerializeField] Button homeButton, startButton, continueButton, verifyButton;
    [SerializeField] GameObject stateSelector;
    [SerializeField] Transform robotTransform;
    [SerializeField] Animator robotAnimator;
    [SerializeField] FadeOverlay fadeOverlay;

    private AutomataController automataController;

    [SerializeField] InputActionReference leftGrab;
    [SerializeField] InputActionReference rightGrab;
    [SerializeField] InputActionReference rightCreateEdge;
    [SerializeField] InputActionReference leftCreateEdge;
    [SerializeField] InputActionReference rightCreateState;
    [SerializeField] InputActionReference leftCreateState;
    [SerializeField] InputActionReference toggleMenu;
    [SerializeField] InputActionReference rightEditAction;
    [SerializeField] InputActionReference leftEditAction;
    private string triggered = "";
    private bool nextStep = false;

    [SerializeField] GameObject sphereVolume1;
    private MeshRenderer sphereRenderer1;
    [SerializeField] TutorialTriggerVolume sphereVolumeScript1;

    [SerializeField] GameObject sphereVolume2;
    private MeshRenderer sphereRenderer2;
    [SerializeField] TutorialTriggerVolume sphereVolumeScript2;

    [SerializeField] GameObject errorContainer;
    private TMP_Text errorText;

    private State state1;
    private State state2;

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip correctClip;
    [SerializeField] AudioClip incorrectClip;

    private void Start()
    {
        automataController = FindObjectOfType<AutomataController>();
        //automataController.RestrictInterations("TutorialStart");

        leftGrab.action.Disable();
        rightGrab.action.Disable();
        rightCreateEdge.action.Disable();
        leftCreateEdge.action.Disable();
        rightCreateState.action.Disable();
        leftCreateState.action.Disable();
        toggleMenu.action.Disable();
        rightEditAction.action.Disable();
        leftEditAction.action.Disable();

        rightCreateState.action.canceled += StateCreated;
        leftCreateState.action.canceled += StateCreated;
        rightCreateEdge.action.canceled += EdgeCreated;
        leftCreateEdge.action.canceled += EdgeCreated;

        homeButton.onClick.AddListener(GoMainMenu);
        startButton.onClick.AddListener(StartTutorial);
        continueButton.onClick.AddListener(ContinueClicked);
        verifyButton.onClick.AddListener(VerifyClicked);

        homeButton.transform.localScale = Vector3.zero;
        startButton.transform.localScale = Vector3.zero;
        verifyButton.transform.localScale = Vector3.zero;
        errorContainer.transform.localScale = Vector3.zero;

        errorText = errorContainer.GetComponentInChildren<TMP_Text>();

        sphereVolume1.SetActive(false);
        sphereRenderer1 = sphereVolume1.GetComponent<MeshRenderer>();
        Color transparentSphere = sphereRenderer1.material.color;
        transparentSphere.a = 0;
        sphereRenderer1.material.color = transparentSphere;

        sphereVolume2.SetActive(false);
        sphereRenderer2 = sphereVolume2.GetComponent<MeshRenderer>();
        sphereRenderer2.material.color = transparentSphere;

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

    private void VerifyClicked()
    {
        triggered = "Verify";
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
            "Hold down the plus button <size=150%><sprite=0></size> on either controller to create a state and release the button to place it.";

        Tween showText = transform.DOScaleY(1f, 0.5f).SetEase(Ease.OutBack);
        yield return showText.WaitForCompletion();

        rightCreateState.action.Enable();
        leftCreateState.action.Enable();
        
        while (triggered != "StateCreated")
        {
            yield return null;
        }
        triggered = "";
        rightCreateState.action.Disable();
        leftCreateState.action.Disable();

        state1 = automataController.GetStateByIndex(0);





        // Move the state into the volume
        string text = "<size=150%><b>Moving States</b></size>\n\n" +
            "Point at a state you wish to move and squeeze the \"Grip\" button on the side of the controller to grab the state.\n\n" +
            "Once grabbed, you can use the controller joystick to move the state towards or away from you.\n\n" +
            "Move the state inside the <color=#00E7FF>blue sphere</color> to continue.";
        yield return StartCoroutine(ChangeText(text));

        leftGrab.action.Enable();
        rightGrab.action.Enable();

        sphereVolume1.SetActive(true);
        Tween showSphere1 = sphereRenderer1.material.DOColor(new Color32(0, 0, 0, 10), 0.5f).SetOptions(true);
        yield return showSphere1.WaitForCompletion();

        while (!sphereVolumeScript1.stateInside)
        {
            yield return null;
        }

        Tween hideSphere1 = sphereRenderer1.material.DOColor(new Color32(0, 0, 0, 0), 0.5f).SetOptions(true).OnComplete(() => sphereVolume1.SetActive(false));

        yield return showText.WaitForCompletion();

        StartCoroutine(RobotJump());




        // Change state Type
        text = "<size=150%><b>Editing States</b></size>\n\n" +
            "You can also edit the properties of states, such as whether they are the initial state or an accepting state.\n\n" +
            "To bring up the edit menu, point at the state you wish to modify and hold down the edit button <size=150%><sprite=1></size> on the controller.";
        yield return StartCoroutine(ChangeText(text));

        rightEditAction.action.Enable();
        leftEditAction.action.Enable();

        while (!stateSelector.activeInHierarchy)
        {
            yield return null;
        }

        text = "To select an option, hover over it and release the edit <size=150%><sprite=1></size> button.\n\n" +
           "Try changing the appearence of the state to the <color=#E8642b>volcanic planet</color> at the top of the edit wheel.";
        yield return StartCoroutine(ChangeText(text));


        while (!(state1.GetStateType() == 3 && !stateSelector.activeInHierarchy))
        {
            yield return null;
        }

        text = "<size=150%><b>Edit Menu</b></size>\n\n" +
           "From the edit menu you can:\n\n" +
           "<color=#FFFFFF>Change the state's appearence</color>\n" +
           "<color=#00FF00>Set the initial state (bottom left)</color>\n" +
           "<color=#00E7FF>Set an accepting state (bottom right)</color>\n" +
           "<color=#FF0000>Delete a state (bottom middle)</color>\n\n" +
           "Try <color=#FF0000>deleting</color> the state to continue.";
        yield return StartCoroutine(ChangeText(text));

        while (state1 != null)
        {
            yield return null;
        }




        // Create Two states to setup edge creation
        text = "The first state you create will automatically be set as the <color=#00FF00>initial state (Planet Earth)</color>.\n\n" +
            "<color=#00E7FF>Accepting states</color> are indicated by planets that have orbiting moons.\n\n" +
            "To continue, create two more states and move them inside the two <color=#00E7FF>blue spheres</color>.";
        yield return StartCoroutine(ChangeText(text));

        sphereVolume1.SetActive(true);
        sphereVolume2.SetActive(true);

        sphereVolumeScript1.stateInside = false;

        seq = DOTween.Sequence();
        seq.Append(sphereRenderer1.material.DOColor(new Color32(0, 0, 0, 10), 0.5f).SetOptions(true));
        seq.Join(sphereRenderer2.material.DOColor(new Color32(0, 0, 0, 10), 0.5f).SetOptions(true));

        yield return seq.WaitForCompletion();

        rightCreateState.action.Enable();
        leftCreateState.action.Enable();

        while (!(sphereVolumeScript1.stateInside && sphereVolumeScript2.stateInside))
        {
            if (automataController.GetNumStates() < 2)
            {
                rightCreateState.action.Enable();
                leftCreateState.action.Enable();
            }
            else
            {
                rightCreateState.action.Disable();
                leftCreateState.action.Disable();
            }

            yield return null;
        }

        rightCreateState.action.Disable();
        leftCreateState.action.Disable();

        state1 = automataController.GetStateByIndex(0);
        state2 = automataController.GetStateByIndex(1);

        seq = DOTween.Sequence();
        seq.Append(sphereRenderer1.material.DOColor(new Color32(0, 0, 0, 0), 0.5f).SetOptions(true));
        seq.Join(sphereRenderer2.material.DOColor(new Color32(0, 0, 0, 0), 0.5f).SetOptions(true));
        seq.AppendCallback(() => sphereVolume1.SetActive(false));
        seq.AppendCallback(() => sphereVolume2.SetActive(false));
        

        
        
        
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

        continueButton.transform.localScale = Vector3.zero;
        continueButton.gameObject.SetActive(true);
        continueButton.transform.DOScale(1f, 0.5f);

        while (!nextStep)
        {
            yield return null;
        }
        nextStep = false;

        yield return continueButton.transform.DOScale(0f, 0.5f).WaitForCompletion();




        // Edit Edge
        text = "<size=150%><b>Editing Edges</b></size>\n\n" +
           "You can also delete edges or change their symbols by pointing at an edge's symbol and holding down the edit button <size=150%><sprite=1></size> on the right controller.\n\n" +
           "Have a go at <color=#FF0000>deleting</color> an edge or <color=#00E7FF>changing its symbols</color>, then click the continue button below to proceed.";
        yield return StartCoroutine(ChangeText(text));

        continueButton.gameObject.SetActive(true);
        continueButton.transform.DOScale(1f, 0.5f);

        while (!nextStep)
        {
            yield return null;
        }
        nextStep = false;

        yield return continueButton.transform.DOScale(0f, 0.5f).WaitForCompletion();



        // Menu
        // Add this at some point!



        // Create automaton that recognises language of words containing an even number of a's
        automataController.DeleteAllStates();

        text = "<size=150%><b>Challenge!</b></size>\n\n" +
           "Now that you have seen the basic controls, it's time to construct an automaton!\n\n" +
           "Try to construct an automaton that accepts the language of <color=#00E7FF>words containing an even number of 'a's</color> (alphabet contains only 'a')\n\n" +
           "When you have finished condstructing the automaton, click the button below and I'll check whether it's correct!";
        yield return StartCoroutine(ChangeText(text));

        rightCreateState.action.Enable();
        leftCreateState.action.Enable();

        verifyButton.transform.localScale = Vector3.zero;
        verifyButton.gameObject.SetActive(true);
        verifyButton.transform.DOScale(1f, 0.5f);

        bool correct = false;
        (bool, string) result;
        while (!correct)
        {
            while (triggered != "Verify")
            {
                yield return null;
            }
            triggered = "";
            verifyButton.transform.DOScale(0f, 0.5f);
            robotAnimator.SetTrigger("Compute");
            errorContainer.transform.DOScale(0f, 0.5f);

            yield return new WaitForSeconds(2f);
            result = automataController.CompareAutomata(automataController.TutorialAutomata());
            if (result.Item1)
            {
                audioSource.clip = correctClip;
                audioSource.Play();
                correct = true;
                robotAnimator.SetTrigger("Love");
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
            }
        }

        text = "<size=150%><b>Well done!</b></size>\n\n" +
            "You've successfully completed the tutorial!\n\n" +
            "Click the Home button below to return to the main menu where you can create your own Automata in Sandbox mode, or attempt a number of challenges!";
        yield return StartCoroutine(ChangeText(text));

        homeButton.transform.localPosition = new Vector3(0f, -38f, 0f);
        homeButton.transform.DOScale(1.0f, 0.5f);
    }
}
