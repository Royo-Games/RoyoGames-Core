using UnityEngine;


public abstract class StateBehaviour : StateBehaviour<string>
{
}

public abstract class StateBehaviour<TStateId> : MonoBehaviour, IState<TStateId>
{
    public virtual TStateId StateID { get; set; }
    public virtual void OnEnter() { }
    public virtual void OnExit() { }
    public virtual void OnFixedUpdate() { }
    public virtual void OnLateUpdate() { }
    public virtual void OnUpdate() { }
}
