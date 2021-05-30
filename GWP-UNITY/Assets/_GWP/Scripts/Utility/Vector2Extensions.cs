using System.Collections.Generic;
using UnityEngine;

public static class Vector2Extensions
{
    public static float RandomRange(this Vector2 vector) => Random.Range(vector.x, vector.y);

    public static Vector3 ToVector3XZ(this Vector2 vector2) => new Vector3(vector2.x, 0, vector2.y);

    public static Vector2 Rotate(this Vector2 v, float degrees) => Quaternion.Euler(0, 0, degrees) * v;

    public static T FindClosest<T>(this Vector2 point, IEnumerable<T> ts, System.Func<Vector2, T, float> getSqDistance)
    {
        T closest = default;
        float minSqDistance = float.MaxValue;

        foreach (var t in ts)
        {
            float distance = getSqDistance(point, t);
            if (minSqDistance < distance) continue;
            minSqDistance = distance;
            closest = t;
        }

        return closest;
    }
}
