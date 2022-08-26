using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Challenges
{
    private static int challengeNumber = 0;

    public static Challenge[] challenges = new Challenge[7];

    static Challenges()
    {
        challenges[0] = new Challenge(0, "Words with a maximum length of 2", "{ a , b }", Generate0(), 0, 4);
        challenges[1] = new Challenge(1, "Words starting with the subword \"ab\"", "{ a , b }", Generate1(), 0, 4);
        challenges[2] = new Challenge(2, "Words whose length is a multiple of two or three", "{ a }", Generate2(), 1, 6);
        challenges[3] = new Challenge(3, "Words containing the subword \"bab\"", "{ a , b }", Generate3(), 1, 4);
        challenges[4] = new Challenge(4, "Words ending with the subword \"abba\"", "{ a , b }", Generate4(), 1, 5);
        challenges[5] = new Challenge(5, "Words with even number of 'a's and odd number of 'b's", "{ a , b }", Generate5(), 1, 4);
        challenges[6] = new Challenge(6, "Words starting with 'a' that don't contain subword \"cb\"", "{ a , b , c }", Generate6(), 2, 4);
    }

    public static Challenge GetCurrentChallenge()
    {
        return challenges[challengeNumber];
    }

    public static void SetCurrentChallenge(int challengeNumber)
    {
        Challenges.challengeNumber = challengeNumber;
    }

    static StaticAutomata Generate0()
    {
        // Automata that accepts words of max length 2
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
        finalStates.Add(0);
        finalStates.Add(1);
        finalStates.Add(2);


        Dictionary<int, List<(char, int)>> transitions = new Dictionary<int, List<(char, int)>>();
        foreach (int state in states)
        {
            transitions[state] = new List<(char, int)>();
        }

        transitions[0].Add(('a', 1));
        transitions[1].Add(('a', 2));
        transitions[2].Add(('a', 3));
        transitions[3].Add(('a', 3));
        transitions[0].Add(('b', 1));
        transitions[1].Add(('b', 2));
        transitions[2].Add(('b', 3));
        transitions[3].Add(('b', 3));

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
        // Automata that accepts words that have length that is a multiple of 2 or 3
        List<char> alphabet = new List<char>();
        alphabet.Add('a');

        List<int> states = new List<int>();
        states.Add(0);
        states.Add(1);
        states.Add(2);
        states.Add(3);
        states.Add(4);
        states.Add(5);

        int startState = 0;

        List<int> finalStates = new List<int>();
        finalStates.Add(0);
        finalStates.Add(2);
        finalStates.Add(3);
        finalStates.Add(4);

        Dictionary<int, List<(char, int)>> transitions = new Dictionary<int, List<(char, int)>>();
        foreach (int state in states)
        {
            transitions[state] = new List<(char, int)>();
        }

        transitions[0].Add(('a', 1));
        transitions[1].Add(('a', 2));
        transitions[2].Add(('a', 3));
        transitions[3].Add(('a', 4));
        transitions[4].Add(('a', 5));
        transitions[5].Add(('a', 0));

        return new StaticAutomata(alphabet, states, startState, finalStates, transitions);
    }

    static StaticAutomata Generate3()
    {
        // Automata that accepts words containing subword "bab"
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
        finalStates.Add(3);

        Dictionary<int, List<(char, int)>> transitions = new Dictionary<int, List<(char, int)>>();
        foreach (int state in states)
        {
            transitions[state] = new List<(char, int)>();
        }

        transitions[0].Add(('a', 0));
        transitions[0].Add(('b', 1));
        
        transitions[1].Add(('a', 2));
        transitions[1].Add(('b', 1));
        
        transitions[2].Add(('a', 0));
        transitions[2].Add(('b', 3));

        transitions[3].Add(('a', 3));
        transitions[3].Add(('b', 3));

        return new StaticAutomata(alphabet, states, startState, finalStates, transitions);
    }

    static StaticAutomata Generate4()
    {
        // Automata that accepts words ending in "abba" (alphbet contains "a" and "b")
        List<char> alphabet = new List<char>();
        alphabet.Add('a');
        alphabet.Add('b');

        List<int> states = new List<int>();
        states.Add(0);
        states.Add(1);
        states.Add(2);
        states.Add(3);
        states.Add(4);

        int startState = 0;

        List<int> finalStates = new List<int>();
        finalStates.Add(4);

        Dictionary<int, List<(char, int)>> transitions = new Dictionary<int, List<(char, int)>>();
        foreach (int state in states)
        {
            transitions[state] = new List<(char, int)>();
        }

        transitions[0].Add(('a', 1));
        transitions[0].Add(('b', 0));

        transitions[1].Add(('a', 1));
        transitions[1].Add(('b', 2));

        transitions[2].Add(('a', 1));
        transitions[2].Add(('b', 3));

        transitions[3].Add(('a', 4));
        transitions[3].Add(('b', 0));

        transitions[4].Add(('a', 1));
        transitions[4].Add(('b', 2));

        return new StaticAutomata(alphabet, states, startState, finalStates, transitions);
    }

    static StaticAutomata Generate5()
    {
        // Automata that accepts words with even number of 'a's and odd number of 'b's
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
        transitions[0].Add(('b', 2));

        transitions[1].Add(('a', 0));
        transitions[1].Add(('b', 3));

        transitions[2].Add(('a', 3));
        transitions[2].Add(('b', 0));

        transitions[3].Add(('a', 2));
        transitions[3].Add(('b', 1));

        return new StaticAutomata(alphabet, states, startState, finalStates, transitions);
    }

    static StaticAutomata Generate6()
    {
        // Automata that accepts words beginning with 'a' that don't contain the subword "cb"
        List<char> alphabet = new List<char>();
        alphabet.Add('a');
        alphabet.Add('b');
        alphabet.Add('c');

        List<int> states = new List<int>();
        states.Add(0);
        states.Add(1);
        states.Add(2);
        states.Add(3);

        int startState = 0;

        List<int> finalStates = new List<int>();
        finalStates.Add(1);
        finalStates.Add(2);

        Dictionary<int, List<(char, int)>> transitions = new Dictionary<int, List<(char, int)>>();
        foreach (int state in states)
        {
            transitions[state] = new List<(char, int)>();
        }

        transitions[0].Add(('a', 1));
        transitions[0].Add(('b', 3));
        transitions[0].Add(('c', 3));

        transitions[1].Add(('a', 1));
        transitions[1].Add(('b', 1));
        transitions[1].Add(('c', 2));

        transitions[2].Add(('a', 1));
        transitions[2].Add(('b', 3));
        transitions[2].Add(('c', 2));

        transitions[3].Add(('a', 3));
        transitions[3].Add(('b', 3));
        transitions[3].Add(('c', 3));

        return new StaticAutomata(alphabet, states, startState, finalStates, transitions);
    }
}

public class Challenge
{
    public int number;
    public string description;
    public string alphabet;
    public StaticAutomata automaton;
    public int minStates;
    public int difficulty;
    public bool completed;
    public bool minimal;

    public Challenge (int number, string description, string alphabet, StaticAutomata automaton, int difficulty, int minStates)
    {
        this.number = number;
        this.description = description;
        this.alphabet = alphabet;
        this.automaton = automaton;
        this.minStates = minStates;
        this.difficulty = difficulty;
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
