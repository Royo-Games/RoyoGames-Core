
using System;

public class LocalTransition<TStateId, TManager> : ITransition<TStateId, TManager>
{
    public TStateId FromState { get; set; }
    public TStateId ToState { get; set; }
    public Func<TManager, bool> Condition { get; set; }

    public LocalTransition(TStateId fromState, TStateId toState, Func<TManager, bool> condition)
    {
        FromState = fromState;
        ToState = toState;
        Condition = condition;
    }
}