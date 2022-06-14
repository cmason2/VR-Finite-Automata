using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Menu : MonoBehaviour
{

    public Button testButton;
    public TMP_InputField wordInputField;
    public TMP_Text outputText;

    AutomataController automataController;
    // Start is called before the first frame update
    void Start()
    {
        automataController = FindObjectOfType<AutomataController>();

        testButton.onClick.AddListener(CheckWord);
    }

    void CheckWord()
    {
        bool result = automataController.CheckInputWord(wordInputField.text);
        if (result)
        {
            outputText.text = "Accepted";
        }
        else
        {
            outputText.text = "NOT accepted";
        }
    }
}
