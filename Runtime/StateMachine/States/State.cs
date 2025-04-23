
public abstract class State : State<string> { }
public abstract class State<TStateId> : State<TStateId, string> { }
public abstract class State<TStateId, TBlackBoard> : IState<TStateId, TBlackBoard>
{
    public virtual TStateId StateID { get; set; }
    public StateMachine<TStateId, TBlackBoard> StateMachine { get; set; }

    public virtual void OnEnter()
    {
    }

    public virtual void OnExit()
    {
    }

    public virtual void OnFixedUpdate()
    {
    }

    public virtual void OnLateUpdate()
    {
    }

    public virtual void OnUpdate()
    {
    }
}