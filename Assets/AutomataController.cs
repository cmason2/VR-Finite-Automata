using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomataController : MonoBehaviour
{

    [SerializeField] int lastStateID = -1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Returns an unused identifier for the creation of a new state
    public int GetNextStateID()
    {
        lastStateID++;
        return lastStateID;
    }
}
