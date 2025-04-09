using UnityEngine;

public class State : MonoBehaviour, IState
{
    public virtual void OnEnter(StateMachine stateMachine)
    {
    }

    public virtual void OnExit(StateMachine stateMachine)
    {
    }

    public virtual void OnFixedUpdate(StateMachine stateMachine)
    {
    }

    public virtual void OnLateUpdate(StateMachine stateMachine)
    {
    }

    public virtual void OnUpdate(StateMachine stateMachine)
    {
    }
}
