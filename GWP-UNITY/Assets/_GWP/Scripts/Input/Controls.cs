// GENERATED AUTOMATICALLY FROM 'Assets/_GWP/InputActions.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @Controls : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @Controls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""InputActions"",
    ""maps"": [
        {
            ""name"": ""Pointer"",
            ""id"": ""b039bc08-cd6b-4c5b-8e17-bebc5d0c2485"",
            ""actions"": [
                {
                    ""name"": ""PointerMoved"",
                    ""type"": ""Value"",
                    ""id"": ""edb3962f-9295-4599-a4af-845d666e6060"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""PointerTappedMain"",
                    ""type"": ""Button"",
                    ""id"": ""1c5f8ff0-f5f5-4059-80cc-21e0879706f6"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""PointerTappedAlternative"",
                    ""type"": ""Button"",
                    ""id"": ""8a3e3626-709e-487b-abd0-1c2702477034"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Scroll"",
                    ""type"": ""Value"",
                    ""id"": ""d8c1e7df-76d1-4af1-85b6-8e74af799d9a"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""1fa36d47-9a4b-4743-8e4a-bfd44abfc500"",
                    ""path"": ""<Pointer>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PointerMoved"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""48489e4e-8dd7-4c3f-a02c-f1b642ed6852"",
                    ""path"": ""<Pointer>/press"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PointerTappedMain"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""19510d3c-3eb2-4d55-99e0-4ebb3ec3ca04"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PointerTappedAlternative"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f9c11acd-3dd9-48b6-8d7a-e4df727c12ab"",
                    ""path"": ""<Mouse>/scroll"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Scroll"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Pointer
        m_Pointer = asset.FindActionMap("Pointer", throwIfNotFound: true);
        m_Pointer_PointerMoved = m_Pointer.FindAction("PointerMoved", throwIfNotFound: true);
        m_Pointer_PointerTappedMain = m_Pointer.FindAction("PointerTappedMain", throwIfNotFound: true);
        m_Pointer_PointerTappedAlternative = m_Pointer.FindAction("PointerTappedAlternative", throwIfNotFound: true);
        m_Pointer_Scroll = m_Pointer.FindAction("Scroll", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // Pointer
    private readonly InputActionMap m_Pointer;
    private IPointerActions m_PointerActionsCallbackInterface;
    private readonly InputAction m_Pointer_PointerMoved;
    private readonly InputAction m_Pointer_PointerTappedMain;
    private readonly InputAction m_Pointer_PointerTappedAlternative;
    private readonly InputAction m_Pointer_Scroll;
    public struct PointerActions
    {
        private @Controls m_Wrapper;
        public PointerActions(@Controls wrapper) { m_Wrapper = wrapper; }
        public InputAction @PointerMoved => m_Wrapper.m_Pointer_PointerMoved;
        public InputAction @PointerTappedMain => m_Wrapper.m_Pointer_PointerTappedMain;
        public InputAction @PointerTappedAlternative => m_Wrapper.m_Pointer_PointerTappedAlternative;
        public InputAction @Scroll => m_Wrapper.m_Pointer_Scroll;
        public InputActionMap Get() { return m_Wrapper.m_Pointer; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PointerActions set) { return set.Get(); }
        public void SetCallbacks(IPointerActions instance)
        {
            if (m_Wrapper.m_PointerActionsCallbackInterface != null)
            {
                @PointerMoved.started -= m_Wrapper.m_PointerActionsCallbackInterface.OnPointerMoved;
                @PointerMoved.performed -= m_Wrapper.m_PointerActionsCallbackInterface.OnPointerMoved;
                @PointerMoved.canceled -= m_Wrapper.m_PointerActionsCallbackInterface.OnPointerMoved;
                @PointerTappedMain.started -= m_Wrapper.m_PointerActionsCallbackInterface.OnPointerTappedMain;
                @PointerTappedMain.performed -= m_Wrapper.m_PointerActionsCallbackInterface.OnPointerTappedMain;
                @PointerTappedMain.canceled -= m_Wrapper.m_PointerActionsCallbackInterface.OnPointerTappedMain;
                @PointerTappedAlternative.started -= m_Wrapper.m_PointerActionsCallbackInterface.OnPointerTappedAlternative;
                @PointerTappedAlternative.performed -= m_Wrapper.m_PointerActionsCallbackInterface.OnPointerTappedAlternative;
                @PointerTappedAlternative.canceled -= m_Wrapper.m_PointerActionsCallbackInterface.OnPointerTappedAlternative;
                @Scroll.started -= m_Wrapper.m_PointerActionsCallbackInterface.OnScroll;
                @Scroll.performed -= m_Wrapper.m_PointerActionsCallbackInterface.OnScroll;
                @Scroll.canceled -= m_Wrapper.m_PointerActionsCallbackInterface.OnScroll;
            }
            m_Wrapper.m_PointerActionsCallbackInterface = instance;
            if (instance != null)
            {
                @PointerMoved.started += instance.OnPointerMoved;
                @PointerMoved.performed += instance.OnPointerMoved;
                @PointerMoved.canceled += instance.OnPointerMoved;
                @PointerTappedMain.started += instance.OnPointerTappedMain;
                @PointerTappedMain.performed += instance.OnPointerTappedMain;
                @PointerTappedMain.canceled += instance.OnPointerTappedMain;
                @PointerTappedAlternative.started += instance.OnPointerTappedAlternative;
                @PointerTappedAlternative.performed += instance.OnPointerTappedAlternative;
                @PointerTappedAlternative.canceled += instance.OnPointerTappedAlternative;
                @Scroll.started += instance.OnScroll;
                @Scroll.performed += instance.OnScroll;
                @Scroll.canceled += instance.OnScroll;
            }
        }
    }
    public PointerActions @Pointer => new PointerActions(this);
    public interface IPointerActions
    {
        void OnPointerMoved(InputAction.CallbackContext context);
        void OnPointerTappedMain(InputAction.CallbackContext context);
        void OnPointerTappedAlternative(InputAction.CallbackContext context);
        void OnScroll(InputAction.CallbackContext context);
    }
}
