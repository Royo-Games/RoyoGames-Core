using System.Collections;
using UnityEngine;

public static class AnimatorExtentions
{
    public static IEnumerator WaitNormalizedTimeCoroutine(this Animator animator, int layer, float targetNormalizedTime)
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitUntil(() => animator.WaitNormalizedTime(layer, targetNormalizedTime));
    }
    private static bool WaitNormalizedTime(this Animator animator, int layer, float targetNormalizedTime)
    {
        var stateInfo = animator.GetCurrentAnimatorStateInfo(layer);
        return !animator.IsInTransition(layer) && stateInfo.normalizedTime >= targetNormalizedTime;
    }
}
