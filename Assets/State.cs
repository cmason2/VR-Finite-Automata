using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State : MonoBehaviour
{
    
    [SerializeField] int stateID;
    [SerializeField] bool isStartState = false;
    [SerializeField] bool isFinalState = false;

    [SerializeField] Material startMaterial;
    [SerializeField] Material normalMaterial;
    [SerializeField] Material finalMaterial;

    [SerializeField] MeshRenderer stateRenderer;

    private void Start()
    {
        SetMaterial();
    }

    public void SetStateID(int id)
    {
        stateID = id;
    }

    public int GetStateID()
    {
        return stateID;
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

    public void HoverEntered()
    {
        Color color = stateRenderer.material.color;
        color.a = 0.6f;
        stateRenderer.material.color = color;
    }

    public void HoverExited()
    {
        SetMaterial();
    }

    public void SetMaterial()
    {
        if (isStartState)
            stateRenderer.material = startMaterial;
        else if (isFinalState)
            stateRenderer.material= finalMaterial;
        else
            stateRenderer.material= normalMaterial;
    }
}
