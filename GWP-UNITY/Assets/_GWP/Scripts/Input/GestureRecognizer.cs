using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for gesture recognizer. Uses gestures to convert raw input into user actions that can be used in gameplay code.
/// </summary>
public class GestureRecognizer
{
    #region Events
    public event Action<Pointer> DragStarted;
    public event Action<Pointer> DragMoved;
    public event Action<Pointer> DragCompleted;

    public event Action<Zoom> ZoomStarted;
    public event Action<Zoom> ZoomChanged;
    public event Action<Zoom> ZoomCompleted;

    public event Action<Pointer> TapCompleted;
    public event Action<Pointer> LongTapCompleted;

    #endregion

    protected IInputManager input;

    #region Gestures
    protected DragGesture dragGesture;
    protected ScrollGesture scrollGesture;
    protected TapGesture tapGesture;
    protected TapGesture longTapGesture; 
    #endregion

    private Dictionary<int, Pointer> pointers = new Dictionary<int, Pointer>();

    protected const float longTapThreshold = 0.5f;

    public GestureRecognizer(IInputManager input)
    {
        this.input = input;
        input.PointerPressed += OnPointerPressed;
        input.PointerMoved += OnPointerMoved;
        input.PointerStationary += OnPointerStationary;
        input.PointerReleased += OnPointerReleased;
        input.MouseScroll += OnMouseScroll;

        InitializeGestures();
    }

    public void Dispose()
    {
        if (null == input) { return; }
        input.PointerPressed -= OnPointerPressed;
        input.PointerMoved -= OnPointerMoved;
        input.PointerStationary -= OnPointerStationary;
        input.PointerReleased -= OnPointerReleased;
        input.MouseScroll -= OnMouseScroll;
    }

    private void OnPointerPressed(Pointer pointer)
    {
        if (!pointers.ContainsKey(pointer.pointerId))
        {
            pointers.Add(pointer.pointerId, pointer);
        }
        else
        {
            pointers[pointer.pointerId] = pointer;
        }

        tapGesture.StartOrUpdate(pointer, null, null);
        longTapGesture.StartOrUpdate(pointer, null, null);
    }

    private void OnPointerMoved(Vector2 position)
    {
        scrollGesture.Position = position;

        foreach (var startPointer in pointers.Values)
        {
            Pointer pointer = new Pointer()
            {
                pointerId = startPointer.pointerId,
                position = position
            };

            dragGesture.StartOrUpdate(pointer, DragStarted, DragMoved);
            tapGesture.StartOrUpdate(pointer, null, null);
            longTapGesture.StartOrUpdate(pointer, null, null);
        }
    }

    private void OnPointerStationary(Vector2 position)
    {
        foreach (var startPointer in pointers.Values)
        {
            Pointer pointer = new Pointer()
            {
                pointerId = startPointer.pointerId,
                position = position
            };

            tapGesture.StartOrUpdate(pointer, null, null);
            longTapGesture.StartOrUpdate(pointer, null, null);
        }
    }

    private void OnPointerReleased(Pointer pointer)
    {
        if (!pointers.ContainsKey(pointer.pointerId)) { return; }

        dragGesture.Complete(pointer, DragCompleted);
        tapGesture.Complete(pointer, TapCompleted);
        longTapGesture.Complete(pointer, LongTapCompleted);

        pointers.Remove(pointer.pointerId);
    }

    private void OnMouseScroll(float rawScrollDelta)
    {
        if (Mathf.Epsilon <= Mathf.Abs(rawScrollDelta))
        {
            scrollGesture.StartOrUpdate(rawScrollDelta, ZoomStarted, ZoomChanged);
        }
        else
        {
            scrollGesture.Complete(rawScrollDelta, ZoomCompleted);
        }
    }

    private void InitializeGestures()
    {
        dragGesture = new DragGesture(GetPointer);
        scrollGesture = new ScrollGesture((int i, out float s) => { s = 0; return true; });
        tapGesture = new TapGesture(GetPointer) { maxDuration = longTapThreshold };
        longTapGesture = new TapGesture(GetPointer) { minDuration = longTapThreshold };
    }

    private bool GetPointer(int id, out Pointer pointer)
    {
        return pointers.TryGetValue(id, out pointer);
    }
}
