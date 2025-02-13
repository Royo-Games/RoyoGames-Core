using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class AnimatorStateEventListener : MonoBehaviour
{
    public UnityEvent<AnimatorStateEventInfo> OnAnimationEvent => onAnimationEvent;

    [Space]
    [SerializeField]
    private UnityEvent<AnimatorStateEventInfo> onAnimationEvent;
}
