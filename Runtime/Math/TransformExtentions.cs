using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformExtentions
{
    public static bool CheckAngle(this Transform source, Transform target, float angle)
    {
        var objectDir = target.position - source.position;
        objectDir.y = 0;

        return Vector3.Angle(source.forward, objectDir) <= angle / 2.0f;
    }
    public static bool CheckDistance(this Transform source, Transform target, float distance)
    {
        return Vector3.Distance(source.position, target.position) <= distance;  
    }
}
