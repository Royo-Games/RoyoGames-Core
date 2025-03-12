using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepBase : MonoBehaviour, IState
{
    public StepLine StepLine { get; set; }

    void IState.OnEnter(int layerIndex, StateMachine stateMachine) { OnEnter(StepLine); }
    void IState.OnExit(int layerIndex, StateMachine stateMachine) { OnExit(StepLine); }
    void IState.OnUpdate(int layerIndex, StateMachine stateMachine) { OnUpdate(StepLine); }
    void IState.OnFixedUpdate(int layerIndex, StateMachine stateMachine) { OnFixedUpdate(StepLine); }
    void IState.OnLateUpdate(int layerIndex, StateMachine stateMachine) { OnLateUpdate(StepLine); }

    public virtual void OnEnter(StepLine stepLine) { }
    public virtual void OnExit(StepLine stepLine) { }
    public virtual void OnUpdate(StepLine stepLine) { }
    public virtual void OnFixedUpdate(StepLine stepLine) { }
    public virtual void OnLateUpdate(StepLine stepLine) { }
    public virtual void NextStep()
    {
        StepLine.NextStep();
    }
}
