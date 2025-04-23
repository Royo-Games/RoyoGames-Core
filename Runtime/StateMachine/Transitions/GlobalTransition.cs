using System;

public class GlobalTransition : GlobalTransition<string, string>
{
    public GlobalTransition(string toState, Func<string, bool> condition) : base(toState, condition) { }
}
public class GlobalTransition<TStateId> : GlobalTransition<TStateId, string>
{
    public GlobalTransition(TStateId toState, Func<string, bool> condition) : base(toState, condition){}
}
public class GlobalTransition<TStateId, TBlackBoard> : ITransition<TStateId, TBlackBoard>
{
    public TStateId FromState { get; set; }
    public TStateId ToState { get; set; }
    public Func<TBlackBoard, bool> Condition { get; set; }

    public GlobalTransition(TStateId toState, Func<TBlackBoard, bool> condition)
    {
        ToState = toState;
        Condition = condition;
    }
}