using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class State : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] AudioClip audioClip;
    
    [SerializeField] int stateID;
    [SerializeField] bool isStartState = false;
    [SerializeField] bool isFinalState = false;
    [SerializeField] List<Bezier> edges;

    private AutomataController automataController;

    [SerializeField] Material[] stateMaterials;
    [SerializeField] Material[] cloudMaterials;
    [SerializeField] MeshRenderer stateRenderer;
    [SerializeField] MeshRenderer cloudRenderer;
    private MeshRenderer moonRenderer;
    private int stateType = -1;

    [SerializeField] GameObject clouds;
    [SerializeField] GameObject moon;

    public bool isGrabbed = false;

    private void Start()
    {
        automataController = FindObjectOfType<AutomataController>();
        audioSource = FindObjectOfType<AudioSource>();
        edges = new List<Bezier>();
        moonRenderer = moon.GetComponent<MeshRenderer>();
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

    public void SetStateType(int type)
    {
        stateType = type;
    }

    public int GetStateType()
    {
        return stateType;
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

    public void HideEdges()
    {
        foreach (Bezier edge in edges)
        {
            edge.HideEdge();
        }
    }

    public void ShowEdges()
    {
        foreach (Bezier edge in edges)
        {
            edge.ShowEdge();
        }
    }

    public void MoveAttachedEdges(Vector3 moveVector)
    {
        foreach (Bezier edge in edges)
        {
            if (!edge.IsLoop())
            {
                edge.transform.parent.position += moveVector; 
            }
        }
    }

    public void DeleteState()
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
        transform.DOScale(Vector3.zero, 0.3f).OnComplete(() => Destroy(gameObject));
    }

    public void SetMaterial()
    {
        if (stateType != -1)
        {
            stateRenderer.material = stateMaterials[stateType];
            if (cloudMaterials[stateType] != null)
            {
                cloudRenderer.material = cloudMaterials[stateType];
                clouds.SetActive(true);
            }
            else
            {
                clouds.SetActive(false);
            }
        }
        
        if (isFinalState)
        {
            moon.SetActive(true);
        }
        else
        {
            moon.SetActive(false);
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

    public void DisableRenderers()
    {
        stateRenderer.enabled = false;
        cloudRenderer.enabled = false;
        moonRenderer.enabled = false;
    }

    public void EnableRenderers()
    {
        stateRenderer.enabled = true;
        cloudRenderer.enabled = true;
        moonRenderer.enabled = true;
    }
}
