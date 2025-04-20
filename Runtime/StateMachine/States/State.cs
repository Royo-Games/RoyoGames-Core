
public abstract class State : State<string> { }

public abstract class State<TStateId> : IState<TStateId>
{
    public abstract TStateId StateID { get; set; }
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

    void IState<TStateId>.OnEnter()
    {
        throw new System.NotImplementedException();
    }

    void IState<TStateId>.OnExit()
    {
        throw new System.NotImplementedException();
    }

    void IState<TStateId>.OnFixedUpdate()
    {
        throw new System.NotImplementedException();
    }

    void IState<TStateId>.OnLateUpdate()
    {
        throw new System.NotImplementedException();
    }

    void IState<TStateId>.OnUpdate()
    {
        throw new System.NotImplementedException();
    }
}