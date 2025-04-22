
using System;

public interface ITransition<TStateId, TBlackBoard>
{
    public Func<TBlackBoard, bool> Condition { get; set; }
    public TStateId ToState { get; set; }
}
