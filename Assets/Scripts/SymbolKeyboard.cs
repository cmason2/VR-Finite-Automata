using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using System;

public class SymbolKeyboard : MonoBehaviour
{
    [SerializeField] Toggle button1, button2, button3, button4;
    [SerializeField] Button submitButton, cancelButton;
    [SerializeField] GameObject errorPanel;
    [SerializeField] TMP_Text errorText;

    private State state;
    private Bezier edge;

    [SerializeField] TMP_Text symbolsText;
    public string symbolsString;
    private List<string> symbolsList;

    public bool valid = false;
    public bool cancelled = false;

    private AudioSource audioSource;
    [SerializeField] AudioClip keypressClip;
    [SerializeField] AudioClip errorClip;

    private AutomataController automataController;

    void Start()
    {
        automataController = FindObjectOfType<AutomataController>();
        audioSource = FindObjectOfType<AudioSource>();

        errorPanel.transform.localScale = Vector3.zero;

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
        (bool, string) validityCheck = automataController.IsSymbolUsed(state, edge, symbolsString);
        if (!validityCheck.Item1)
        {
            valid = true;
        }
        else
        {
            errorText.text = validityCheck.Item2;
            audioSource.clip = errorClip;
            audioSource.Play();
            LeanTween.cancelAll();
            errorPanel.transform.localScale = Vector3.zero;
            LeanTween.scale(errorPanel, Vector3.one, 0.5f).setEase(LeanTweenType.easeOutBack);
            LeanTween.scale(errorPanel, Vector3.zero, 0.5f).setDelay(5f);
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

    public void ResetKeyboard()
    {
        cancelled = false;
        valid = false;
        button1.isOn = false;
        button2.isOn = false;
        button3.isOn = false;
        button4.isOn = false;
        symbolsList = new List<string>();
    }
}
