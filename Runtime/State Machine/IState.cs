using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState
{
    public abstract void OnEnter(int layerIndex,StateMachine stateMachine);
    public abstract void OnExit(int layerIndex,StateMachine stateMachine);
    public abstract void OnUpdate(int layerIndex, StateMachine stateMachine);
    public abstract void OnFixedUpdate(int layerIndex, StateMachine stateMachine);
    public abstract void OnLateUpdate(int layerIndex, StateMachine stateMachine);
}
