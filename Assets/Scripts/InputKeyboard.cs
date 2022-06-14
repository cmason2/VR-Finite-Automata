using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InputKeyboard : MonoBehaviour
{

    public TMP_InputField wordInputField;
    public Button button1, button2, button3, button4, backspaceButton, submitButton;
    // Start is called before the first frame update
    void Start()
    {
        button1.onClick.AddListener(delegate { SymbolClicked(button1); });
        button2.onClick.AddListener(delegate { SymbolClicked(button2); });
        button3.onClick.AddListener(delegate { SymbolClicked(button3); });
        button4.onClick.AddListener(delegate { SymbolClicked(button4); });

        submitButton.onClick.AddListener(delegate { SubmitClicked(); });
        backspaceButton.onClick.AddListener(delegate { BackspaceClicked(); });
    }

    private void SymbolClicked(Button button)
    {
        wordInputField.text += button.GetComponentInChildren<TMP_Text>().text; 
    }

    private void SubmitClicked()
    {
        gameObject.SetActive(false);
    }

    private void BackspaceClicked()
    {
        if(wordInputField.text.Length > 0)
            wordInputField.text = wordInputField.text.Remove(wordInputField.text.Length - 1);
    }
}
