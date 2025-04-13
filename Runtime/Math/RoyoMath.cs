using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RoyoMath
{
    public static float NormalizeAngle(float angle)
    {
        angle = angle % 360;

        if (angle < 0)
            angle += 360f;
        return angle;
    }

    public static int CoordToIndex(Vector2Int coord, Vector2Int Size)
    {
        return coord.y * Size.y + coord.x;
    }
    public static Vector2Int IndexToCoord(int index, Vector2Int Size)
    {
        int x = index % Size.x;
        int y = index / Size.x;

        return new Vector2Int(x, y);
    }
    public static Vector3 Parabola(Vector3 start, Vector3 end, float height, float t)
    {
        var mid = Vector3.Lerp(start, end, t);
        float parabolicOffset = Parabola(t, height);
        return new Vector3(mid.x, parabolicOffset + Mathf.Lerp(start.y, end.y, t), mid.z);
    }
    public static Vector2 Parabola(Vector2 start, Vector2 end, float height, float t)
    {
        var mid = Vector2.Lerp(start, end, t);
        float parabolicOffset = Parabola(t, height);
        return new Vector2(mid.x, parabolicOffset + Mathf.Lerp(start.y, end.y, t));
    }
    public static float Parabola(float t, float height)
    {
        return -4f * height * t * t + 4f * height * t;
    }
    public static bool ScanRadius(
      Vector3 startPosition,
      Vector3 targetPosition,
      Vector3 scanDirection,
      float scanAngle,
      float scanRadius,
      float targetRadius)
    {
        Vector3 toEnemy = targetPosition - startPosition;
        float distanceToEnemy = toEnemy.magnitude;

        if (distanceToEnemy >= scanRadius + targetRadius)
            return false;

        float centerAngle = Vector3.Angle(scanDirection, toEnemy);

        float ratio = targetRadius / distanceToEnemy;
        ratio = Mathf.Clamp01(ratio);

        float offsetAngle = Mathf.Asin(ratio) * Mathf.Rad2Deg;
        float halfAngleWithRadius = (scanAngle * 0.5f) + offsetAngle;

        return centerAngle <= halfAngleWithRadius;
    }
}
