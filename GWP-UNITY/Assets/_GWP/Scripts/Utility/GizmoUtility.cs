#if UNITY_EDITOR
using UnityEngine;
using System;

public static class GizmoUtility
{
    public static void DrawRect(Rect rect, Quaternion rotation)
    {
        Vector3 bl = rotation * new Vector3(rect.xMin, rect.yMin);
        Vector3 tl = rotation * new Vector3(rect.xMin, rect.yMax);
        Vector3 br = rotation * new Vector3(rect.xMax, rect.yMin);
        Vector3 tr = rotation * new Vector3(rect.xMax, rect.yMax);

        Gizmos.DrawLine(bl, tl);
        Gizmos.DrawLine(tl, tr);
        Gizmos.DrawLine(tr, br);
        Gizmos.DrawLine(br, bl);
    }

    public static void DrawPath(Func<int, Vector3> getPoint, int numPoints)
    {
        for (int i = 1; i < numPoints; i++)
        {
            Gizmos.DrawLine(getPoint(i - 1), getPoint(i));
        }
    }
}
#endif