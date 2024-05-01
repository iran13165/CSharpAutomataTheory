class State(bool accepting)
{
  public bool accepting = accepting;
  public int Number { get; set; }
  readonly Dictionary<string, HashSet<State>> _transitions = [];

  public Dictionary<string, HashSet<State>> getTransitions()
  {
    return _transitions;
  }
  public State AddTransitionForSymbol(string symbol, State ToState)
  {
      this.GetTransitionsForSymbol(symbol).Add(ToState);
    return this;
  }
  public HashSet<State>? GetTransitionsForSymbol(string symbol)
  {
    _transitions.TryGetValue(symbol, out HashSet<State>? transitions);
    transitions ??= [];
    if(transitions.Count == 0 && !this._transitions.ContainsKey(symbol))
    this._transitions.Add(symbol, transitions);


    return transitions;
  }
  public bool Test(string str, State startState, HashSet<State>? visited = default)
  {
    visited ??= [];
    if (visited.Contains(startState))
      return false;
    visited.Add(startState);

    if (str.Length == 0)
    {
      if (startState.accepting)
      {
        return true;
      }
      foreach (State nextState in startState.GetTransitionsForSymbol("ε"))
      {
        if (Test("", nextState, visited))
        {
          return true;
        }
        return false;
      }
    }
    if (str.Length > 0)
    {
      string symbol = str[..1];
      string rest = str[1..];

      var symbolTransitions = startState.GetTransitionsForSymbol(symbol);
      foreach (var nextState in symbolTransitions)
      {
        if (Test(rest, nextState))
        {
          return true;
        }
      }
      foreach (var nextState in startState.GetTransitionsForSymbol("ε"))
      {
        if (nextState.Test(str, nextState, visited))
        {
          return true;
        }
      }
    }

    return false;
  }

}