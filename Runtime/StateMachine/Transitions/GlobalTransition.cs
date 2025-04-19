public class GlobalTransition : GlobalTransition<string>
{
    public GlobalTransition(string toState, ICondition condition) : base(toState, condition){}
}

public class GlobalTransition<TStateId> : ITransition<TStateId>
{
    public ICondition Condition { get; set; }
    public TStateId ToState { get; set; }

    public GlobalTransition(TStateId toState, ICondition condition)
    {
        ToState = toState;
        Condition = condition;
    }
}
