using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StepBase : MonoBehaviour, IState
{
    public StepLine StepLine { get; set; }

    void IState.OnEnter(StateMachine stateMachine) { OnEnter(StepLine); }
    void IState.OnExit(StateMachine stateMachine) { OnExit(StepLine); }
    void IState.OnUpdate(StateMachine stateMachine) { OnUpdate(StepLine); }
    void IState.OnFixedUpdate(StateMachine stateMachine) { OnFixedUpdate(StepLine); }
    void IState.OnLateUpdate(StateMachine stateMachine) { OnLateUpdate(StepLine); }

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
