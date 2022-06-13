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

        alphabet = new List<string>();
        states = new List<State>(FindObjectsOfType<State>()); // Add existing states in scene
        numStates = states.Count;
        lastStateID = states.Count - 1;

        transitions = new Dictionary<State, List<(Bezier, State)>>();

        // Add key in transitions dictionary for each pre-existing state
        foreach (State state in states)
        {
            transitions[state] = new List<(Bezier, State)>();
        }

        // Add pre-existing transitions
        List<Bezier> edges = new List<Bezier>(FindObjectsOfType<Bezier>());
        foreach (Bezier edge in edges)
        {
            State sourceState = edge.GetSourceState();
            State targetState = edge.GetTargetState();
            
            transitions[sourceState].Add((edge, edge.GetTargetState()));
            
            // Add each edge to their connected states
            sourceState.AddEdge(edge);
            targetState.AddEdge(edge);

            // Add any symbols used on edges into the alphabet
            List<string> edgeSymbols = edge.GetSymbolList();
            foreach (string symbol in edgeSymbols)
            {
                if (!alphabet.Contains(symbol))
                {
                    alphabet.Add(symbol);
                }
            }
        }

        //foreach (State state in transitions.Keys)
        //{
        //    foreach (var transition in transitions[state])
        //    {
        //        Debug.Log(state.GetStateID() + transition.Item1.GetSymbolText() + transition.Item2.GetStateID());
        //    }
        //}

        //foreach (string symbol in alphabet)
        //{
        //    Debug.Log(symbol);
        //}

        inputWord = "ab";
        string validityResult = CheckAutomataValidity();
        if (validityResult == "Valid")
        {
            alphabet.Sort();
            Debug.Log("Alphabet: " + string.Join(",", alphabet));
            Debug.Log("VALID AUTOMATA: Input word \"" + inputWord + "\" is " + CheckInputWord(inputWord));
        }
        else
        {
            Debug.Log(validityResult);
        }

        comparisonAutomata = ExampleAutomata();
        Debug.Log(CompareAutomata(playerAutomata, comparisonAutomata));
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

    public void DeleteState(State stateToDelete)
    {
        // Remove the state from the states list
        states.Remove(stateToDelete);
        // Remove all transitions from this state
        transitions.Remove(stateToDelete);

        // Remove all transitions containing this state as destination state
        foreach (State sourceState in transitions.Keys)
        {
            transitions[sourceState].RemoveAll(i => i.Item2 == stateToDelete);
        }
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
        
        if (!transitions.ContainsKey(state))
        {
            transitions[state] = new List<(Bezier, State)>();
        }
        transitions[state].Add((edge, nextState));
        Debug.Log("Transition added: " + state.GetStateID() + edge.GetSymbolText() + nextState.GetStateID());
    }

    public void DeleteTransition(State state, Bezier edgeToDelete)
    {
        Debug.Log("DeleteTransition: startState = " + state);
        if (transitions.ContainsKey(state))
        {
            transitions[state].RemoveAll(item => item.Item1 == edgeToDelete);
        }
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
                    Debug.Log(word[i].ToString() + "-> " + (!(currentState is null) ? currentState.name : "NOWHERE"));
                    
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

    public bool IsSymbolUsed(State state, Bezier edge, string symbols)
    {
        Debug.Log("Checking for " + symbols);
        PrintTransitions(state);
        List<string> symbolList = new List<string>(symbols.Split(','));
        if(transitions.ContainsKey(state))
        {
            foreach (var transition in transitions[state])
            {
                // Check if the transition corresponds to an edge being edited
                if (transition.Item1 != edge)
                {
                    foreach (string symbol in symbolList)
                    {
                        if (transition.Item1.GetSymbolList().Contains(symbol))
                            return true;
                    }
                }
            }
            return false;
        }
        else // State has no transitions, therefore symbol isn't used
        {
            return false;
        }
    }

    private void PrintTransitions(State s)
    {
        Debug.Log("Transitions for state " + s.GetStateID());
        foreach (var transition in transitions[s])
        {
            Debug.Log("\nSymbols: " + transition.Item1.GetSymbolText() + "   Next State: " + transition.Item2.GetStateID());
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
