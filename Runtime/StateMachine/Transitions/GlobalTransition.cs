using System;
using UnityEditor.Experimental.GraphView;

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