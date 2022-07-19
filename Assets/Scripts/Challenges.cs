using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Challenges
{
    private static int challengeNumber = 1;

    private static string[] challengeDescriptions = new string[]
    {
        "All words beginning with \"ab\"",
        "Word length is a multiple of 2 or 3"
    };

    public static Challenge[] challenges = new Challenge[3];

    static Challenges()
    {
        challenges[0] = new Challenge(0, "Words contain an even number of \"a\"s", Generate0(), 2);
        challenges[1] = new Challenge(1, "All words beginning with \"ab\"", Generate1(), 4);
        challenges[2] = new Challenge(2, "Word length is a multiple of 2 or 3", Generate2(), 6);
    }

    public static Challenge GetCurrentChallenge()
    {
        return challenges[challengeNumber];
    }

    static StaticAutomata Generate0()
    {
        // Automata that accepts even number of 'a's (a is only symbol in alphabet)
        List<char> alphabet = new List<char>();
        alphabet.Add('a');

        List<int> states = new List<int>();
        states.Add(0);
        states.Add(1);

        int startState = 0;

        List<int> finalStates = new List<int>();
        finalStates.Add(0);

        Dictionary<int, List<(char, int)>> transitions = new Dictionary<int, List<(char, int)>>();
        foreach (int state in states)
        {
            transitions[state] = new List<(char, int)>();
        }

        transitions[0].Add(('a', 1));
        transitions[1].Add(('a', 0));

        return new StaticAutomata(alphabet, states, startState, finalStates, transitions);
    }

    static StaticAutomata Generate1()
    {
        // Automata that accepts word beginning with ab (alphbet contains "a" and "b")
        List<char> alphabet = new List<char>();
        alphabet.Add('a');
        alphabet.Add('b');

        List<int> states = new List<int>();
        states.Add(0);
        states.Add(1);
        states.Add(2);
        states.Add(3);

        int startState = 0;

        List<int> finalStates = new List<int>();
        finalStates.Add(2);

        Dictionary<int, List<(char, int)>> transitions = new Dictionary<int, List<(char, int)>>();
        foreach (int state in states)
        {
            transitions[state] = new List<(char, int)>();
        }

        transitions[0].Add(('a', 1));
        transitions[0].Add(('b', 3));

        transitions[1].Add(('a', 3));
        transitions[1].Add(('b', 2));

        transitions[2].Add(('a', 2));
        transitions[2].Add(('b', 2));

        transitions[3].Add(('a', 3));
        transitions[3].Add(('b', 3));

        return new StaticAutomata(alphabet, states, startState, finalStates, transitions);
    }

    static StaticAutomata Generate2()
    {
        // Automata that accepts even number of 'a's (a is only symbol in alphabet)
        List<char> alphabet = new List<char>();
        alphabet.Add('a');

        List<int> states = new List<int>();
        states.Add(0);
        states.Add(1);

        int startState = 0;

        List<int> finalStates = new List<int>();
        finalStates.Add(0);

        Dictionary<int, List<(char, int)>> transitions = new Dictionary<int, List<(char, int)>>();
        foreach (int state in states)
        {
            transitions[state] = new List<(char, int)>();
        }

        transitions[0].Add(('a', 1));
        transitions[1].Add(('a', 0));

        return new StaticAutomata(alphabet, states, startState, finalStates, transitions);
    }
}

public class Challenge
{
    int number;
    public string description;
    public StaticAutomata automaton;
    public int minStates;
    bool completed;
    bool minimal;

    public Challenge (int number, string description, StaticAutomata automaton, int minStates)
    {
        this.number = number;
        this.description = description;
        this.automaton = automaton;
        this.minStates = minStates;
        completed = false;
        minimal = false;
    }

    public void SetCompleted()
    {
        completed = true;
    }

    public void SetMinimal()
    {
        minimal = true;
    }
}
