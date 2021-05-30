using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using PointerDevice = UnityEngine.InputSystem.Pointer;

[DisallowMultipleComponent]
public class InputManager : MonoBehaviour, IInputManager, Controls.IPointerActions
{
    public bool IsOverUI => null != EventSystem.current && EventSystem.current.IsPointerOverGameObject();
    public bool IsPointerOnScreen
    {
        get 
        {
            Vector2 viewportPoint = InteractionCam.ScreenToViewportPoint(PointerDevice.current.position.ReadValue());
            return viewportPoint.x > 0 && viewportPoint.x < 1 && viewportPoint.y > 0 && viewportPoint.y < 1;
        } 
    }

    public Camera InteractionCam { get; private set; }
    public int AltPointerId => altPointerId;

    public event Action<Pointer> PointerPressed;
    public event Action<Vector2> PointerMoved;
    public event Action<Vector2> PointerStationary;
    public event Action<Pointer> PointerReleased;
    public event Action<float> MouseScroll;

    private bool wasPointerMoved = false;
    private bool wasScrolled = false;

    private const float pointerRadiusInches = 0.25f;
    private const int mainPointerId = 0;
    private const int altPointerId = 1;

    public void Initialize(_App app, Action onInitialized)
    {
        var controls = new Controls();
        controls.Enable();
        controls.Pointer.SetCallbacks(this);

        InteractionCam = Camera.main;
        app.Scene.SceneLoadCompleted += OnSceneLoaded;
        onInitialized?.Invoke();
    }

    public float PointerRadiusWS(Camera cam)
    {
        if (!cam) cam = InteractionCam;
        return cam.PhysicalToWorldDistance(pointerRadiusInches);
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene) 
        => InteractionCam = Camera.main;

    private void Update()
    {
        if (!wasPointerMoved)
        {
            PointerStationary?.Invoke(PointerDevice.current.position.ReadValue());
        }
        else
        {
            wasPointerMoved = false;
        }

        if (!wasScrolled)
        {
            MouseScroll?.Invoke(0);
        }
        else
        {
            wasScrolled = false;
        }
    }

    public void OnPointerMoved(InputAction.CallbackContext context)
    {
        wasPointerMoved = true;
        PointerMoved?.Invoke(context.ReadValue<Vector2>());
    }

    public void OnPointerTappedMain(InputAction.CallbackContext context)
    {
        OnPointerTapped(context, mainPointerId);
    }

    public void OnPointerTappedAlternative(InputAction.CallbackContext context) 
    {
        OnPointerTapped(context, altPointerId);
    }

    public void OnScroll(InputAction.CallbackContext context)
    {
        if (IsOverUI) return;
        MouseScroll?.Invoke(context.ReadValue<Vector2>().y);
        wasScrolled = true;
    }

    private void OnPointerTapped(InputAction.CallbackContext context, int id)
    {
        var device = InputSystem.devices.First(
            d => d.path == context.control.device.path) as PointerDevice;
        Pointer pointer = new Pointer()
        {
            pointerId = id,
            position = device.position.ReadValue()
        };

        if (!IsOverUI && IsPointerOnScreen && context.started) PointerPressed?.Invoke(pointer);
        else if (context.canceled) PointerReleased?.Invoke(pointer);
    }
}
