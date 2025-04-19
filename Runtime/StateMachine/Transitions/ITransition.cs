
public interface ITransition : ITransition<string> { }
public interface ITransition<TStateId>
{
    public ICondition Condition { get; set; }
    public TStateId ToState { get; set; }
}
