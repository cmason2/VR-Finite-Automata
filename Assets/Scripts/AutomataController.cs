using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class AutomataController : MonoBehaviour
{

    [SerializeField] int numStates = 0;
    [SerializeField] int lastStateID = -1;
    public int stepStatus = 0;
    private State startState;
    private List<string> alphabet;
    private List<State> states;
    private List<State> finalStates;
    private Dictionary<State, List<(Bezier, State)>> transitions;
    [SerializeField] GameObject leftController;
    [SerializeField] GameObject rightController;
    private CreateEdge leftCreateEdgeScript;
    private CreateEdge rightCreateEdgeScript;
    private ShowMenu showMenuScript;
    private CreateState createStateScript;
    private ToggleStateType toggleStateTypeScript;
    [SerializeField] SkinnedMeshRenderer leftMeshRenderer;
    [SerializeField] XRRayInteractor leftRayInteractor;
    [SerializeField] XRRayInteractor rightRayInteractor;
    [SerializeField] TMP_Text outputText;
    [SerializeField] TMP_InputField wordInputText;
    [SerializeField] Button nextButton;
    [SerializeField] Button previousButton;
    [SerializeField] GameObject keyboard;
    [SerializeField] SymbolKeyboard keyboardScript;
    [SerializeField] GameObject menu;
    public string edgeSymbols;

    private StaticAutomata userAutomata;

    // Start is called before the first frame update
    void Start()
    {
        leftCreateEdgeScript = leftController.GetComponent<CreateEdge>();
        rightCreateEdgeScript = rightController.GetComponent<CreateEdge>();
        showMenuScript = leftController.GetComponent<ShowMenu>();
        createStateScript = rightController.GetComponent<CreateState>();
        toggleStateTypeScript = rightController.GetComponent<ToggleStateType>();

        alphabet = new List<string>();
        states = new List<State>(FindObjectsOfType<State>()); // Add existing states in scene
        numStates = states.Count;
        Debug.Log("Initial num states = " + numStates);
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
        }

        Debug.Log(CompareAutomata(ExampleAutomata()));
    }

    //Returns an unused identifier for the creation of a new state
    public int GetNextStateID()
    {
        lastStateID++;
        return lastStateID;
    }

    public int GetNumStates()
    {
        return numStates;
    }

    public void AddState(State state)
    {
        numStates++;
        Debug.Log("State added, num states = " + numStates);
        states.Add(state);
        transitions.Add(state, new List<(Bezier, State)>());
        Debug.Log("State added: " + state.GetStateID());
    }

    public void DeleteState(State stateToDelete)
    {
        numStates--;
        Debug.Log("State deleted, num states = " + numStates);
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

    public (bool, string) CheckAutomataValidity()
    {
        string message = "Valid";
        
        // Check if one start state and at least one final state
        if (states.Count != 0)
        {
            finalStates = new List<State>();
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
                    finalStates.Add(state);
                    numFinalStates++;
                }
            }

            if (numStartStates != 1)
            {
                message = "Invalid Automaton\nNeed exactly one start state, there are currently " + numStartStates;
                return (false, message);
            }
        
            if (numFinalStates < 1)
            {
                message = "Invalid Automaton\nNeed at least one final state, there are currently " + numFinalStates;
                return (false, message);
            }
                
        }
        else
        {
            message = "Invalid Automaton\nAutomaton contains no states";
            return (false, message);
        }

        // Calculate the alphabet of used symbols
        alphabet = new List<string>();
        foreach (var stateTransitions in transitions.Values)
        {
            foreach (var transition in stateTransitions)
            {
                foreach (string symbol in transition.Item1.GetSymbolList())
                {
                    if (!alphabet.Contains(symbol))
                        alphabet.Add(symbol);
                }
            }
        }
        alphabet.Sort();

        Debug.Log("Used symbols: " + string.Join(",", alphabet));

        // Check if there is an edge for each symbol in alphabet from each state
        foreach (State state in states)
        {
            if (transitions.ContainsKey(state))
            {
                List<string> remainingSymbols = new List<string>(alphabet);
                foreach (var transition in transitions[state])
                {
                    foreach (string symbol in transition.Item1.GetSymbolList())
                    {
                        if (remainingSymbols.Contains(symbol))
                            remainingSymbols.Remove(symbol);
                        else
                            Debug.Log("symbol present in transition that is not in alphabet");
                    }
                }
                if (remainingSymbols.Count != 0)
                {
                    message = "Invalid Automaton\nState " + state.GetStateID() + " does not have a transition for \"" + string.Join(",", remainingSymbols) + "\"";
                    return (false, message);
                }
            }
        }

        return (true, message);
    }

    public (bool, string) CheckInputWord(string word)
    {
        var validityResult = CheckAutomataValidity();
        if (CheckAutomataValidity().Item1)
        {
            // Check if input word is empty and initial state is accepting
            if (word == "" && startState.IsFinalState())
            {
                return (true, "Empty word is accepted");
            }
            else if (word == "" && !startState.IsFinalState())
            {
                return (false, "Initial state is not accepting");
            }
            else
            {
                State currentState = startState;
                for (int i = 0; i < word.Length; i++)
                {
                    State previousState = currentState;
                    currentState = GetNextState(currentState, word[i].ToString()).Item2;
                    Debug.Log(word[i].ToString() + "-> " + (!(currentState is null) ? currentState.name : "NOWHERE"));
                    
                    if (currentState == null) // No transitions with current symbol
                        return (false, "No " + "\"" + word[i].ToString() + "\" transition from state " + previousState.GetStateID());
                }
                if (currentState.IsFinalState())
                    return (true, "Accepted");
                else
                    return (false, "Final transition leads to State " + currentState.GetStateID() + ", which is not accepting");
            }
        }
        else
        {
            return (false, validityResult.Item2);
        }
    }

    public IEnumerator StepThroughInput(string word)
    {
        RestrictInterations("Step");

        wordInputText.caretWidth = 0;
        wordInputText.interactable = false;
        nextButton.interactable = false;
        previousButton.interactable = false;

        var validityResult = CheckAutomataValidity();
        if (CheckAutomataValidity().Item1) // Automaton is valid
        {
            Color currentColour = new Color(0, 0, 1);
            Color acceptColour = new Color(0, 1, 0);
            Color rejectColour = new Color(1, 0, 0);

            if (word == "")
            {
                if (startState.IsFinalState())
                {
                    wordInputText.text = "<color=#32A852>\u03b5</color>";
                    startState.SetColour(acceptColour);
                    outputText.text = "Empty word is accepted";
                }
                else
                {
                    wordInputText.text = "<color=#FF0000>\u03b5</color>";
                    startState.SetColour(rejectColour);
                    outputText.text = "Initial state is not accepting";
                }

                while (stepStatus == 0)
                {
                    yield return null;
                }

                if (stepStatus == -2)
                {
                    stepStatus = 0;
                    startState.SetMaterial();
                    wordInputText.text = "";
                    wordInputText.interactable = true;
                    wordInputText.caretWidth = 1;
                    yield break;
                }
            }
            else // At least one symbol in input word
            {
                List<(Bezier, State)> previousTransitions = new List<(Bezier, State)>();

                State currentState = startState;
                Bezier currentEdge = null;
                currentState.SetColour(currentColour);

                previousTransitions.Add((null, currentState));

                int currentIndex = 0;
                while (true)
                {
                    // Highlight word
                    wordInputText.text = "<color=#FF0000>" + word.Substring(0, currentIndex) + "</color>" + word.Substring(currentIndex);

                    previousButton.interactable = true;
                    nextButton.interactable = true; // Move this?

                    if (currentState == null)
                    {
                        nextButton.interactable = false;
                    }
                    
                    if (currentIndex == 0)
                    {
                        previousButton.interactable = false;
                    }

                    if (currentIndex == word.Length)
                    {
                        nextButton.interactable = false;
                        if (currentState.IsFinalState())
                        {
                            currentState.SetColour(acceptColour);
                            wordInputText.text = "<color=#32A852>" + word + "</color>";
                            outputText.text = "Accepted";
                        }
                        else
                        {
                            currentState.SetColour(rejectColour);
                            outputText.text = "Final transition leads to state " + currentState.GetStateID() + ", which is not accepting";
                        }
                    }

                    // Wait until a menu button (back, stop, forward) is pressed
                    while (stepStatus == 0)
                    {
                        yield return null;
                    }

                    outputText.text = "";

                    if (stepStatus == 1) // Find next transition
                    {
                        stepStatus = 0;

                        string currentSymbol = word[currentIndex].ToString();
                        if (alphabet.Contains(currentSymbol))
                        {
                            (currentEdge, currentState) = GetNextState(currentState, word[currentIndex].ToString());
                        }
                        else
                        {
                            outputText.text = "Symbol \"" + currentSymbol + "\" is not in the automaton's alphabet";
                            currentState = null;
                        }
                        // outputText.text = word[i].ToString() + "-> " + (!(currentState is null) ? currentState.name : "NOWHERE");

                        Bezier lastTransition = previousTransitions[previousTransitions.Count - 1].Item1;
                        if (lastTransition != null)
                            lastTransition.SetColour(lastTransition.edgeColour); // Set previous edge colour to black
                        previousTransitions[previousTransitions.Count - 1].Item2.SetMaterial(); // Set previous state colour to original

                        if (currentState == null) // No transitions with current symbol
                            previousTransitions[previousTransitions.Count - 1].Item2.SetColour(rejectColour); // Set previous state to reject colour
                        else
                        {
                            currentState.SetColour(currentColour);
                            currentEdge.SetColour(currentColour);
                            previousTransitions.Add((currentEdge, currentState));
                            currentIndex++;
                        }
                    }
                    else if (stepStatus == -1) // Go back to previous transition
                    {
                        stepStatus = 0;

                        if (currentState != null)
                        {
                            currentEdge.SetColour(currentEdge.edgeColour);
                            currentState.SetMaterial();
                            previousTransitions.RemoveAt(previousTransitions.Count - 1);
                            currentIndex--;
                        }

                        (currentEdge, currentState) = previousTransitions[previousTransitions.Count - 1];
                        
                        
                        currentState.SetColour(currentColour);
                        if (currentEdge != null)
                            currentEdge.SetColour(currentColour);
                    }
                    else if (stepStatus == -2) // Stop button pressed
                    {
                        stepStatus = 0;
                        if (currentState == null)
                            currentState = previousTransitions[previousTransitions.Count - 1].Item2;
                        currentState.SetMaterial();
                        if (currentEdge != null)
                            currentEdge.SetColour(currentEdge.edgeColour);
                        wordInputText.text = word;
                        wordInputText.interactable = true;
                        wordInputText.caretWidth = 1;
                    }                    
                }                    
            }
        }
        else // Automaton invalid
        {
            outputText.text = validityResult.Item2;
        }

        EnableAllInteractions();
    }

    private (Bezier, State) GetNextState(State state, string symbol)
    {
        foreach (var transition in transitions[state])
        {
            if (transition.Item1.GetSymbolList().Contains(symbol))
            {
                return (transition.Item1, transition.Item2);
            }
        }

        return (null, null);
    }

    public (bool, string) IsSymbolUsed(State state, Bezier edge, string symbols)
    {
        Debug.Log("Checking for " + symbols);
        //PrintTransitions(state);
        List<string> symbolList = new List<string>(symbols.Split(','));
        List<string> duplicateSymbols = new List<string>();
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
                            duplicateSymbols.Add(symbol);
                    }
                }
            }

            if (duplicateSymbols.Count > 0)
            {
                duplicateSymbols.Sort();
                string message = duplicateSymbols.Count == 1 ? "Symbol \"" : "Symbols \"";
                message += string.Join(",", duplicateSymbols);
                message += "\" already present in other transitions from this state!";
                return (true, message);
            }
            else
            {
                return (false, "");
            }
        }
        else // State has no transitions, therefore symbol isn't used
        {
            return (false, "");
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
        RestrictInterations("SymbolKeyboard");
        menu.SetActive(false); // Make sure menu is hidden
        leftRayInteractor.enabled = false;
        rightRayInteractor.raycastMask = LayerMask.GetMask("UI");

        keyboardScript.SetStateAndEdge(state, edge);
        keyboard.SetActive(true);

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

        keyboardScript.ResetKeyboard();
        keyboard.SetActive(false);
        leftRayInteractor.enabled = true;
        leftMeshRenderer.enabled = true;
        rightRayInteractor.raycastMask = ~0;
        EnableAllInteractions();
    }

    public (bool, string) CompareAutomata(StaticAutomata validAutomata)
    {
        // First check if the user's Automata is valid
        var validityResult = CheckAutomataValidity();
        if (!validityResult.Item1)
        {
            return (false, validityResult.Item2);
        }

        userAutomata = GenerateUserAutomata();

        // MAKE-SET for every state
        int userCount = userAutomata.states.Count;
        int validCount = validAutomata.states.Count;
        int totalCount = userCount + validCount;

        // Offset stateIDs in userAutomata by the number of states in validAutomata to allow for use of array indexing
        userAutomata.states = userAutomata.states.ConvertAll<int>(state => state + validCount);
        userAutomata.finalStates = userAutomata.finalStates.ConvertAll<int>(state => state + validCount);
        userAutomata.startState += validCount;

        Dictionary<int, int> parent = new Dictionary<int, int>();
        Dictionary<int, int> rank = new Dictionary<int, int>();
        Queue<(int, int)> queue = new Queue<(int, int)>();
        Dictionary<(int, int), (int, int, string)> witnessMap = new Dictionary<(int, int), (int, int, string)>();

        Debug.Log("Valid automata stateIDs: " + string.Join(",", validAutomata.states));
        Debug.Log("User  automata stateIDs: " + string.Join(",", userAutomata.states));

        for (int i = 0; i < validCount; i++)
        {
            int sID = validAutomata.states[i];
            parent.Add(sID, sID);
            rank.Add(sID, 0);
        }

        for (int i = 0; i < userCount; i++)
        {
            int sID = userAutomata.states[i];
            parent.Add(sID, sID);
            rank.Add(sID, 0);
        }
        
        Debug.Log(string.Join(",", parent));
        Debug.Log(string.Join(",", rank));

        // Check alphabets are equivalent
        //if (!Enumerable.SequenceEqual(userAutomata.alphabet, validAutomata.alphabet))
        //{
        //    return (false, "Alphabets do not match\nYour alphabet: " + string.Join(",", userAutomata.alphabet) + "\nCorrect alphabet: " + string.Join(",", validAutomata.alphabet));
        //}

        // Process start states
        if (userAutomata.finalStates.Contains(userAutomata.startState) ^ validAutomata.finalStates.Contains(validAutomata.startState))
        {
            return (false, "Witness: ?");
        }
        else
        {
            Merge(userAutomata.startState, validAutomata.startState);
            queue.Enqueue((userAutomata.startState, validAutomata.startState));
        }

        while (queue.Count > 0)
        {
            (int userState, int validState) = queue.Dequeue();

            foreach (string symbol in validAutomata.alphabet)
            {
                int userNextState = -1;
                int validNextState = -1;

                foreach (var transition in userAutomata.transitions[userState - validCount]) // - Offset
                {
                    if (transition.Item1 == symbol)
                    {
                        userNextState = transition.Item2 + validCount; // + Offset
                        break;
                    }
                }

                foreach (var transition in validAutomata.transitions[validState])
                {
                    if (transition.Item1 == symbol)
                    {
                        validNextState = transition.Item2;
                        break;
                    }
                }

                int root1 = FindSet(userNextState);
                int root2 = FindSet(validNextState);

                if (root1 != root2)
                {
                    Merge(root1, root2);
                    (int, int) nextPair = (userNextState, validNextState);
                    queue.Enqueue(nextPair);
                    witnessMap[nextPair] = (userState, validState, symbol);

                    if (userAutomata.finalStates.Contains(userNextState) ^ validAutomata.finalStates.Contains(validNextState))
                    {
                        string witnessString = "";
                        while (nextPair != (userAutomata.startState, validAutomata.startState))
                        {
                            witnessString = witnessMap[nextPair].Item3 + witnessString;
                            nextPair = (witnessMap[nextPair].Item1, witnessMap[nextPair].Item2);
                        }
                        return (false, "Witness: " + witnessString);
                    }
                }
            }
        }

        return (true, "EQUIVALENT");

        void Merge(int x, int y)
        {
            if (rank[x] > rank[y])
                parent[y] = x;
            else
            {
                parent[x] = y;
                if (rank[x] == rank[y])
                    rank[y] = rank[y] + 1;
            }
        }

        int FindSet(int x)
        {
            if (x != parent[x])
                parent[x] = FindSet(parent[x]);

            return parent[x];
        }
    }

    private StaticAutomata GenerateUserAutomata()
    {
        List<int> intStates = new List<int>(states.ConvertAll<int>(state => state.GetStateID()));
        intStates.Sort();
        int intStartState = startState.GetStateID();
        List<int> intFinalStates = new List<int>(finalStates.ConvertAll<int>(state => state.GetStateID()));
        Dictionary<int, List<(string, int)>> flatTransitions = new Dictionary<int, List<(string, int)>>();

        foreach (var state in transitions.Keys)
        {
            int stateID = state.GetStateID();
            flatTransitions[stateID] = new List<(string, int)>();
            foreach (var transition in transitions[state])
            {
                int nextStateID = transition.Item2.GetStateID();
                foreach (string symbol in transition.Item1.GetSymbolList())
                {
                    flatTransitions[stateID].Add((symbol, nextStateID));
                }
            }
        }

        StaticAutomata userAutomata = new StaticAutomata(alphabet, intStates, intStartState, intFinalStates, flatTransitions);
        return userAutomata;
    }

    public StaticAutomata ExampleAutomata()
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

    public void RestrictInterations(string currentTask)
    {
        switch (currentTask)
        {
            case "SymbolKeyboard":
                leftCreateEdgeScript.enabled = false;
                rightCreateEdgeScript.enabled = false;
                showMenuScript.enabled = false;
                createStateScript.enabled = false;
                toggleStateTypeScript.enabled = false;
                break;
            case "Step":
                leftCreateEdgeScript.enabled = false;
                rightCreateEdgeScript.enabled = false;
                showMenuScript.enabled = false;
                createStateScript.enabled = false;
                toggleStateTypeScript.enabled = false;
                break;
            case "CreateEdge":
                createStateScript.enabled = false;
                toggleStateTypeScript.enabled = false;
                break;
        }
    }

    public void EnableAllInteractions()
    {
        leftCreateEdgeScript.enabled = true;
        rightCreateEdgeScript.enabled = true;
        showMenuScript.enabled = true;
        createStateScript.enabled = true;
        toggleStateTypeScript.enabled = true;
    }
}
