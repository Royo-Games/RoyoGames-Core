
public class LocalTransition<TStateId> : ITransition<TStateId>
{
    public TStateId FromState { get; set; }
    public TStateId ToState { get; set; }
    public ICondition Condition { get; set; }


    public LocalTransition(TStateId fromState, TStateId toState, ICondition condition)
    {
        FromState = fromState;
        ToState = toState;
        Condition = condition;
    }
}
