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

    private AutomataController automataController;

    [SerializeField] Material startMaterial; //Earth
    [SerializeField] Material normalMaterial;
    [SerializeField] Material finalMaterial; //Sun
    [SerializeField] MeshRenderer stateRenderer;

    private Transform clouds;
    private Transform moon;

    private void Start()
    {
        automataController = FindObjectOfType<AutomataController>();
        audioSource = FindObjectOfType<AudioSource>();
        clouds = this.transform.Find("Clouds");
        moon = this.transform.Find("Moon");
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
        isStartState = b ? true : false;
        SetMaterial();
    }

    public bool IsStartState()
    {
        return isStartState;
    }

    public void SetFinalState(bool b)
    {
        isFinalState = b ? true : false;
        SetMaterial();
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

    public void DisableCurves()
    {
        foreach (Bezier edge in edges)
        {
            edge.numGrabs--;
            if(edge.numGrabs <= 0)
                edge.enabled = false;
        }
    }

    public void EnableCurves()
    {
        foreach (Bezier edge in edges)
        {
            edge.enabled = true;
            edge.numGrabs++;
        }
    }

    public void OnActivated()
    {
        audioSource.clip = audioClip;
        audioSource.Play();

        automataController.DeleteState(this); // Remove this state and any transitions containing this state
        
        // Unparent any edges (loops)
        foreach (Bezier edge in edges)
        {
            if (edge.transform.parent.parent != null)
                edge.transform.parent.parent = null;
            Destroy(edge.transform.root.gameObject); // Destroy every edge connected to this state
        }

        Destroy(gameObject);
    }

    public void SetMaterial()
    {
        if (isStartState)
        {
            stateRenderer.material = startMaterial;
        }
        else if (isFinalState)
        {
            stateRenderer.material = finalMaterial;
            if (clouds != null)
            {
                clouds.gameObject.SetActive(false);
            } 
        }
        else
        {
            stateRenderer.material = normalMaterial;
            if (clouds != null)
                clouds.gameObject.SetActive(true);
        }
    }

    public void SetColour(Color colour)
    {
        stateRenderer.material.color = colour;
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
