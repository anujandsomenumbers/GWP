using System;
using UnityEngine;

public interface IInputManager : IManager
{
    bool IsOverUI { get; }
    Camera InteractionCam { get; }
    int AltPointerId { get; }

    float PointerRadiusWS(Camera cam = null);

    event Action<Pointer> PointerPressed;
    event Action<Vector2> PointerMoved;
    event Action<Vector2> PointerStationary;
    event Action<Pointer> PointerReleased;
    event Action<float> MouseScroll;
}
