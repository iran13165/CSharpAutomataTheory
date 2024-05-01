
using System.Collections.Generic;
class Builders
{
    public NFA _char(string c)
    {
        State inState = new(false);
        State outState = new(true);

        State t = inState.AddTransitionForSymbol(c, outState);
        return new(t, outState);
    }
    // -----------------------------------------------------------------------------
    // Epsilon NFA fragment

    /**
     * Epsilon factory.
     *
     * Creates an NFA fragment for ε (recognizes an empty string).
     *
     * [in] --ε--> [out]
     */
    public NFA e()
    {
        return _char("ε");
    }
    public NFA ConcatPair(NFA first, NFA second)
    {
        first.outState.accepting = false;
        second.outState.accepting = true;

        first.outState.AddTransitionForSymbol("ε", second.inState);
        return new(first.inState, second.outState);
    }
    public NFA Concat(params NFA[] nfs)
    {
        NFA first = nfs[0];
        for (int i = 1; i < nfs.Length; i++)
        {
            first = ConcatPair(first, nfs[i]);
        }
        return first;
    }
    public NFA OrPair(NFA first, NFA second)
    {
        State inState = new(false);
        State outState = new(true);
        inState.AddTransitionForSymbol("ε", first.inState);
        inState.AddTransitionForSymbol("ε", second.inState);

        outState.accepting = true;
        first.outState.accepting = false;
        second.outState.accepting = false;

        first.outState.AddTransitionForSymbol("ε", outState);
        second.outState.AddTransitionForSymbol("ε", outState);

        return new(inState, outState);

    }
    public NFA Or(params NFA[] nfs)
    {
        NFA first = nfs[0];

        for (int i = 1; i < nfs.Length; i++)
        {
            first = OrPair(first, nfs[i]);
        }

        return first;
    }
    /**
    * Optimized Kleene-star: just adds ε-transitions from
    * input to the output, and back.
    */
    public NFA Rep(NFA fragment)
    {
        fragment.inState.AddTransitionForSymbol("ε", fragment.outState);
        fragment.outState.AddTransitionForSymbol("ε", fragment.inState);
        return fragment;
    }
    /**
    * Optimized Plus: just adds ε-transitions from
    * the output to the input.
    */
    public NFA PlusRep(NFA fragment)
    {
        fragment.outState.AddTransitionForSymbol("ε", fragment.inState);
        return fragment;
    }
    /**
    * Optimized ? repetition: just adds ε-transitions from
    * the input to the output.
    */
    public NFA QuestionRep(NFA fragment)
    {
        fragment.inState.AddTransitionForSymbol("ε", fragment.outState);
        return fragment;
    }
  
}