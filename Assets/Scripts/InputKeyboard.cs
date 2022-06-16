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
        wordInputField.ActivateInputField();
        int caretPosition = wordInputField.caretPosition;
        Debug.Log(caretPosition);
        wordInputField.text = wordInputField.text.Insert(caretPosition, button.GetComponentInChildren<TMP_Text>().text);
        caretPosition++;
        StartCoroutine(UpdateCaretPos(caretPosition));
    }

    private void SubmitClicked()
    {
        gameObject.SetActive(false);
    }

    private void BackspaceClicked()
    {
        wordInputField.ActivateInputField();
        int caretPosition = wordInputField.caretPosition;
        if(caretPosition > 0)
            wordInputField.text = wordInputField.text.Remove(caretPosition - 1, 1);
        else if (caretPosition == 0 && wordInputField.text.Length > 0)
            wordInputField.text = wordInputField.text.Remove(0, 1);

        caretPosition--;
        StartCoroutine(UpdateCaretPos(caretPosition));
    }

    IEnumerator UpdateCaretPos(int caretPos)
    {
        int width = wordInputField.caretWidth;
        wordInputField.caretWidth = 0;

        yield return new WaitForEndOfFrame();
        wordInputField.caretPosition = caretPos;
        wordInputField.caretWidth = width;
    }
}
