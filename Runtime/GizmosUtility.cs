using System.Collections.Generic;
using UnityEngine;

public static class GizmosUtility
{
    public static void Drawlines<T>(Vector3 from, List<T> points) where T : MonoBehaviour
    {
        foreach (MonoBehaviour point in points)
        {
            if (point == null)
                continue;

            Gizmos.DrawLine(from, point.transform.position);
        }
    }
}
