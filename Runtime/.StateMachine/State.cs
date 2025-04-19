using UnityEngine;

public class State : MonoBehaviour, IState
{
    public StateMachine StateMachine { get; set; }

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
