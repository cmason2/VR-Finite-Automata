using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State : MonoBehaviour
{
    
    [SerializeField] int stateID;

    public void SetStateID(int id)
    {
        stateID = id;
    }

    public int GetStateID()
    {
        return stateID;
    }
}
