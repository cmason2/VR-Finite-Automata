using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using System;
using DG.Tweening;

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
    private List<char> symbolsList;

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

        symbolsList = new List<char>();

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

        char symbol = toggle.GetComponentInChildren<TMP_Text>().text[0];
        if (toggle.isOn)
        {
            symbolsList.Add(symbol);
        }
        else
        {
            symbolsList.Remove(symbol);
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
            errorPanel.transform.localScale = Vector3.zero;
            errorPanel.transform.DOKill();
            errorPanel.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
            errorPanel.transform.DOScale(Vector3.zero, 0.5f).SetDelay(5f);
            Debug.Log("Symbol used in another transition from this state");
        }
    }

    private void CancelClicked()
    {
        cancelled = true;
    }

    public void SetStateAndEdge(bool isNewEdge, State s, Bezier e)
    {
        state = s;
        edge = e;

        if (!isNewEdge)
        {
            List<char>existingSymbolsList = new List<char>(edge.GetSymbolList());
            Debug.Log(string.Join(",", existingSymbolsList));
            symbolsText.text = edge.GetSymbolText();
            InitialiseToggles();

            void InitialiseToggles()
            {
                if (existingSymbolsList.Contains('a'))
                    button1.isOn = true;
                if (existingSymbolsList.Contains('b'))
                    button2.isOn = true;
                if (existingSymbolsList.Contains('c'))
                    button3.isOn = true;
                if (existingSymbolsList.Contains('d'))
                    button4.isOn = true;
            }
        }
    }

    public void ResetKeyboard()
    {
        Debug.Log("Keyboard reset");
        cancelled = false;
        valid = false;
        button1.isOn = false;
        button2.isOn = false;
        button3.isOn = false;
        button4.isOn = false;
        symbolsList = new List<char>();
    }
}
