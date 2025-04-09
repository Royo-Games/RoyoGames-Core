using System;
using System.Collections.Generic;

public class StepLine : StateMachine
{
    public Action<StepLine> onCompletedStepLineEvent { get; set; }

    public int CurrentStepIndex { get; private set; }
    public int StartStepIndex
    {
        get
        {
            return _startStepIndex;
        }
        set
        {
            _startStepIndex = value;
        }
    }

    public List<Stepold> Steps { get; private set; } = new List<Stepold>();
    public Stepold CurrentStep { get; set; }

    private int _nextStepIndex;
    private int _startStepIndex;

    public virtual void StartStepLine(int startStepIndex)
    {
        ResetStepLine();
        NextStep();
    }

    public virtual void ResetStepLine()
    {
        CurrentStepIndex = _startStepIndex;
        _nextStepIndex = _startStepIndex;
        CurrentStep = null;
    }

    public virtual void NextStep()
    {
        if (Steps == null || Steps.Count == 0)
            return;

        if (_nextStepIndex == Steps.Count)
        {
            if (CurrentStep != null)
            {
                CurrentStep = null;
                ChangeState(null);
                onCompletedStepLineEvent?.Invoke(this);
            }

            return;
        }

        ChangeStep(_nextStepIndex);
        _nextStepIndex++;
    }

    private void ChangeStep(int stepIndex)
    {
        CurrentStep = Steps[stepIndex];
        CurrentStepIndex = stepIndex;

        ChangeState(CurrentStep);
    }
}
