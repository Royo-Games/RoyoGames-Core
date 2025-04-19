using System;
using System.Collections.Generic;

public class StateStack : StateStack<string>
{
    public StateStack(StateMachine<string> stateMachine) : base(stateMachine)
    {
    }
}

public class StateStack<TStateId> where TStateId : class
{
    public Action OnStackCompleted;

    private readonly Stack<TStateId> _stack = new();
    private readonly StateMachine<TStateId> _stateMachine;

    public int Count
    {
        get
        {
            return _stack.Count;
        }
    }

    public StateStack(StateMachine<TStateId> stateMachine)
    {
        _stateMachine = stateMachine ?? throw new ArgumentNullException(nameof(stateMachine));
    }

    public void PushState(TStateId stateId)
    {
        if (stateId == null) throw new ArgumentNullException(nameof(stateId));

        _stack.Push(stateId);
    }
    public bool PopState()
    {
        if (_stack.Count == 0)
        {
            OnStackCompleted?.Invoke();
            return false;
        }

        TStateId nextStateID = _stack.Pop();
        _stateMachine.ChangeState(nextStateID);

        return true;
    }

    public void Clear()
    {
        _stack.Clear();
    }

    public bool TryPeek(out TStateId stateId)
    {
        if (_stack.Count > 0)
        {
            stateId = _stack.Peek();
            return true;
        }

        stateId = default!;
        return false;
    }
}
