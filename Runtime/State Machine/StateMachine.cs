using System;
using System.Collections.Generic;

public class StateMachine
{
    private List<LayerInfo> layers;
    public int LayerCount => layers.Count;
    public Action<int, IState> OnChangedState;

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
            ResetState(i);
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
        var layer = layers[layerIndex];

        layer.PreviousState = layer.CurrentState;
        layer.CurrentState = state;

        layer.PreviousState?.OnExit(layerIndex, this);
        layer.CurrentState?.OnEnter(layerIndex, this);

        OnChangedState?.Invoke(layerIndex, state);
    }
    public void SetDefaultState(int layerIndex, IState state)
    {
        layers[layerIndex].DefaultState = state;
    }
    public void ResetState(int layerIndex)
    {
        ChangeState(layerIndex, layers[layerIndex].DefaultState);
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
