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

    public void PrintToConsole()
    {
        Debug.Log("Alphabet: " + string.Join(",", alphabet));
        Debug.Log("States: " + string.Join(",", states));
        Debug.Log("Starting State: " + startState);
        Debug.Log("Final State(s): " + string.Join(",", finalStates));
        Debug.Log("Transitions");
        foreach (var state in transitions.Keys)
        {
            foreach (var transition in transitions[state])
            {
                Debug.Log(state + " : " + transition.Item1 + " -> " + transition.Item2);
            }
        }
    }
}
