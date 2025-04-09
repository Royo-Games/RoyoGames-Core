using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState
{
    public void OnEnter(StateMachine stateMachine);
    public void OnExit(StateMachine stateMachine);
    public void OnUpdate(StateMachine stateMachine);
    public void OnFixedUpdate(StateMachine stateMachine);
    public void OnLateUpdate(StateMachine stateMachine);
}
