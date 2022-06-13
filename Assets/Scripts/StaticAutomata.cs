using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticAutomata
{
    public List<string> alphabet;
    public List<int> states;
    public int startState;
    public List<int> finalStates;
    public Dictionary<int, List<(string, int)>> transitions;

    public StaticAutomata(List<string> alphabet, List<int> states, int startState, List<int> finalStates, Dictionary<int, List<(string, int)>> transitions)
    {
        this.alphabet = alphabet;
        this.states = states;
        this.startState = startState;
        this.finalStates = finalStates;
        this.transitions = transitions;
    }
}
