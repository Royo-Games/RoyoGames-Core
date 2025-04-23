
using System;

public interface ITransition<TStateId, TManager> where TManager : class
{
    public Func<TManager, bool> Condition { get; set; }
    public TStateId ToState { get; set; }
}
