using System;
using System.Collections;
using System.Collections.Generic;

public class StateQueue : StateQueue<string>
{
    public StateQueue(StateMachine<string> stateMachine) : base(stateMachine)
    {
    }
}

public class StateQueue<TStateId>
{
    public Action OnCompletedQueue;

    private readonly Queue<TStateId> _queue = new();
    private readonly StateMachine<TStateId> _stateMachine;

    public int Count => _queue.Count;

    public StateQueue(StateMachine<TStateId> stateMachine)
    {
        _stateMachine = stateMachine
            ?? throw new ArgumentNullException(nameof(stateMachine));
    }

    public void EnqueueState(TStateId stateId)
    {
        if (stateId == null)
            throw new ArgumentException("State ID cannot be empty.", nameof(stateId));
        _queue.Enqueue(stateId);
    }

    public bool DequeueState()
    {
        if (_queue.Count == 0)
        {
            OnCompletedQueue?.Invoke();
            return false;
        }

        TStateId nextStateID = _queue.Dequeue();
        _stateMachine.ChangeState(nextStateID);

        if(nextStateID == null)
            return false;
        else
            return true;
    }

    public void Clear()
    {
        _queue.Clear();
    }

    public bool TryPeek(out TStateId stateId)
    {
        if (_queue.Count > 0)
        {
            stateId = _queue.Peek();
            return true;
        }

        stateId = default!;
        return false;
    }
}