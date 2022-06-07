using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class State : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] AudioClip audioClip;
    
    [SerializeField] int stateID;
    [SerializeField] bool isStartState = false;
    [SerializeField] bool isFinalState = false;
    [SerializeField] List<Bezier> edges;

    [SerializeField] TMP_Text stateText;

    private AutomataController automataController;

    [SerializeField] Material startMaterial;
    [SerializeField] Material normalMaterial;
    [SerializeField] Material finalMaterial;
    [SerializeField] MeshRenderer stateRenderer;

    private void Start()
    {
        automataController = FindObjectOfType<AutomataController>();
        audioSource = FindObjectOfType<AudioSource>();
        edges = new List<Bezier>();
        SetMaterial();
    }

    //private void OnDestroy()
    //{
    //    automataController.DeleteState(this); // Remove this state and any transitions containing this state
    //    edges.ForEach(e => Destroy(e.transform.root.gameObject)); // Destroy every edge connected to this state
    //}

    public void SetStateID(int id)
    {
        stateID = id;
        stateText.SetText(id.ToString());
    }

    public int GetStateID()
    {
        return stateID;
    }

    public List<Bezier> GetEdges()
    {
        return edges;
    }

    public void SetInitialState(bool b)
    {
        if (b)
        {
            isStartState = true;
            stateRenderer.material = startMaterial;
        }
        else
        {
            isStartState = false;
            stateRenderer.material = normalMaterial;
        }
    }

    public bool IsStartState()
    {
        return isStartState;
    }

    public void SetFinalState(bool b)
    {
        if (b)
        {
            isFinalState = true;
            stateRenderer.material = finalMaterial;
        }
        else
        {
            isFinalState = false;
            stateRenderer.material = normalMaterial;
        }
    }

    public bool IsFinalState()
    {
        return isFinalState;
    }

    public void OnHoverEntered()
    {
        Color color = stateRenderer.material.color;
        color.a = 0.6f;
        stateRenderer.material.color = color;
    }

    public void OnHoverExited()
    {
        SetMaterial();
    }

    public void OnActivated()
    {
        Debug.Log("In OnActivated() in state");
        audioSource.clip = audioClip;
        audioSource.Play();

        automataController.DeleteState(this); // Remove this state and any transitions containing this state
        edges.ForEach(e => Destroy(e.transform.root.gameObject)); // Destroy every edge connected to this state

        Destroy(gameObject);
    }

    public void SetMaterial()
    {
        if (isStartState)
            stateRenderer.material = startMaterial;
        else if (isFinalState)
            stateRenderer.material = finalMaterial;
        else
            stateRenderer.material = normalMaterial;
    }

    public void AddEdge(Bezier edge)
    {
        edges.Add(edge);
    }

    public void DeleteEdge(Bezier edge)
    {
        edges.Remove(edge);
    }
}
