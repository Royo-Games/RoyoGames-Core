using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState
{
    public StateMachine StateMachine { get; set; }
    public void OnEnter();
    public void OnExit();
    public void OnUpdate();
    public void OnFixedUpdate();
    public void OnLateUpdate();
}
