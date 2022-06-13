using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using System;

public class SymbolKeyboard : MonoBehaviour
{
    public Toggle button1, button2, button3, button4;
    public Button submitButton, cancelButton;

    private State state;
    private Bezier edge;

    private TMP_Text symbolsText;

    public string symbolsString;
    private List<string> symbolsList;

    public bool valid = false;
    public bool cancelled = false;

    private AudioSource audioSource;
    [SerializeField] AudioClip keypressClip;

    private AutomataController automataController;

    void Start()
    {
        automataController = FindObjectOfType<AutomataController>();
        audioSource = FindObjectOfType<AudioSource>();

        symbolsText = GetComponentInChildren<TMP_Text>();
        symbolsList = new List<string>();

        button1.onValueChanged.AddListener(delegate { ToggleClicked(button1); });
        button2.onValueChanged.AddListener(delegate { ToggleClicked(button2); });
        button3.onValueChanged.AddListener(delegate { ToggleClicked(button3); });
        button4.onValueChanged.AddListener(delegate { ToggleClicked(button4); });

        submitButton.onClick.AddListener(delegate { SubmitClicked(); });
        cancelButton.onClick.AddListener(delegate { CancelClicked(); });
    }

    private void ToggleClicked(Toggle toggle)
    {
        audioSource.clip = keypressClip;
        audioSource.Play();
        //Debug.Log(toggle.GetComponentInChildren<TMP_Text>().text + "\" toggled, now " + toggle.isOn);
        if (toggle.isOn)
        {
            symbolsList.Add(toggle.GetComponentInChildren<TMP_Text>().text);
        }
        else
        {
            symbolsList.Remove(toggle.GetComponentInChildren<TMP_Text>().text);
        }

        symbolsList.Sort();
        symbolsString = String.Join(",", symbolsList);
        symbolsText.text = symbolsString;

        if (symbolsList.Count > 0)
            submitButton.interactable = true;
        else
            submitButton.interactable = false;
    }

    private void SubmitClicked()
    {
        if (!automataController.IsSymbolUsed(state.GetStateID(), symbolsList))
        {
            valid = true;
        }
        else
        {
            Debug.Log("Symbol used in another transition from this state");
        }
    }

    private void CancelClicked()
    {
        cancelled = true;
    }

    public void SetStateAndEdge(State s, Bezier e)
    {
        state = s;
        edge = e;
    }
}
