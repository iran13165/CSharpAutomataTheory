class NFAState : State
{
    public NFAState(bool accepting) : base(accepting)
    {
    }
    public HashSet<State> GetEpsilonClosure(State StartState)
    {
        var EpsilonTransitions = StartState.GetTransitionsForSymbol("ε");
        var closure = new HashSet<State>();
        var _EpsilonClosure = new HashSet<State>();
        closure.Add(StartState);

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

}