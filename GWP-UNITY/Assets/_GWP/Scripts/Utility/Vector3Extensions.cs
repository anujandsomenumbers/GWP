using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Vector3Extensions
{
    public enum Axis { X = 0, Y = 1, Z = 2 }

    public static Vector2Int FloorToInt2(this Vector3 vector3, Axis ignored)
    => Vector3ToInt2(vector3, ignored, f => Mathf.FloorToInt(f));

    public static Vector2Int RoundToInt2(this Vector3 vector3, Axis ignored)
        => Vector3ToInt2(vector3, ignored, f => Mathf.RoundToInt(f));

    public static Vector3Int RoundToInt3(this Vector3 vector3)
    => Vector3ToInt3(vector3, f => Mathf.RoundToInt(f));

    public static Vector3 SnapTo(this Vector3 v3, float snapAngle)
        => SnapTo(v3, snapAngle, Quaternion.identity);

    public static Vector2 XZ(this Vector3 vector3)
        => new Vector2(vector3.x, vector3.z);

    public static Vector3 Average(this IEnumerable<Vector3> vector3s, Axis ignored)
    {
        Vector3 sum = Vector3.zero;
        foreach (var vector in vector3s)
        {
            if (ignored != Axis.X) sum.x += vector.x;
            if (ignored != Axis.Y) sum.y += vector.y;
            if (ignored != Axis.Z) sum.z += vector.z;
        }
        return sum / vector3s.Count();
    }

    public static Vector3 SnapTo(this Vector3 v3, float snapAngle, Quaternion reference)
    {
        Vector3 fwd = reference * Vector3.forward;
        Vector3 norm = reference * Vector3.up;

        float angle = Vector3.SignedAngle(fwd, v3, norm);
        float stepCount = Mathf.Round(angle / snapAngle);
        angle = stepCount * snapAngle;
        Vector3 snappedDir = Quaternion.AngleAxis(angle, norm) * fwd;
        return snappedDir * v3.magnitude;
    }

    public static Vector3 Direction(Axis axis)
    {
        switch (axis)
        {
            case Axis.X:
                return Vector3.right;
            case Axis.Y:
                return Vector3.up;
            case Axis.Z:
                return Vector3.forward;
        }

        return Vector3.zero;
    }

    public static Vector3 RotateAround(this Vector3 vec, Vector3 pivot, Quaternion rotation)
    {
        Vector3 dir = vec - pivot;
        dir = rotation * dir;
        return dir + pivot;
    }

    private static Vector2Int Vector3ToInt2(Vector3 v3, Axis ignored, System.Func<float, int> toInt)
    {
        v3 = Vector3.ProjectOnPlane(v3, Direction(ignored));
        Vector2Int v2i = Vector2Int.zero;

        int setIndex = -1;
        for (int i = 0; i < 3; i++)
        {
            if ((int)ignored == i) continue;
            v2i[++setIndex] = toInt(v3[i]);
        }

        return v2i;
    }

    private static Vector3Int Vector3ToInt3(Vector3 v3, System.Func<float, int> toInt)
    {
        Vector3Int v3i = Vector3Int.zero;

        int setIndex = -1;
        for (int i = 0; i < 3; i++)
        {
            v3i[++setIndex] = toInt(v3[i]);
        }

        return v3i;
    }

    public static T FindClosest<T>(this Vector3 point, IEnumerable<T> ts, System.Func<Vector3, T, float> getSqDistance)
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