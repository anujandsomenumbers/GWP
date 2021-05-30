using System;

public class DragGesture : Gesture<Pointer, Pointer>
{
    private Pointer pointer;

    public DragGesture(GetStartValueDelegate getStartValue) : base(getStartValue) { }

    public override void StartOrUpdate(Pointer pointer, Action<Pointer> onStarted, Action<Pointer> onUpdated)
    {
        if (!hasStarted)
        {
            Pointer startPointer;
            if (!GetStartValue(pointer.pointerId, out startPointer)) { return; }
            float diff = (pointer.position - startPointer.position).sqrMagnitude;
            if (InputUtility.PixelsToInches(diff) >= slopInches)
            {
                this.pointer = pointer;
                hasStarted = true;
                onStarted?.Invoke(pointer);
            }
        }
        else
        {
            if (this.pointer.pointerId != pointer.pointerId) { return; }
            onUpdated?.Invoke(pointer);
        }
    }

    public override void Complete(Pointer pointer, Action<Pointer> onCompleted)
    {
        if (!hasStarted) { return; }
        if (this.pointer.pointerId != pointer.pointerId) { return; }
        onCompleted?.Invoke(pointer);
        hasStarted = false;
    }
}
