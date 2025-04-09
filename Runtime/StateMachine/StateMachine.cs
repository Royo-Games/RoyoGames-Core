using System;
using System.Collections.Generic;

public class StateMachine
{
    public Action<IState> OnChangedState;
    public Action<StateMachine> OnCompletedSteps;

    public IState DefaultState { get; set; }
    public IState PreviousState { get; private set; }
    public IState CurrentState { get; private set; }

    public int CurrentStepIndex => _currentStepIndex;

    public List<IState> Steps;

    private bool _isChanging;
    private int _currentStepIndex;
    private bool _isCompletedSteps;

    private Queue<IState> pendingStates;

    public StateMachine()
    {
        Steps = new List<IState>();
        pendingStates = new Queue<IState>();
    }

    public void ChangeState(IState state)
    {
        if (_isChanging)
        {
            pendingStates.Enqueue(state);
            return;
        }

        _isChanging = true;

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
            _isChanging = false;

            while (pendingStates.Count > 0)
                ChangeState(pendingStates.Dequeue());
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

    public void StartStep(int startStepIndex)
    {
        _isCompletedSteps = false;
        SetStep(startStepIndex);
    }

    public void NextStep()
    {
        if (Steps == null || Steps.Count == 0)
            return;

        int nextStepIndex = _currentStepIndex + 1;

        if (nextStepIndex >= Steps.Count)
        {
            if (!_isCompletedSteps)
                OnCompletedSteps?.Invoke(this);

            _isCompletedSteps = true;
            return;
        }

        SetStep(nextStepIndex);
    }

    private void SetStep(int stepIndex)
    {
        if (Steps == null || Steps.Count == 0 || stepIndex < 0 || stepIndex >= Steps.Count)
            return;

        _currentStepIndex = stepIndex;
        ChangeState(Steps[_currentStepIndex]);
    }
}
