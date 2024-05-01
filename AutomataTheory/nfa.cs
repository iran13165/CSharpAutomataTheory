class NFA(State inState, State outState)
{
    public State inState = inState;
    public State outState = outState;

    public HashSet<State> AcceptingStates = [];
    readonly HashSet<int> acceptingStateNumbers = [];
    readonly HashSet<string> Alphabet = [];
    
    public HashSet<string> GetAlphabet() {
    if (this.Alphabet.Count == 0) {
      var table = this.GetTransitionTable();
      foreach (var state in table) {
        var transitions = table[state.Key];
        foreach (var symbol in transitions)
          if (symbol.Key != "ε*") {
            this.Alphabet.Add(symbol.Key);
          }
      }
    }
    return this.Alphabet;
  }
    /**
  * Returns set of accepting states.
  */
    public HashSet<State> GetAcceptingStates()
    {
        if (this.AcceptingStates.Count == 0)
        {
            // States are determined during table construction.
            this.GetTransitionTable();
        }
        return AcceptingStates;
    }

    /**
     * Returns accepting state numbers.
     */
    public HashSet<int> GetAcceptingStateNumbers()
    {

        if (AcceptingStates.Count != 0)
        {
            foreach (var acceptingState in this.GetAcceptingStates())
            {
                acceptingStateNumbers.Add(acceptingState.Number);
            }
        }
        return acceptingStateNumbers;
    }
    public static HashSet<State> GetEpsilonClosure(State StartState)
    {
        var EpsilonTransitions = StartState.GetTransitionsForSymbol("ε");
        var closure = new HashSet<State>
        {
            StartState
        };

        foreach (var NextState in EpsilonTransitions)
        {
            if (!closure.Contains(NextState))
            {
                closure.Add(NextState);
                var NextClosure = GetEpsilonClosure(NextState);
                foreach (var state in NextClosure)
                {
                    closure.Add(state);
                }
            }

        }


        return closure;
    }
    public Dictionary<int, Dictionary<string, HashSet<int>>> GetTransitionTable()
    {
        Dictionary<int, Dictionary<string, HashSet<int>>> transitionTable = new();

        HashSet<int> AcceptanceStates = [];

        HashSet<State> Visited = [];

        void VisitState(State state)
        {
            if (Visited.Contains(state))
            {
                return;
            }
            Visited.Add(state);

            state.Number = Visited.Count;
            transitionTable.Add(state.Number, []);
            if (state.accepting)
            {
                AcceptingStates.Add(state);
            }
            var transitions = state.getTransitions();

            foreach (var transition in transitions)
            {
                HashSet<int> CombineState = [];
                foreach (var next in transition.Value)
                {
                    VisitState(next);
                    CombineState.Add(next.Number);
                }
                transitionTable[state.Number].Add(transition.Key, CombineState);
            }

        }

        VisitState(this.inState);

        foreach (var state in Visited)
        {
            transitionTable[state.Number].Remove("ε");

            HashSet<State> EpsilonClosure = GetEpsilonClosure(state);

            transitionTable[state.Number].Add("ε*", EpsilonClosure.Select(x => x.Number).ToHashSet());
        }

        return transitionTable;
    }
}