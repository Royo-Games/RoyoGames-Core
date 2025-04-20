public class TransitionGlobal : TransitionGlobal<string>
{
    public TransitionGlobal(string toState, ICondition condition) : base(toState, condition){}
}

public class TransitionGlobal<TStateId> : ITransition<TStateId>
{
    public ICondition Condition { get; set; }
    public TStateId ToState { get; set; }

    public TransitionGlobal(TStateId toState, ICondition condition)
    {
        ToState = toState;
        Condition = condition;
    }
}
