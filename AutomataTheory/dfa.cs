


class DFA(NFA nfa)
{
    readonly NFA nfa = nfa;
    readonly HashSet<string> OriginalAccecptingStateNumbers = [];
     HashSet<int> AccecptingStateNumbers = [];
    readonly Dictionary<int, Dictionary<string, int>> FinalDfaTable = [];
    Dictionary<int, HashSet<int>> currentTransitionMap = [];
    HashSet<string> GetAlphabet()
    {
        return nfa.GetAlphabet();
    }

    public Dictionary<int, Dictionary<string, int>> GetTransionTable()
    {
        var NfaTable = this.nfa.GetTransitionTable();
        var NfaStates = NfaTable.Select(x => x.Key).ToArray();
        // Start state of DFA is E(S[nfa])
        var startState = NfaTable[NfaStates[0]]["ε*"];
        // Init the worklist (states which should be in the DFA).
        int[][] worklist = [[.. startState]];

        var alphabet = this.nfa.GetAlphabet();

        var NfaAcceptingStates = this.nfa.GetAcceptingStateNumbers();

        Dictionary<string, Dictionary<string, string>> DafaTable = [];

        void UpdateAcceptingStates(int[] states)
        {
            foreach (var NfaAcceptingState in NfaAcceptingStates)
            {
                if (states.Contains(NfaAcceptingState))
                {
                    OriginalAccecptingStateNumbers.Add(string.Join(",", states));
                    break;
                }
            }

        }
        while (worklist.Length > 0)
        {
            var states = worklist[0];
            worklist = worklist.Where(x => x != worklist[0]).ToArray();
            var dfaStateLabel = string.Join(",", states);
            DafaTable[dfaStateLabel] = [];
            foreach (var symbol in alphabet)
            {
                HashSet<int> OnSymbol = [];
                UpdateAcceptingStates(states);
                foreach (var state in states)
                {

                    if (!NfaTable[state].ContainsKey(symbol))
                    {
                        continue;
                    }
                    var NfaStatesOnSymbol = NfaTable[state][symbol];
                    foreach (var NfaStateOnSymbol in NfaStatesOnSymbol)
                    {
                        if (!NfaTable.ContainsKey(NfaStateOnSymbol))
                        {
                            continue;
                        }
                        var p = NfaTable[NfaStateOnSymbol]["ε*"];
                        foreach (var item in p)
                        {
                            OnSymbol.Add(item);
                        }
                    }
                }
                var dfaStatesOnSymbolSet = new HashSet<int>(OnSymbol);
                int[] dfaStatesOnSymbol = [.. OnSymbol];
                if (dfaStatesOnSymbol.Length > 0)
                {
                    var dfaOnSymbolStr = string.Join(",", dfaStatesOnSymbol);
                    DafaTable[dfaStateLabel][symbol] = dfaOnSymbolStr;
                    if (!DafaTable.ContainsKey(dfaOnSymbolStr))
                    {
                        worklist = [.. worklist, dfaStatesOnSymbol];
                    }
                }
            }
        }
        return RemapStateNumbers(DafaTable);
    }
    Dictionary<int, Dictionary<string, int>> RemapStateNumbers(Dictionary<string, Dictionary<string, string>> CalculatedDfaTable)
    {
        Dictionary<string, int> NewStateMap = [];

        var StateLabels = CalculatedDfaTable.Keys.ToArray();
        for (int i = 1; i <= StateLabels.Length; i++)
        {
            NewStateMap[StateLabels[i - 1]] = i;
        }
        foreach (var item in CalculatedDfaTable)
        {
            var OriginalRow = CalculatedDfaTable[item.Key];
            Dictionary<string, int> row = [];

            foreach (var symbol in OriginalRow)
            {
                row[symbol.Key] = NewStateMap[symbol.Value];
            }
            FinalDfaTable[NewStateMap[item.Key]] = row;

        }
        //Remap Accepting State Numbers
        foreach (var state in OriginalAccecptingStateNumbers)
        {
            AccecptingStateNumbers.Add(NewStateMap[state]);
        }

        return FinalDfaTable;
    }
    public Dictionary<int, Dictionary<string, int>> Minimize()
    {

        var table = GetTransionTable();
        var allStates = table.Keys.ToArray();
        var alphabet = GetAlphabet();
        var Accepting = AccecptingStateNumbers;



        HashSet<int> NonAccepting = [];
        foreach (var state in allStates)
        {
            var isAccepting = Accepting.Contains(state);
            if (isAccepting)
            {
                currentTransitionMap[state] = Accepting;
            }
            else
            {
                NonAccepting.Add(state);
                currentTransitionMap[state] = NonAccepting;

            }
        }
        int[][][] all = [[[.. NonAccepting], [.. Accepting]]];
        all = all.Where(x => x.Length >= 0).ToArray();
        int[][] current = all[^1];


        int[][] previous = [];
        if (all.Length - 2 >= 0)
        {
            previous = all[^2];
        }
        while (!SameRow(current, previous))
        {
            Dictionary<int, HashSet<int>> NewTransitionMap = [];
            foreach (var set in current)
            {
                Dictionary<int, HashSet<int>> handledStates = [];
                var first = set.FirstOrDefault();
                var rest = set.Skip(1);
                handledStates[first] = [first];

                foreach (var state in rest)
                {
                    Dictionary<int, HashSet<int>> helpingHandling = [];
                    foreach (var item in handledStates)
                    {
                        helpingHandling[item.Key] = item.Value;
                    }
                    foreach (var handledState in helpingHandling.Keys)
                    {
                        if (AreEquvalent(state, handledState, table, alphabet))
                        {
                            handledStates[handledState].Add(state);
                            handledStates[state] = handledStates[handledState];
                            continue;
                        } 
                        else
                        {
                             handledStates[state] = [state];
                        }
                    }
                   
                }
                foreach (var item in handledStates)
                {
                    NewTransitionMap[item.Key] = item.Value;
                }

            }
            currentTransitionMap = NewTransitionMap;

           HashSet< HashSet<int>> newSets = [];
           foreach (var item in currentTransitionMap)
           {
            newSets.Add(item.Value);
           }
           int [][] r = [];
           foreach (var item in newSets)
           {
            r= [.. r, [.. item]];
           }
           all = [.. all,r];

           current = all[^1];
           previous = all[^2];
            
        }
        Dictionary<HashSet<int>,int> remaped = [];
        int idx = 0;
        foreach (var item in current)
        {
            idx++;
            remaped[[.. item]] = idx;
            
        }
        Dictionary<int,Dictionary<string,int>> minimizedTable = [];
        HashSet<int> minimizedAcceptingStates =[];
        void updateAcceptingStates(int[] set,int idx)
        {
            foreach(var state in set)
            {
                if(Accepting.Contains(state))
                {
                    minimizedAcceptingStates.Add(idx);
                }
            }
        }
        Dictionary<int[], int> a=[];
        // a[[2,3]] = 4;
        // var k = a[[2,3]];
        foreach (var item in remaped)
        {
            minimizedTable[item.Value] = [];
            foreach (var symbol in alphabet)
            {
                updateAcceptingStates(item.Key.ToArray(), item.Value);
                int originalTransition = -1;
                foreach (var originalState in item.Key)
                {
                    if(table[originalState].ContainsKey(symbol))
                    {
                        originalTransition = table[originalState][symbol];
                        break;
                    }
                }
                if(originalTransition != -1)
                {
                    var p = currentTransitionMap[originalTransition];
                    foreach (var re in remaped)
                    {
                        if(re.Key.SetEquals(p))
                        {
                            minimizedTable[item.Value][symbol] = remaped[re.Key];
                        }
                    }
                     //minimizedTable[item.Value][symbol] = remaped[p];
                }
            }
        }
        table = minimizedTable;
        this.AccecptingStateNumbers=minimizedAcceptingStates;
        return table;

    }

    private bool AreEquvalent(int state, int handledState, Dictionary<int, Dictionary<string, int>> table, HashSet<string> alphabet)
    {
        foreach (var symbol in alphabet)
        {
            if (!GoToSameSet(state, handledState, table, symbol))
            {
                return false;
            }

        }
        return true;
    }

    private bool GoToSameSet(int state, int handledState, Dictionary<int, Dictionary<string, int>> table, string symbol)
    {
        if (currentTransitionMap[state] != currentTransitionMap[handledState])
        {
            return false;
        }
        int s1 = -1;
        var originalTransitionS1 = table[state].ContainsKey(symbol);
        if (originalTransitionS1)
        {
            s1 = table[state][symbol];
        }
        int s2 = -1;
        var originalTransitionS2 = table[state].ContainsKey(symbol);
        if (originalTransitionS2)
        {
            s2 = table[handledState][symbol];
        }
        // If no actual transition on this symbol, treat it as positive.
        if (!originalTransitionS1 && !originalTransitionS2)
        {
            return true;
        }
        // Otherwise, check if they are in the same sets.
        return currentTransitionMap[state].Contains(s1) &&
        currentTransitionMap[handledState].Contains(s2);

    }

    private bool SameRow(int[][] current, int[][] previous)
    {
        if (current.Length == 0)
        {
            return false;
        }
        if (current.Length != previous.Length)
        {
            return false;
        }
        for (int i = 0; i < current.Length; i++)
        {
            var s1 = current[i];
            var s2 = previous[i];
            if (s1.Length != s2.Length)
            {
                return false;
            }
            if (string.Join(",", s1.OrderDescending()) != string.Join(",", s2.OrderDescending()))
            {
                return false;
            }
        }
        return true;
    }
}