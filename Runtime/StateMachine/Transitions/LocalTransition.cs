
using System;

public class LocalTransition<TStateId, TBlackBoard> : ITransition<TStateId, TBlackBoard>
{
    public TStateId FromState { get; set; }
    public TStateId ToState { get; set; }
    public Func<TBlackBoard, bool> Condition { get; set; }


    public LocalTransition(TStateId fromState, TStateId toState, Func<TBlackBoard, bool> condition)
    {
        FromState = fromState;
        ToState = toState;
        Condition = condition;
    }
}
