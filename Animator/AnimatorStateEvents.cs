using System;
using UnityEngine;

public class AnimatorStateEvents : StateMachineBehaviour
{
    public EventInfo[] Events;
    private float loopCount;

    private AnimatorStateEventListener listener;

    [Serializable]
    public class EventInfo
    {
        public string eventName;
        [Range(0, 1.0f)]
        public float normalizedTime;

        [HideInInspector]
        public bool isTriggered;
    }
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (listener == null)
            listener = animator.GetComponent<AnimatorStateEventListener>();

        loopCount = 0;
        ResetTriggers();
        base.OnStateEnter(animator, stateInfo, layerIndex);
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (listener == null)
            return;

        var normalizedTime = stateInfo.normalizedTime - loopCount;

        foreach (var trigger in Events)
        {
            if (trigger.isTriggered)
                continue;

            if (trigger.normalizedTime <= normalizedTime)
            {
                listener.OnAnimationEvent.Invoke(
                    new AnimatorStateEventInfo(trigger.eventName, 
                    stateInfo,
                    trigger.normalizedTime,
                    layerIndex));

                trigger.isTriggered = true;
            }
        }

        if (normalizedTime >= 1)
        {
            ResetTriggers();
            loopCount++;
        }

        base.OnStateUpdate(animator, stateInfo, layerIndex);
    }
    private void ResetTriggers()
    {
        foreach (var trigger in Events)
        {
            trigger.isTriggered = false;
        }
    }
}
