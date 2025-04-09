using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StepLine : MonoBehaviour
{
    private StateMachine stateMachine = new StateMachine(1);

    [SerializeField] private int startStepIndex;
    [SerializeField] private bool autoStart;
    [Space]
    [SerializeField] private UnityEvent onCompletedStepLineEvent;

    public int CurrentStepIndex { get; private set; }
    public bool AutoStartOnAwake
    {
        get
        {
            return autoStart;
        }
        set
        {
            autoStart = value;
        }
    }

    public int StartStepIndex
    {
        get
        {
            return startStepIndex;
        }
        set
        {
            startStepIndex = value;
        }
    }

    public List<StepBase> Steps { get; private set; } = new List<StepBase>();
    public StepBase CurrentStep { get; set; }
    public UnityEvent OnCompletedStepLineEvent => onCompletedStepLineEvent;

    private int nextStepIndex;
    private bool isChanging;

    public virtual void Start()
    {
        if (autoStart)
        {
            StartStepLine();
        }
    }

    public virtual void Update()
    {
        stateMachine.Update();
    }

    public virtual void FixedUpdate()
    {
        stateMachine.FixedUpdate();
    }

    public virtual void LateUpdate()
    {
        stateMachine.LateUpdate();
    }

    public virtual void StartStepLine()
    {
        ResetStepLine();
        UpdateStepList();
        NextStep();
    }

    public virtual void ResetStepLine()
    {
        CurrentStepIndex = startStepIndex;
        nextStepIndex = startStepIndex;
        CurrentStep = null;
    }

    public virtual void UpdateStepList()
    {
        Steps.Clear();
        GetComponentsInChildren(Steps);

        foreach (StepBase step in Steps)
            step.StepLine = this;
    }

    public virtual void NextStep()
    {
        if (Steps == null || Steps.Count == 0)
            return;

        if (nextStepIndex == Steps.Count)
        {
            if (CurrentStep != null)
            {
                CurrentStep = null;
                stateMachine.ChangeState(0, null);
                onCompletedStepLineEvent?.Invoke();
            }

            return;
        }

        ChangeStep(nextStepIndex);
        nextStepIndex++;
    }

    private void ChangeStep(int stepIndex)
    {
        if (isChanging)
        {
            throw new Exception("Nested state change attempted! ChangeState cannot be called recursively during state transitions.");
        }

        isChanging = true;

        try
        {
            CurrentStep = Steps[stepIndex];
            CurrentStepIndex = stepIndex;

            stateMachine.ChangeState(0, CurrentStep);
        }
        finally
        {
            isChanging = false;
        }
    }
}
