using UnityEngine;

public abstract class StateBehaviour : StateBehaviour<string> { }

public abstract class StateBehaviour<TStateId> : MonoBehaviour, IState<TStateId>
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
}
