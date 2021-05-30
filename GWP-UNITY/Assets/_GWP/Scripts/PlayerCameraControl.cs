using UnityEngine;

public class PlayerCameraControl
{
    public bool Enabled 
    { 
        get => _enabled;
        set 
        {
            if (value && !_enabled) Subscribe();
            else if (!value && _enabled) Unsubscribe();

            _enabled = value;
        } 
    }
    private bool _enabled = true;

    public float zoomSpeedMultiplier = 1f;

    private bool isZooming = false;
    private int? dragPointerId;
    private Vector3 lastPointerPos;
    private Vector3 lastPointerPos2;
    private Vector3 initialPointerPos;
    private Vector3 initialCamPos;
    private float depth = 10f;
    private Vector3 initialZoomPtrPosition;

    private readonly CameraMover cameraMover;
    private readonly GestureRecognizer gestures;

    public PlayerCameraControl(CameraMover cameraMover, GestureRecognizer gestureRecognizer)
    {
        gestures = gestureRecognizer;
        this.cameraMover = cameraMover;

        Subscribe();
    }

    public void Dispose() => Unsubscribe();
    public void Expose() => Subscribe();

    private void OnZoomStarted(Zoom zoom)
    {
        // Cancel & prevent drag
        dragPointerId = null;
        isZooming = true;

        // Start zoom
        initialZoomPtrPosition = cameraMover.cam.ScreenToWorldPoint(zoom.position);

        initialCamPos = cameraMover.transform.position;
        cameraMover.movementMode = CameraMover.MovementModes.Direct;
    }

    private void OnZoomChanged(Zoom zoom)
    {
        if (!isZooming) return;
        cameraMover.TargetSize += zoom.amount * zoomSpeedMultiplier;
        cameraMover.UpdateZoom();

        Vector3 dir = cameraMover.cam.ScreenToWorldPoint(zoom.position) - initialZoomPtrPosition;
        cameraMover.TargetPos = cameraMover.transform.position - dir;
    }

    private void OnZoomCompleted(Zoom zoom)
    {
        isZooming = false;
        SetCamVelocity(Vector3.zero);
    }

    private void OnDragStarted(Pointer pointer)
    {
        if (!isZooming && null != dragPointerId) return;
        if (_App.Instance.Input.AltPointerId != pointer.pointerId) return;
        dragPointerId = pointer.pointerId;
        initialCamPos = cameraMover.transform.position;
        initialPointerPos = lastPointerPos = lastPointerPos2 = pointer.position;
        initialPointerPos.z = depth;
        lastPointerPos.z = depth;
        lastPointerPos2.z = depth;
    }

    private void OnDragMoved(Pointer pointer)
    {
        if (null == dragPointerId) return;
        Vector3 pos = pointer.position;
        pos.z = depth;
        cameraMover.TargetPos =
            cameraMover.cam.ScreenToWorldPoint(initialPointerPos)
            - cameraMover.cam.ScreenToWorldPoint(pos)
            + initialCamPos;
        lastPointerPos2 = lastPointerPos;
        lastPointerPos = pos;
        cameraMover.movementMode = CameraMover.MovementModes.Direct;
    }

    private void OnDragCompleted(Pointer pointer)
    {
        if (null == dragPointerId) return;
        dragPointerId = null;

        if (Vector2.Distance(lastPointerPos2, lastPointerPos) > 1f)
        {
            SetCamVelocity(
                (cameraMover.cam.ScreenToWorldPoint(lastPointerPos2)
                - cameraMover.cam.ScreenToWorldPoint(lastPointerPos)) / Time.deltaTime);
        }
        else
        {
            SetCamVelocity(Vector3.zero);
        }
    }

    private void SetCamVelocity(Vector3 velocity)
    {
        cameraMover.Velocity = velocity;
        cameraMover.movementMode = CameraMover.MovementModes.Inertia;
    }

    private void Subscribe()
    {
        gestures.DragStarted += OnDragStarted;
        gestures.DragMoved += OnDragMoved;
        gestures.DragCompleted += OnDragCompleted;

        gestures.ZoomStarted += OnZoomStarted;
        gestures.ZoomChanged += OnZoomChanged;
        gestures.ZoomCompleted += OnZoomCompleted;
    }

    private void Unsubscribe()
    {
        gestures.DragStarted -= OnDragStarted;
        gestures.DragMoved -= OnDragMoved;
        gestures.DragCompleted -= OnDragCompleted;

        gestures.ZoomStarted -= OnZoomStarted;
        gestures.ZoomChanged -= OnZoomChanged;
        gestures.ZoomCompleted -= OnZoomCompleted;
    }
}
