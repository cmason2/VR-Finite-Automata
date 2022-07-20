using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{

    public Button testButton, stepButton, stopButton, nextButton, previousButton, homeButton, resetButton;
    public TMP_InputField wordInputField;
    public TMP_Text outputText;
    public GameObject keyboard;

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip successAudio;
    [SerializeField] AudioClip errorAudio;

    [SerializeField] Color32 validColor;
    [SerializeField] Color32 invalidColor;

    AutomataController automataController;
    // Start is called before the first frame update
    void Start()
    {
        automataController = FindObjectOfType<AutomataController>();

        testButton.onClick.AddListener(CheckWord);
        stepButton.onClick.AddListener(StepClicked);
        stopButton.onClick.AddListener(StopClicked);
        nextButton.onClick.AddListener(delegate { StepTriggered(true); });
        previousButton.onClick.AddListener(delegate { StepTriggered(false); });
        homeButton.onClick.AddListener(GoMainMenu);
        resetButton.onClick.AddListener(ResetAutomaton);
    }

    private void StepTriggered(bool next)
    {
        if (next)
        {
            automataController.stepStatus = 1;
        }
        else
        {
            automataController.stepStatus = -1;
        }
    }

    private void StopClicked()
    {
        automataController.stepStatus = -2;
        testButton.gameObject.SetActive(true);
        stepButton.gameObject.SetActive(true);
        stopButton.gameObject.SetActive(false);
        nextButton.gameObject.SetActive(false);
        previousButton.gameObject.SetActive(false);
        keyboard.SetActive(true);
        outputText.text = "";
    }

    void CheckWord()
    {
        string result = automataController.CheckInputWord(wordInputField.text);
        if (result == "<color=#32A852>Accepted</color>")
        {
            audioSource.clip = successAudio;
            outputText.color = validColor;
        }
        else
        {
            audioSource.clip = errorAudio;
            outputText.color = invalidColor;
            //outputText.text = result.Item2; --Verbose error of why it was rejected
        }

        outputText.text = result;
        audioSource.Play();
    }

    void StepClicked()
    {
        outputText.text = "";
        StartCoroutine(automataController.StepThroughInput(wordInputField.text));
        keyboard.SetActive(false);
        testButton.gameObject.SetActive(false);
        stepButton.gameObject.SetActive(false);
        stopButton.gameObject.SetActive(true);
        nextButton.gameObject.SetActive(true);
        previousButton.gameObject.SetActive(true);
    }

    void GoMainMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    void ResetAutomaton()
    {
        automataController.DeleteAllStates();
    }
}
