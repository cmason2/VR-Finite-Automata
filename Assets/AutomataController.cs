using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AutomataController : MonoBehaviour
{

    [SerializeField] int numStates = 0;
    [SerializeField] int lastStateID = -1;
    private State startState;
    private List<string> alphabet;
    //private Dictionary<int, (bool, bool)> states;
    private List<State> states;
    private Dictionary<State, List<(Bezier, State)>> transitions;

    // Start is called before the first frame update
    void Start()
    {
        alphabet = new List<string>();
        states = new List<State>(FindObjectsOfType<State>()); // Add existing states in scene
        numStates = states.Count;
        lastStateID = states.Count - 1;

        transitions = new Dictionary<State, List<(Bezier, State)>>();
        List<Bezier> edges = new List<Bezier>(FindObjectsOfType<Bezier>());
        foreach (Bezier edge in edges)
        {
            State sourceState = edge.GetSourceState();
            
            // Add state key in transitions dictionary if it doesn't exist
            if (!transitions.ContainsKey(sourceState))
            {
                transitions[sourceState] = new List<(Bezier, State)>();
            }
            
            transitions[sourceState].Add((edge, edge.GetTargetState()));
        }

        foreach (State state in transitions.Keys)
        {
            foreach (var transition in transitions[state])
            {
                Debug.Log(state.GetStateID() + transition.Item1.GetSymbolText() + transition.Item2.GetStateID());
            }
        }
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

    public void AddState(State state)
    {
        states.Add(state);
        transitions.Add(state, new List<(Bezier, State)>());
        Debug.Log("State added: " + state.GetStateID());
    }

    public void AddTransition(State state, Bezier edge, State nextState)
    {
        List<string> symbols = edge.GetSymbolList();
        foreach (string sym in symbols)
        {
            if (!alphabet.Contains(sym))
            {
                alphabet.Add(sym);
                Debug.Log("Symbol added: " + alphabet[alphabet.Count - 1]);
            }
        }
        
        transitions[state].Add((edge, nextState));
        Debug.Log("Transition added: " + state.GetStateID() + edge.GetSymbolText() + nextState.GetStateID());
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
                //Debug.Log(state.Key + state.Value.Item1.ToString());
                if (state.IsStartState())
                {
                    startState = state;
                    numStartStates++;
                }
                if (state.IsFinalState())
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
            if (word == "" && startState.IsFinalState())
            {
                return true;
            }
            else
            {
                State currentState = startState;
                for (int i = 0; i < word.Length; i++)
                {
                    currentState = GetNextState(currentState, word[i].ToString());
                    
                    if (currentState == null) // No transitions with current symbol
                        return false;
                }
                if (currentState.IsFinalState())
                    return true;
                else
                    return false;
            }
        }
        else
        {
            return false;
        }
    }

    public bool StepThroughInput(string word)
    {
        return false;
    }

    private State GetNextState(State state, string symbol)
    {
        foreach (var transition in transitions[state])
        {
            if (transition.Item1.GetSymbolList().Contains(symbol))
            {
                return transition.Item2;
            }
        }

        return null;
    }
}
