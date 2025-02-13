using UnityEngine;

public struct AnimatorStateEventInfo
{
    public string eventName;
    public float normalizedTime;
    public int layerIndex;
    public AnimatorStateInfo stateInfo;

    public AnimatorStateEventInfo(string eventName, AnimatorStateInfo animatorStateInfo, float normalizedTime, int layerIndex)
    {
        this.eventName = eventName;
        this.stateInfo = animatorStateInfo;
        this.normalizedTime = normalizedTime;
        this.layerIndex = layerIndex;
    }
}
