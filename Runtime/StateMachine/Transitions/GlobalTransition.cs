using System;

public class GlobalTransition<TStateId, TManager> : ITransition<TStateId, TManager> where TManager : class
{
    public TStateId FromState { get; set; }
    public TStateId ToState { get; set; }
    public Func<TManager, bool> Condition { get; set; }

    public GlobalTransition(TStateId toState, Func<TManager, bool> condition)
    {
        ToState = toState;
        Condition = condition;
    }
}