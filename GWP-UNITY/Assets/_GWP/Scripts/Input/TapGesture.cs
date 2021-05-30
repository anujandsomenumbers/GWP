using System;
using UnityEngine;

public class TapGesture : Gesture<Pointer, Pointer>
{
    public float minDuration = 0;
    public float maxDuration = Mathf.Infinity;
    
    private Pointer pointer;
    private bool wasCancelled = false;
    private float duration = 0;

    public TapGesture(GetStartValueDelegate getStartValue) : base(getStartValue) { }

    public override void StartOrUpdate(Pointer pointer, Action<Pointer> onStarted, Action<Pointer> onUpdated)
    {
        if (!hasStarted && !wasCancelled)
        {
            Start(pointer, onStarted);
        }
        else
        {
            Update(pointer, onUpdated);
        }
    }

    public override void Complete(Pointer pointer, Action<Pointer> onCompleted)
    {
        wasCancelled = false;

        if (!hasStarted) { Reset(); return; }
        if (this.pointer.pointerId != pointer.pointerId) { Reset(); return; }
        if (duration < minDuration) { Reset(); return; }

        onCompleted?.Invoke(pointer);

        Reset();
    }

    private void Start(Pointer pointer, Action<Pointer> onStarted)
    {
        this.pointer = pointer;
        hasStarted = true;
        onStarted?.Invoke(pointer);
    }

    private void Update(Pointer pointer, Action<Pointer> onUpdated)
    {
        if (this.pointer.pointerId != pointer.pointerId) { return; }
        if (!GetStartValue(pointer.pointerId, out Pointer startPointer)) { return; }

        float diff = (pointer.position - startPointer.position).magnitude;

        if ((InputUtility.PixelsToInches(diff) >= slopInches))
        {
            Cancel();
        }
        else
        {
            duration += Time.deltaTime;
            if (duration > maxDuration)
            {
                Cancel();
            }
            else
            {
                this.pointer = pointer;
                onUpdated?.Invoke(pointer);
            }
        }
    }

    private void Cancel()
    {
        wasCancelled = true;
        Reset();
    }

    private void Reset()
    {
        hasStarted = false;
        duration = 0;
    }
}
