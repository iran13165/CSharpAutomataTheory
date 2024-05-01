
using System.ComponentModel;
using static State;

// State s1 = new(false);
// State s2 = new(true);
// State s3 = new(true);
// s1.AddTransitionForSymbol("a", s2);
// s1.AddTransitionForSymbol("a", s3);
// var s = s1.GetTransitionsForSymbol("a");
// Console.WriteLine(s);
Builders builders = new Builders();
NFA a = builders._char("a");
NFA b = builders._char("b");
//NFA c = builders._char("c");
NFA n = builders.Or(a, b);

DFA dFA = new(n);
dFA.Minimize();


//bool isAccept = n.inState.Test("crt",n.inState);

;

//Console.WriteLine(isAccept);

