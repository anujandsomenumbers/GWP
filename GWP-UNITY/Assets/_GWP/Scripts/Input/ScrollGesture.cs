using System;
using UnityEngine;

public class ScrollGesture : Gesture<float, Zoom>
{
    public Vector2 Position { get; set; }

    private float lastUpdateTime;
    
    private const float scrollSpeedMultiplier = -0.01f;
    private const float completionDelay = 0.25f;

    public ScrollGesture(GetStartValueDelegate getStartValue) : base(getStartValue) { }

    public override void Complete(float rawScroll, Action<Zoom> onCompleted)
    {
        if (!hasStarted) return;
        if (Time.time - lastUpdateTime < completionDelay) return;
        hasStarted = false;
        onCompleted?.Invoke(MakeZoom(rawScroll));
    }

    public override void StartOrUpdate(float rawScrollDelta, Action<Zoom> onStarted, Action<Zoom> onUpdated)
    {
        if (TryStart(rawScrollDelta))
        {
            onStarted?.Invoke(MakeZoom(rawScrollDelta));
            return;
        }
        else
        {
            onUpdated?.Invoke(MakeZoom(rawScrollDelta));
        }

        lastUpdateTime = Time.time;
    }

    private bool TryStart(float rawScrollDelta)
    {
        if (hasStarted) { return false; }

        hasStarted = true;
        return true;
    }

    private Zoom MakeZoom(float rawScrollDelta)
    {
        return new Zoom
        {
            amount = rawScrollDelta * scrollSpeedMultiplier,
            position = Position
        };
    }
}
