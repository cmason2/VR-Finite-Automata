using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomataController : MonoBehaviour
{

    [SerializeField] int numStates = 0;
    [SerializeField] int lastStateID = -1;
    private int startStateID;
    private List<string> alphabet;
    private Dictionary<int, (bool, bool)> states;
    private Dictionary<int, List<(int, string, int, State, GameObject, State)>> transitions;

    // Start is called before the first frame update
    void Start()
    {
        alphabet = new List<string>();
        states = new Dictionary<int, (bool, bool)>();
        transitions = new Dictionary<int, List<(int, string, int, State, GameObject, State)>>();
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

    public void AddState(int stateID, bool isStart, bool isFinal)
    {
        states.Add(stateID, (isStart, isFinal));
        transitions.Add(stateID, new List<(int, string, int, State, GameObject, State)>());
        Debug.Log("State added: " + stateID + states[stateID]);
    }

    public void UpdateState(int stateID, bool start, bool final)
    {
        states[stateID] = (start, final);
    }

    public void AddTransition(int state1ID, string symbol, int state2ID, State s1, GameObject edge, State s2)
    {
        string[] symbols = symbol.Split(',');
        foreach (string sym in symbols)
        {
            if (!alphabet.Contains(sym))
            {
                alphabet.Add(sym);
                Debug.Log("Symbol added: " + alphabet[alphabet.Count - 1]);
            }
        }
        
        transitions[state1ID].Add((state1ID, symbol, state2ID, s1, edge, s2));
        Debug.Log("Transition added: " + transitions[transitions.Count - 1]);
    }

    public string CheckAutomataValidity()
    {
        string result = "Valid";
        
        // Check if one start state and at least one final state
        if (states.Count != 0)
        {
            int numStartStates = 0;
            int numFinalStates = 0;
            foreach (var state in states)
            {
                Debug.Log(state.Key + state.Value.Item1.ToString());
                if (state.Value.Item1)
                {
                    startStateID = state.Key;
                    numStartStates++;
                }
                if (state.Value.Item2)
                {
                    numFinalStates++;
                }
            }

            if (numStartStates != 1)
            {
                result = "Need exactly one start state, there are currently " + numStartStates;
                return result;
            }
        
            if (numFinalStates < 1)
            {
                result = "Need at least one final state, there are currently " + numFinalStates;
                return result;
            }
                
        }
        else
        {
            result = "Automata contains no states";
            return result;
        }
        
        // Check if there is an edge for each symbol in alphabet from each state



        return result;
    }

    public bool CheckInputWord(string word)
    {
        if (CheckAutomataValidity() == "Valid")
        {
            // Check if input word is empty and initial state is accepting
            if (word == "" && states[startStateID].Item2)
            {
                return true;
            }
            else
            {
                for (int i = 0; i < word.Length; i++)
                {

                }
            }
        }
        return false;
    }

    public bool StepThroughInput(string word)
    {
        return false;
    }
}
