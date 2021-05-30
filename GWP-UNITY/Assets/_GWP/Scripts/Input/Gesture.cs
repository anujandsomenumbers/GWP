using System;

/// <summary>
/// Base class for input gestures of any kind. Gestures interpret device input as specified user actions.
/// </summary>
/// <typeparam name="TIn">Input type for the gesture's StartOrUpdate and Complete methods.</typeparam>
/// <typeparam name="TOut">Output type used as a parameter in the Started/Updateed/Completed callbacks.</typeparam>
public abstract class Gesture<TIn, TOut>
{
    protected bool hasStarted = false;

    public delegate bool GetStartValueDelegate(int index, out TIn startValue);
    protected readonly GetStartValueDelegate GetStartValue;

    protected const float slopInches = 0.1f;

    public Gesture(GetStartValueDelegate getStartValue)
    {
        GetStartValue = getStartValue;
    }

    public abstract void StartOrUpdate(TIn currentValue, Action<TOut> onStarted, Action<TOut> onUpdated);
    public abstract void Complete(TIn currentValue, Action<TOut> onCompleted);
}
