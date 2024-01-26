//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.7.0
//     from Assets/Other/PlayerInput.inputactions
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

public partial class @PlayerInput: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerInput()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerInput"",
    ""maps"": [
        {
            ""name"": ""PlayerInputActionMaps"",
            ""id"": ""87496760-8c2b-4d7a-9ec6-e5e65dc0d9a0"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""PassThrough"",
                    ""id"": ""319cadc1-aace-4e12-9b2f-47d5bd3c6035"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": []
        }
    ],
    ""controlSchemes"": []
}");
        // PlayerInputActionMaps
        m_PlayerInputActionMaps = asset.FindActionMap("PlayerInputActionMaps", throwIfNotFound: true);
        m_PlayerInputActionMaps_Move = m_PlayerInputActionMaps.FindAction("Move", throwIfNotFound: true);
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

    // PlayerInputActionMaps
    private readonly InputActionMap m_PlayerInputActionMaps;
    private List<IPlayerInputActionMapsActions> m_PlayerInputActionMapsActionsCallbackInterfaces = new List<IPlayerInputActionMapsActions>();
    private readonly InputAction m_PlayerInputActionMaps_Move;
    public struct PlayerInputActionMapsActions
    {
        private @PlayerInput m_Wrapper;
        public PlayerInputActionMapsActions(@PlayerInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_PlayerInputActionMaps_Move;
        public InputActionMap Get() { return m_Wrapper.m_PlayerInputActionMaps; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerInputActionMapsActions set) { return set.Get(); }
        public void AddCallbacks(IPlayerInputActionMapsActions instance)
        {
            if (instance == null || m_Wrapper.m_PlayerInputActionMapsActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_PlayerInputActionMapsActionsCallbackInterfaces.Add(instance);
            @Move.started += instance.OnMove;
            @Move.performed += instance.OnMove;
            @Move.canceled += instance.OnMove;
        }

        private void UnregisterCallbacks(IPlayerInputActionMapsActions instance)
        {
            @Move.started -= instance.OnMove;
            @Move.performed -= instance.OnMove;
            @Move.canceled -= instance.OnMove;
        }

        public void RemoveCallbacks(IPlayerInputActionMapsActions instance)
        {
            if (m_Wrapper.m_PlayerInputActionMapsActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IPlayerInputActionMapsActions instance)
        {
            foreach (var item in m_Wrapper.m_PlayerInputActionMapsActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_PlayerInputActionMapsActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public PlayerInputActionMapsActions @PlayerInputActionMaps => new PlayerInputActionMapsActions(this);
    public interface IPlayerInputActionMapsActions
    {
        void OnMove(InputAction.CallbackContext context);
    }
}
