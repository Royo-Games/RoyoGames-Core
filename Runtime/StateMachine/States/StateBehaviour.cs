using UnityEngine;

public abstract class StateBehaviour : StateBehaviour<string> { }
public abstract class StateBehaviour<TStateId> : StateBehaviour<TStateId, Empty> { }
public abstract class StateBehaviour<TStateId, TBlackBoard> : MonoBehaviour, IState<TStateId, TBlackBoard> where TBlackBoard : class
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
