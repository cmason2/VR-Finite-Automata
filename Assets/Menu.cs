using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Menu : MonoBehaviour
{

    public Button testButton;
    public Button stepButton;
    public TMP_InputField wordInputField;
    public TMP_Text outputText;

    [SerializeField] Color32 validColor;
    [SerializeField] Color32 invalidColor;

    AutomataController automataController;
    // Start is called before the first frame update
    void Start()
    {
        automataController = FindObjectOfType<AutomataController>();

        testButton.onClick.AddListener(CheckWord);
        stepButton.onClick.AddListener(StepClicked);
    }

    void CheckWord()
    {
        var result = automataController.CheckInputWord(wordInputField.text);
        if (result.Item1)
        {
            outputText.color = validColor;
            outputText.text = "Accepted";
        }
        else
        {
            outputText.color = invalidColor;
            outputText.text = "NOT accepted\n" + result.Item2;
        }
    }

    void StepClicked()
    {
        var comparisonResult = automataController.CompareAutomata(automataController.ExampleAutomata());
        if (comparisonResult.Item1)
        {
            outputText.color = validColor;
            outputText.text = comparisonResult.Item2;
        }
        else
        {
            outputText.color = invalidColor;
            outputText.text = "NOT equivalent\n" + comparisonResult.Item2;
        }
    }
}
