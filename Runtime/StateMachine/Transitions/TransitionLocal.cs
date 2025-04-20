
public class TransitionLocal : TransitionLocal<string>
{
    public TransitionLocal(string fromState, string toState, ICondition condition) : base(fromState, toState, condition){}
}
public class TransitionLocal<TStateId> : ITransition<TStateId>
{
    public TStateId FromState { get; set; }
    public TStateId ToState { get; set; }
    public ICondition Condition { get; set; }


    public TransitionLocal(TStateId fromState, TStateId toState, ICondition condition)
    {
        FromState = fromState;
        ToState = toState;
        Condition = condition;
    }
}
