using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.XR;

public class InputHandler : Singleton<InputHandler>
{
    private List<UnityEngine.XR.InputDevice> devices = new List<UnityEngine.XR.InputDevice>();
    private Keyboard _keyboard;

    internal override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        GetDevices();
    }

    private void GetDevices()
    {
        _keyboard = Keyboard.current;
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Controller, devices);
    }

    internal bool wasKeyPressedThisFrame(Key key)
    {
        return _keyboard[key].wasPressedThisFrame;
    }
}
