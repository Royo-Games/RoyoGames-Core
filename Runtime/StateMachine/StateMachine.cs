using System;

public class StateMachine
{
    public Action<IState> OnChangedState;

    private bool isChanging;

    public IState DefaultState { get; set; }
    public IState PreviousState { get; private set; }
    public IState CurrentState { get; private set; }

    public void ChangeState(IState state)
    {
        if (isChanging)
        {
            throw new Exception("Nested state change attempted! ChangeState cannot be called recursively during state transitions.");
        }

        isChanging = true;

        try
        {
            PreviousState = CurrentState;
            CurrentState = state;

            PreviousState?.OnExit(this);
            CurrentState?.OnEnter(this);

            OnChangedState?.Invoke(state);
        }
        finally
        {
            isChanging = false;
        }
    }

    public void ChangeStateToDefault()
    {
        ChangeState(DefaultState);
    }

    public void ChangeStateToPrevious()
    {
        ChangeState(PreviousState);
    }

    public void Update()
    {
        if (CurrentState == null)
            return;

        CurrentState.OnUpdate(this);
    }
    public void FixedUpdate()
    {
        if (CurrentState == null)
            return;

        CurrentState.OnFixedUpdate(this);
    }
    public void LateUpdate()
    {
        if (CurrentState == null)
            return;

        CurrentState.OnLateUpdate(this);
    }
}
