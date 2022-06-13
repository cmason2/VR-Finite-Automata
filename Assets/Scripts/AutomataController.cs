using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AutomataController : MonoBehaviour
{

    [SerializeField] int numStates = 0;
    [SerializeField] int lastStateID = -1;

    private List<State> stateObjects;

    private List<string> alphabet;
    private List<int> states;
    private List<int> startStates;
    private List<int> finalStates;
    private Dictionary<int, List<(string, int)>> transitions;
    private string inputWord;
    private GameObject leftHandController;
    [SerializeField] GameObject keyboard;
    public string edgeSymbols;

    private StaticAutomata playerAutomata;
    private StaticAutomata comparisonAutomata;

    // Start is called before the first frame update
    void Start()
    {
        leftHandController = GameObject.Find("LeftHand Controller");

        InitialiseAutomata();

        string validityString = CheckAutomataValidity();
        if (validityString == "Valid")
        {
            Debug.Log("Automata is VALID");
            playerAutomata = new StaticAutomata(alphabet, states, startStates[0], finalStates, transitions);
            playerAutomata.PrintToConsole();

            inputWord = "aa";
            Debug.Log("\"" + inputWord + "\" is " + (CheckInputWord(playerAutomata, inputWord) ? "accepted" : "NOT accepted"));

        }
        else
        {
            Debug.Log("Automata is INVALID: " + validityString);
        }

        //comparisonAutomata = ExampleAutomata();
        //comparisonAutomata.PrintToConsole();
    }

    void InitialiseAutomata()
    {
        alphabet = new List<string>();
        states = new List<int>();
        startStates = new List<int>();
        finalStates = new List<int>();
        transitions = new Dictionary<int, List<(string, int)>>();

        stateObjects = new List<State>(FindObjectsOfType<State>()); // Add existing states in scene

        numStates = stateObjects.Count;
        lastStateID = stateObjects.Count - 1;

        // Add key in transitions dictionary for each pre-existing state, check if state is start/final state
        foreach (State state in stateObjects)
        {
            int stateID = state.GetStateID();
            states.Add(stateID);
            transitions[stateID] = new List<(string, int)>();

            if (state.IsStartState())
                startStates.Add(stateID);

            if (state.IsFinalState())
                finalStates.Add(stateID);
        }

        // Add pre-existing transitions
        List<Bezier> edges = new List<Bezier>(FindObjectsOfType<Bezier>());
        foreach (Bezier edge in edges)
        {
            State sourceState = edge.GetSourceState();
            State targetState = edge.GetTargetState();
            int sourceID = sourceState.GetStateID();
            int targetID = targetState.GetStateID();

            List<string> symbols = edge.GetSymbolList();

            foreach (string symbol in symbols)
            {
                transitions[sourceID].Add((symbol, targetID));

                // Add any symbols used on edges into the alphabet
                if (!alphabet.Contains(symbol))
                {
                    alphabet.Add(symbol);
                }
            }

            // Add each edge to their connected states
            sourceState.AddEdge(edge);
            targetState.AddEdge(edge);
        }
        alphabet.Sort();
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


    public void AddState(int stateID)
    {
        states.Add(stateID);
        transitions.Add(stateID, new List<(string, int)>());
        Debug.Log("State added: " + stateID);
    }

    public void DeleteState(int stateIDToDelete)
    {
        // Remove the state from the states list
        states.Remove(stateIDToDelete);
        // Remove all transitions from this state
        transitions.Remove(stateIDToDelete);

        // Remove all transitions containing this state as destination state
        foreach (int sourceState in transitions.Keys)
        {
            transitions[sourceState].RemoveAll(i => i.Item2 == stateIDToDelete);
        }
    }

    public void AddTransition(int stateID, List<string> symbols, int nextStateID)
    {
        if (!transitions.ContainsKey(stateID))
        {
            transitions[stateID] = new List<(string, int)>();
        }

        foreach (string sym in symbols)
        {
            if (!alphabet.Contains(sym))
            {
                alphabet.Add(sym);
                Debug.Log("Symbol added: " + alphabet[alphabet.Count - 1]);
            }

            transitions[stateID].Add((sym, nextStateID));
            Debug.Log("Transition added: " + stateID + " : " + sym + " -> " + nextStateID);
        }
    }

    public void DeleteTransition(int stateID, List<string> symbolsToDelete)
    {
        Debug.Log("DeleteTransition: startState = " + stateID);
        if (transitions.ContainsKey(stateID))
        {
            transitions[stateID].RemoveAll(t => symbolsToDelete.Contains(t.Item1));
        }
    }

    public string CheckAutomataValidity()
    {
        string result = "Valid";

        // Check if one start state and at least one final state
        if (states.Count != 0)
        {
            if (startStates.Count != 1)
            {
                result = "Need exactly one start state, there are currently " + startStates.Count;
                return result;
            }

            if (finalStates.Count < 1)
            {
                result = "Need at least one final state, there are currently " + finalStates.Count;
                return result;
            }
        }
        else
        {
            result = "Automata contains no states";
            return result;
        }

        // Check if there is a transition for each symbol in the alphabet from every state
        foreach (int state in states)
        {
            if (transitions.ContainsKey(state))
            {
                List<string> remainingSymbols = new List<string>(alphabet);
                foreach (var transition in transitions[state])
                {
                    if (remainingSymbols.Contains(transition.Item1))
                        remainingSymbols.Remove(transition.Item1);
                    else
                        Debug.Log("symbol present in transition that is not in alphabet");
                }
                if (remainingSymbols.Count != 0)
                {
                    result = "State " + state + " does not have a transition for " + string.Join(",", remainingSymbols);
                    return result;
                }
            }
        }

        return result;
    }

    public bool CheckInputWord(StaticAutomata a, string word)
    {
        if (CheckAutomataValidity() == "Valid")
        {
            // Check if input word is empty and initial state is accepting
            if (word == "" && a.finalStates.Contains(a.startState))
            {
                return true;
            }
            else
            {
                int currentState = a.startState;
                for (int i = 0; i < word.Length; i++)
                {
                    currentState = GetNextState(currentState, word[i].ToString());
                    Debug.Log(word[i].ToString() + "-> " + currentState);

                    if (currentState == -1) // No transitions with current symbol
                        return false;
                }
                if (a.finalStates.Contains(currentState))
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

    private int GetNextState(int stateID, string symbol)
    {
        foreach (var transition in transitions[stateID])
        {
            if (transition.Item1 == symbol)
            {
                return transition.Item2;
            }
        }
        return -1;
    }

    public bool IsSymbolUsed(int state, List<string> symbols)
    {
        Debug.Log("Checking for " + symbols);
        PrintTransitions(state);
        if (transitions.ContainsKey(state))
        {
            foreach (var transition in transitions[state])
            {
                if (symbols.Contains(transition.Item1))
                {
                    return true;
                }
            }
            return false;
        }
        else // State has no transitions, therefore symbol isn't used
        {
            return false;
        }
    }

    private void PrintTransitions(int s)
    {
        Debug.Log("Transitions for state " + s);
        foreach (var transition in transitions[s])
        {
            Debug.Log("\nSymbol: " + transition.Item1 + "   Next State: " + transition.Item2);
        }
    }

    public IEnumerator LoadKeyboard(State state, Bezier edge)
    {
        Debug.Log("In LoadKeyboard() coroutine");
        GameObject keyboardInstance = Instantiate(keyboard, leftHandController.transform);
        SymbolKeyboard keyboardScript = keyboardInstance.GetComponent<SymbolKeyboard>();
        keyboardInstance.transform.localPosition = new Vector3(0.3f, 0f, 0f);

        keyboardScript.SetStateAndEdge(state, edge);

        // Wait until user symbols have been validated
        while (!keyboardScript.valid && !keyboardScript.cancelled)
        {
            yield return null;
        }

        if (keyboardScript.valid)
        {
            edgeSymbols = keyboardScript.symbolsString;
        }
        else // keyboardScript.cancelled == true
        {
            edgeSymbols = "CANCELLED";
        }

        Destroy(keyboardInstance);
    }

    private StaticAutomata ExampleAutomata()
    {
        List<string> alphabet = new List<string>();
        alphabet.Add("a");
        alphabet.Add("b");

        List<int> states = new List<int>();
        states.Add(0);
        states.Add(1);
        states.Add(2);
        states.Add(3);

        int startState = 0;

        List<int> finalStates = new List<int>();
        finalStates.Add(2);

        Dictionary<int, List<(string, int)>> transitions = new Dictionary<int, List<(string, int)>>();
        foreach (int state in states)
        {
            transitions[state] = new List<(string, int)>();
        }

        transitions[0].Add(("a", 1));
        transitions[0].Add(("b", 3));
        transitions[1].Add(("b", 2));
        transitions[1].Add(("a", 3));
        transitions[2].Add(("a", 3));
        transitions[2].Add(("b", 3));
        transitions[3].Add(("a", 3));
        transitions[3].Add(("b", 3));

        return new StaticAutomata(alphabet, states, startState, finalStates, transitions);
    }

    private string CompareAutomata(StaticAutomata a1, StaticAutomata a2)
    {
        
        
        
        
        return "EQUIVALENT";
    }
}
