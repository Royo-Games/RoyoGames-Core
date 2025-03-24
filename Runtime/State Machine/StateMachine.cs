using System;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    private List<LayerInfo> layers;
    public int LayerCount => layers.Count;
    public Action<int, IState> OnChangedState;

    private bool isChanging;

    private class LayerInfo
    {
        public IState DefaultState;
        public IState PreviousState;
        public IState CurrentState;
    }
    public StateMachine(int layerCount)
    {
        layers = new List<LayerInfo>();

        for (int i = 0; i < layerCount; i++)
        {
            layers.Add(new LayerInfo());
        }
    }
    public void Start()
    {
        for (int i = 0; i < layers.Count; i++)
        {
            ChangeStateToDefault(i);
        }
    }
    public IState GetCurrentState(int layerIndex)
    {
        return layers[layerIndex].CurrentState;
    }
    public IState GetPreviousState(int layerIndex)
    {
        return layers[layerIndex].PreviousState;
    }
    public IState GetDefaultState(int layerIndex)
    {
        return layers[layerIndex].DefaultState;
    }
    public void ChangeState(int layerIndex, IState state)
    {
        if (isChanging)
        {
            throw new Exception("Nested state change attempted! ChangeState cannot be called recursively during state transitions.");
        }

        isChanging = true;

        try
        {
            var layer = layers[layerIndex];

            layer.PreviousState = layer.CurrentState;
            layer.CurrentState = state;

            layer.PreviousState?.OnExit(layerIndex, this);
            layer.CurrentState?.OnEnter(layerIndex, this);

            OnChangedState?.Invoke(layerIndex, state);
        }
        finally
        {
            isChanging = false;
        }
    }
    
    public void ChangeStateToDefault(int layerIndex)
    {
        ChangeState(layerIndex, layers[layerIndex].DefaultState);
    }
    public void ChangeStateToPrevious(int layerIndex)
    {
        ChangeState(layerIndex, layers[layerIndex].PreviousState);
    }
    public void SetDefaultState(int layerIndex, IState state)
    {
        layers[layerIndex].DefaultState = state;
    }
    public void Update()
    {
        for (int i = 0; i < layers.Count; i++)
        {
            var layer = layers[i];

            if (layer.CurrentState == null)
                continue;

            layer.CurrentState.OnUpdate(i, this);
        }
    }
    public void FixedUpdate()
    {
        for (int i = 0; i < layers.Count; i++)
        {
            var layer = layers[i];

            if (layer.CurrentState == null)
                continue;

            layer.CurrentState.OnFixedUpdate(i, this);
        }
    }
    public void LateUpdate()
    {
        for (int i = 0; i < layers.Count; i++)
        {
            var layer = layers[i];

            if (layer.CurrentState == null)
                continue;

            layer.CurrentState.OnLateUpdate(i, this);
        }
    }
}
