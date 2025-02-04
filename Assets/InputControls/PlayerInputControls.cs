//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.5.0
//     from Assets/InputControls/PlayerInputControls.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @PlayerInputControls: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerInputControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerInputControls"",
    ""maps"": [
        {
            ""name"": ""Touch"",
            ""id"": ""0d461fb8-1a03-48c4-8cbc-959f5a061933"",
            ""actions"": [
                {
                    ""name"": ""PrimaryTouchPosition"",
                    ""type"": ""Value"",
                    ""id"": ""72e7e984-fcff-43f9-82a6-8f4b1c4dafbf"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""PrimaryTouchContact"",
                    ""type"": ""Button"",
                    ""id"": ""04f33315-48b4-4450-821a-237e51d98dae"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""PrimarySwipeDelta"",
                    ""type"": ""Value"",
                    ""id"": ""7193389a-3605-4a8d-be8f-ecfe3a89c857"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""SecondaryTouchContact"",
                    ""type"": ""Button"",
                    ""id"": ""791457f4-3e48-426b-95fb-8123b820221c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""SecondaryTouchPosition"",
                    ""type"": ""Value"",
                    ""id"": ""b43835f8-e61d-4497-ae63-2b48860aadce"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""690727dc-c4f7-4a9c-80cd-63bddeaf15e9"",
                    ""path"": ""<Pointer>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PrimaryTouchPosition"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b000cb44-9474-4424-a3aa-dc8c8c394b85"",
                    ""path"": ""<Pointer>/press"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PrimaryTouchContact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""65951e91-fde8-42f2-8066-bddef2e0237f"",
                    ""path"": ""<Touchscreen>/primaryTouch/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PrimarySwipeDelta"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1a4788ca-0a14-4c0e-9dde-fff649a7eb0f"",
                    ""path"": ""<Touchscreen>/touch1/press"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SecondaryTouchContact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""64bb9ea5-f3f6-4e24-bd55-e9463a832df5"",
                    ""path"": ""<Touchscreen>/touch1/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SecondaryTouchPosition"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""New action map"",
            ""id"": ""7780af90-471c-4cb1-83e1-0c615ff481db"",
            ""actions"": [
                {
                    ""name"": ""New action"",
                    ""type"": ""Button"",
                    ""id"": ""01d64285-c006-4877-894c-f71e4476efc9"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""f8785172-ff3e-481b-8079-966a40265037"",
                    ""path"": """",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""New action"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Touch
        m_Touch = asset.FindActionMap("Touch", throwIfNotFound: true);
        m_Touch_PrimaryTouchPosition = m_Touch.FindAction("PrimaryTouchPosition", throwIfNotFound: true);
        m_Touch_PrimaryTouchContact = m_Touch.FindAction("PrimaryTouchContact", throwIfNotFound: true);
        m_Touch_PrimarySwipeDelta = m_Touch.FindAction("PrimarySwipeDelta", throwIfNotFound: true);
        m_Touch_SecondaryTouchContact = m_Touch.FindAction("SecondaryTouchContact", throwIfNotFound: true);
        m_Touch_SecondaryTouchPosition = m_Touch.FindAction("SecondaryTouchPosition", throwIfNotFound: true);
        // New action map
        m_Newactionmap = asset.FindActionMap("New action map", throwIfNotFound: true);
        m_Newactionmap_Newaction = m_Newactionmap.FindAction("New action", throwIfNotFound: true);
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

    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }

    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Touch
    private readonly InputActionMap m_Touch;
    private List<ITouchActions> m_TouchActionsCallbackInterfaces = new List<ITouchActions>();
    private readonly InputAction m_Touch_PrimaryTouchPosition;
    private readonly InputAction m_Touch_PrimaryTouchContact;
    private readonly InputAction m_Touch_PrimarySwipeDelta;
    private readonly InputAction m_Touch_SecondaryTouchContact;
    private readonly InputAction m_Touch_SecondaryTouchPosition;
    public struct TouchActions
    {
        private @PlayerInputControls m_Wrapper;
        public TouchActions(@PlayerInputControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @PrimaryTouchPosition => m_Wrapper.m_Touch_PrimaryTouchPosition;
        public InputAction @PrimaryTouchContact => m_Wrapper.m_Touch_PrimaryTouchContact;
        public InputAction @PrimarySwipeDelta => m_Wrapper.m_Touch_PrimarySwipeDelta;
        public InputAction @SecondaryTouchContact => m_Wrapper.m_Touch_SecondaryTouchContact;
        public InputAction @SecondaryTouchPosition => m_Wrapper.m_Touch_SecondaryTouchPosition;
        public InputActionMap Get() { return m_Wrapper.m_Touch; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(TouchActions set) { return set.Get(); }
        public void AddCallbacks(ITouchActions instance)
        {
            if (instance == null || m_Wrapper.m_TouchActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_TouchActionsCallbackInterfaces.Add(instance);
            @PrimaryTouchPosition.started += instance.OnPrimaryTouchPosition;
            @PrimaryTouchPosition.performed += instance.OnPrimaryTouchPosition;
            @PrimaryTouchPosition.canceled += instance.OnPrimaryTouchPosition;
            @PrimaryTouchContact.started += instance.OnPrimaryTouchContact;
            @PrimaryTouchContact.performed += instance.OnPrimaryTouchContact;
            @PrimaryTouchContact.canceled += instance.OnPrimaryTouchContact;
            @PrimarySwipeDelta.started += instance.OnPrimarySwipeDelta;
            @PrimarySwipeDelta.performed += instance.OnPrimarySwipeDelta;
            @PrimarySwipeDelta.canceled += instance.OnPrimarySwipeDelta;
            @SecondaryTouchContact.started += instance.OnSecondaryTouchContact;
            @SecondaryTouchContact.performed += instance.OnSecondaryTouchContact;
            @SecondaryTouchContact.canceled += instance.OnSecondaryTouchContact;
            @SecondaryTouchPosition.started += instance.OnSecondaryTouchPosition;
            @SecondaryTouchPosition.performed += instance.OnSecondaryTouchPosition;
            @SecondaryTouchPosition.canceled += instance.OnSecondaryTouchPosition;
        }

        private void UnregisterCallbacks(ITouchActions instance)
        {
            @PrimaryTouchPosition.started -= instance.OnPrimaryTouchPosition;
            @PrimaryTouchPosition.performed -= instance.OnPrimaryTouchPosition;
            @PrimaryTouchPosition.canceled -= instance.OnPrimaryTouchPosition;
            @PrimaryTouchContact.started -= instance.OnPrimaryTouchContact;
            @PrimaryTouchContact.performed -= instance.OnPrimaryTouchContact;
            @PrimaryTouchContact.canceled -= instance.OnPrimaryTouchContact;
            @PrimarySwipeDelta.started -= instance.OnPrimarySwipeDelta;
            @PrimarySwipeDelta.performed -= instance.OnPrimarySwipeDelta;
            @PrimarySwipeDelta.canceled -= instance.OnPrimarySwipeDelta;
            @SecondaryTouchContact.started -= instance.OnSecondaryTouchContact;
            @SecondaryTouchContact.performed -= instance.OnSecondaryTouchContact;
            @SecondaryTouchContact.canceled -= instance.OnSecondaryTouchContact;
            @SecondaryTouchPosition.started -= instance.OnSecondaryTouchPosition;
            @SecondaryTouchPosition.performed -= instance.OnSecondaryTouchPosition;
            @SecondaryTouchPosition.canceled -= instance.OnSecondaryTouchPosition;
        }

        public void RemoveCallbacks(ITouchActions instance)
        {
            if (m_Wrapper.m_TouchActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(ITouchActions instance)
        {
            foreach (var item in m_Wrapper.m_TouchActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_TouchActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public TouchActions @Touch => new TouchActions(this);

    // New action map
    private readonly InputActionMap m_Newactionmap;
    private List<INewactionmapActions> m_NewactionmapActionsCallbackInterfaces = new List<INewactionmapActions>();
    private readonly InputAction m_Newactionmap_Newaction;
    public struct NewactionmapActions
    {
        private @PlayerInputControls m_Wrapper;
        public NewactionmapActions(@PlayerInputControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Newaction => m_Wrapper.m_Newactionmap_Newaction;
        public InputActionMap Get() { return m_Wrapper.m_Newactionmap; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(NewactionmapActions set) { return set.Get(); }
        public void AddCallbacks(INewactionmapActions instance)
        {
            if (instance == null || m_Wrapper.m_NewactionmapActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_NewactionmapActionsCallbackInterfaces.Add(instance);
            @Newaction.started += instance.OnNewaction;
            @Newaction.performed += instance.OnNewaction;
            @Newaction.canceled += instance.OnNewaction;
        }

        private void UnregisterCallbacks(INewactionmapActions instance)
        {
            @Newaction.started -= instance.OnNewaction;
            @Newaction.performed -= instance.OnNewaction;
            @Newaction.canceled -= instance.OnNewaction;
        }

        public void RemoveCallbacks(INewactionmapActions instance)
        {
            if (m_Wrapper.m_NewactionmapActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(INewactionmapActions instance)
        {
            foreach (var item in m_Wrapper.m_NewactionmapActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_NewactionmapActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public NewactionmapActions @Newactionmap => new NewactionmapActions(this);
    public interface ITouchActions
    {
        void OnPrimaryTouchPosition(InputAction.CallbackContext context);
        void OnPrimaryTouchContact(InputAction.CallbackContext context);
        void OnPrimarySwipeDelta(InputAction.CallbackContext context);
        void OnSecondaryTouchContact(InputAction.CallbackContext context);
        void OnSecondaryTouchPosition(InputAction.CallbackContext context);
    }
    public interface INewactionmapActions
    {
        void OnNewaction(InputAction.CallbackContext context);
    }
}
