using UnityEngine;

public static class AnimationCurveExtensions
{
    public static float Duration(this AnimationCurve curve) => curve[curve.length - 1].time;
}