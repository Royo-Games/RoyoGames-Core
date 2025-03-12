using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState
{
    public void OnEnter(int layerIndex,StateMachine stateMachine);
    public void OnExit(int layerIndex,StateMachine stateMachine);
    public void OnUpdate(int layerIndex, StateMachine stateMachine);
    public void OnFixedUpdate(int layerIndex, StateMachine stateMachine);
    public void OnLateUpdate(int layerIndex, StateMachine stateMachine);
}
