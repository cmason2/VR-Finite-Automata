using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomataController : MonoBehaviour
{

    [SerializeField] int numStates = 0;
    [SerializeField] int lastStateID = -1;
    private List<(int, string, int)> transitions;

    // Start is called before the first frame update
    void Start()
    {
        transitions = new List<(int, string, int)>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Returns an unused identifier for the creation of a new state
    public int GetNextStateID()
    {
        numStates++;
        lastStateID++;
        return lastStateID;
    }

    public int GetNumStates()
    {
        return numStates;
    }

    public void AddTransition(int s1, string symbol, int s2)
    {
        transitions.Add((s1, symbol, s2));
        Debug.Log("Transition added: " + transitions[transitions.Count - 1]);
    }
}
