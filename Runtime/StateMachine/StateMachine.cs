using System;
using System.Collections.Generic;
using System.Linq;

public class StateMachine : StateMachine<string, Empty>
{
    public StateMachine() : base(new Empty()) { }
}

public class StateMachine<TStateId> : StateMachine<TStateId, Empty>
{
    public StateMachine() : base(new Empty()) { }
}

[Serializable]
public class StateMachine<TStateId, TBlackBoard> where TBlackBoard : class
{
    public IState<TStateId, TBlackBoard> CurrentState { get; private set; }
    public Action<IState<TStateId, TBlackBoard>> OnEnterState { get; set; }
    public Action<IState<TStateId, TBlackBoard>> OnExitState { get; set; }
    public bool IsStarted { get; private set; }

    private readonly Dictionary<TStateId, IState<TStateId, TBlackBoard>> _states = new();
    private readonly Dictionary<TStateId, List<LocalTransition<TStateId, TBlackBoard>>> _localTransitions = new();
    private readonly List<GlobalTransition<TStateId, TBlackBoard>> _globalTransitions = new();
    private readonly Queue<TStateId> _pendingStates = new();
    public TBlackBoard BlackBoard { get; private set; }

    private bool _isTransitioning;

    public StateMachine(TBlackBoard blackBoard)
    {
        BlackBoard = blackBoard;
    }

    public void Update()
    {
        if (!IsStarted) return;

        HandleTransitions();
        CurrentState?.OnUpdate();
    }

    public void LateUpdate()
    {
        if (!IsStarted) return;
        CurrentState?.OnLateUpdate();
    }
    public void FixedUpdate()
    {
        if (!IsStarted) return;
        CurrentState?.OnFixedUpdate();
    }

    public void Start()
    {
        IsStarted = true;
    }

    public void Start(TStateId initialStateId)
    {
        IsStarted = true;
        ChangeState(initialStateId);
    }

    public void Stop()
    {
        if (CurrentState != null)
        {
            CurrentState.OnExit();
            OnExitState?.Invoke(CurrentState);
            CurrentState = null;
        }

        _pendingStates.Clear();
        _isTransitioning = false;
        IsStarted = false;
    }

    public void ChangeState(TStateId stateId)
    {
        if (!IsStarted)
            throw new InvalidOperationException("StateMachine has not been started.");

        if (stateId == null)
            throw new ArgumentNullException(nameof(stateId));

        if (!_states.ContainsKey(stateId))
            throw new InvalidOperationException($"Unregistered state: {stateId}");

        if (_isTransitioning)
        {
            _pendingStates.Enqueue(stateId);
            return;
        }
        else
        {
            _isTransitioning = true;

            try
            {
                if (CurrentState != null)
                {
                    CurrentState.OnExit();
                    OnExitState?.Invoke(CurrentState);
                }

                CurrentState = _states[stateId];
                CurrentState.StateMachine = this;
                CurrentState.OnEnter();
                OnEnterState?.Invoke(CurrentState);
            }
            finally
            {
                _isTransitioning = false;

                while (_pendingStates.Count > 0)
                {
                    var pendingId = _pendingStates.Dequeue();
                    ChangeState(pendingId);
                }
            }
        }
    }

    public bool ContainsState(TStateId stateId)
    {
        return _states.ContainsKey(stateId);
    }

    public IState<TStateId, TBlackBoard> GetState(TStateId stateId)
    {
        if (_states.TryGetValue(stateId, out var state))
            return state;
        else
            return null;
    }

    public void HandleTransitions()
    {
        if (_isTransitioning)
            return;

        GlobalTransition<TStateId, TBlackBoard> globalTransition = null;
        foreach (var t in _globalTransitions)
        {
            if (t.Condition(BlackBoard))
            {
                globalTransition = t;
                break;
            }
        }

        if (globalTransition != null && CurrentState != null
            && !globalTransition.ToState.Equals(CurrentState.StateID))
        {
            ChangeState(globalTransition.ToState);
            return;
        }

        if (CurrentState == null)
            return;

        if (!_localTransitions.TryGetValue(CurrentState.StateID, out var list))
            return;

        LocalTransition<TStateId, TBlackBoard> localTransition = null;
        foreach (var t in list)
        {
            if (t.Condition(BlackBoard))
            {
                localTransition = t;
                break;
            }
        }

        if (localTransition != null
            && !localTransition.ToState.Equals(CurrentState.StateID))
        {
            ChangeState(localTransition.ToState);
        }
    }

    public void AddState(IState<TStateId, TBlackBoard> state)
    {
        if (_states.ContainsKey(state.StateID))
            throw new InvalidOperationException($"State already exists: {state.StateID}");

        _states[state.StateID] = state;
    }

    public void RemoveState(IState<TStateId, TBlackBoard> state)
    {
        if (!_states.Remove(state.StateID))
            return;

        _localTransitions.Remove(state.StateID);
        var fromStates = _localTransitions.Keys.ToList();
        foreach (var from in fromStates)
        {
            var list = _localTransitions[from];
            list.RemoveAll(t => t.ToState.Equals(state.StateID));
            if (list.Count == 0)
                _localTransitions.Remove(from);
        }

        _globalTransitions.RemoveAll(t => t.ToState.Equals(state.StateID));

        var remaining = _pendingStates.Where(id => !id.Equals(state.StateID)).ToList();
        _pendingStates.Clear();
        foreach (var id in remaining)
            _pendingStates.Enqueue(id);

        if (CurrentState != null && CurrentState.StateID.Equals(state.StateID))
        {
            CurrentState.OnExit();
            CurrentState = null;
        }
    }

    public void AddTransition(ITransition<TStateId, TBlackBoard> transition)
    {
        switch (transition)
        {
            case LocalTransition<TStateId, TBlackBoard> localTransition:
                if (!_states.ContainsKey(localTransition.FromState) || !_states.ContainsKey(localTransition.ToState))
                {
                    var missing = new List<string>();
                    if (!_states.ContainsKey(localTransition.FromState)) missing.Add($"From '{localTransition.FromState}'");
                    if (!_states.ContainsKey(localTransition.ToState)) missing.Add($"To '{localTransition.ToState}'");
                    throw new InvalidOperationException($"Cannot add transition, unregistered state(s): {string.Join(", ", missing)}");
                }

                if (!_localTransitions.TryGetValue(localTransition.FromState, out var list))
                {
                    list = new List<LocalTransition<TStateId, TBlackBoard>>();
                    _localTransitions[localTransition.FromState] = list;
                }

                if (list.Any(t => t.ToState.Equals(localTransition.ToState) && t.Condition == transition.Condition))
                    throw new InvalidOperationException($"Duplicate transition from '{localTransition.FromState}' to '{transition.ToState}'");

                list.Add(localTransition);
                break;

            case GlobalTransition<TStateId, TBlackBoard> globalTransition:
                if (_globalTransitions.Any(t => t.ToState.Equals(globalTransition.ToState) && t.Condition == globalTransition.Condition))
                    throw new InvalidOperationException($"Duplicate global transition to '{globalTransition.ToState}'");

                _globalTransitions.Add(globalTransition);
                break;
        }
    }

    public LocalTransition<TStateId, TBlackBoard> AddLocalTransition(TStateId from, TStateId to, Func<TBlackBoard, bool> condition)
    {
        LocalTransition<TStateId, TBlackBoard> transition = new(from, to, condition);
        AddTransition(transition);
        return transition;
    }

    public GlobalTransition<TStateId, TBlackBoard> AddGlobalTransition(TStateId to, Func<TBlackBoard, bool> condition)
    {
        GlobalTransition<TStateId, TBlackBoard> transition = new(to, condition);
        AddTransition(transition);
        return transition;
    }

    public void RemoveTransition(ITransition<TStateId, TBlackBoard> transition)
    {
        switch (transition)
        {
            case LocalTransition<TStateId, TBlackBoard> localTransition:
                if (_localTransitions.TryGetValue(localTransition.FromState, out var list))
                {
                    list.Remove(localTransition);
                    if (list.Count == 0)
                        _localTransitions.Remove(localTransition.FromState);
                }
                break;

            case GlobalTransition<TStateId, TBlackBoard> globalTransition:
                _globalTransitions.Remove(globalTransition);
                break;
        }
    }

    public void Reset(bool resetEvents = true)
    {
        Stop();

        _states.Clear();
        _globalTransitions.Clear();
        _localTransitions.Clear();
        _pendingStates.Clear();

        CurrentState = null;
        IsStarted = false;
        _isTransitioning = false;

        if (resetEvents)
        {
            OnEnterState = null;
            OnExitState = null;
        }
    }
}
