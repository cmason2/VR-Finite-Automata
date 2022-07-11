using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class Menu : MonoBehaviour
{

    public Button testButton, stepButton, stopButton, nextButton, previousButton, equivButton;
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
        equivButton.onClick.AddListener(CompareClicked);
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
        var result = automataController.CheckInputWord(wordInputField.text);
        if (result.Item1)
        {
            audioSource.clip = successAudio;
            outputText.color = validColor;
            outputText.text = "Accepted";
        }
        else
        {
            audioSource.clip = errorAudio;
            outputText.color = invalidColor;
            outputText.text = "Rejected";
            //outputText.text = result.Item2; --Verbose error of why it was rejected
        }
        audioSource.Play();
    }

    void StepClicked()
    {
        StartCoroutine(automataController.StepThroughInput(wordInputField.text));
        keyboard.SetActive(false);
        testButton.gameObject.SetActive(false);
        stepButton.gameObject.SetActive(false);
        stopButton.gameObject.SetActive(true);
        nextButton.gameObject.SetActive(true);
        previousButton.gameObject.SetActive(true);
    }

    void CompareClicked()
    {
        var comparisonResult = automataController.CompareAutomata(automataController.ExampleAutomata());
        if (comparisonResult.Item1)
        {
            outputText.color = validColor;
            outputText.text = "Automaton recognises the language containing the single word \"ab\"";
        }
        else
        {
            outputText.color = invalidColor;
            outputText.text = "NOT equivalent\n" + comparisonResult.Item2;
        }
    }
}
