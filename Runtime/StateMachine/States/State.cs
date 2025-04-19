
public abstract class State : State<string> { }

public abstract class State<TStateId> : IState<TStateId>
{
    public abstract TStateId StateID { get; }

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