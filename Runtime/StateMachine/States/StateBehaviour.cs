using UnityEngine;

public abstract class StateBehaviour<TStateId, TManager> : MonoBehaviour, IState<TStateId, TManager> where TManager : class
{
    public virtual TStateId StateID { get; set; }
    public StateMachine<TStateId, TManager> StateMachine { get; set; }

    public virtual void OnEnter(){}
    public virtual void OnExit(){}
    public virtual void OnFixedUpdate(){}
    public virtual void OnLateUpdate(){}
    public virtual void OnUpdate(){}
}
