
public abstract class State<TStateId> : IState<TStateId>
{
    public virtual TStateId StateID { get; set; }
    public StateMachine<TStateId> StateMachine { get; set; }

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