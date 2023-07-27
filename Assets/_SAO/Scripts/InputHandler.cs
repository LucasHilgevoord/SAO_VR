using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public enum ControllerButton {
    A,
    B,
    Joystick,
    Trigger,
}

public class InputHandler : Singleton<InputHandler>
{
    private List<UnityEngine.XR.InputDevice> devices = new List<UnityEngine.XR.InputDevice>();
    private Keyboard _keyboard;

    private static readonly Dictionary<ControllerButton, InputFeatureUsage<bool>> buttonToUsage =
        new Dictionary<ControllerButton, InputFeatureUsage<bool>>
    {
        { ControllerButton.A, UnityEngine.XR.CommonUsages.primaryButton },
        { ControllerButton.B, UnityEngine.XR.CommonUsages.secondaryButton },
        { ControllerButton.Joystick, UnityEngine.XR.CommonUsages.primary2DAxisClick },
        { ControllerButton.Trigger, UnityEngine.XR.CommonUsages.triggerButton }
    };
    internal override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        // Get the event which is called when a device is connected or disconnected
        InputSystem.onDeviceChange += SetDevices;
        SetDevices(null, 0);
    }

    private void SetDevices(UnityEngine.InputSystem.InputDevice device, InputDeviceChange change)
    {
        _keyboard = Keyboard.current;
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Controller, devices);
    }

    public List<UnityEngine.XR.InputDevice> GetDevices() => devices;

    internal bool wasKeyPressedThisFrame(Key key)
    {
        return _keyboard[key].wasPressedThisFrame;
    }

    internal bool WasLMousePressedThisFrame()
    {
        return Mouse.current.leftButton.wasPressedThisFrame;
    }

    internal bool WasControllerJoystickUp()
    {
        return false;
    }

    internal bool WasControllerButtonPressedThisFrame(ControllerButton button)
    {
        InputFeatureUsage<bool> usage;
        if (buttonToUsage.TryGetValue(button, out usage))
        {
            foreach (var device in devices)
            {
                if (device.TryGetFeatureValue(usage, out bool value) && value)
                {
                    return true;
                }
            }
        }
        return false;
    }

    internal IEnumerator CheckControllerButtonWithDelay(ControllerButton button, float delayTime)
    {
        InputFeatureUsage<bool> usage;
        if (buttonToUsage.TryGetValue(button, out usage))
        {
            while (true)
            {
                foreach (var device in devices)
                {
                    if (device.TryGetFeatureValue(usage, out bool value) && value)
                    {
                        // Button was pressed, do something
                        Debug.Log($"Button {button} was pressed.");
                    }
                }

                // Wait for the specified delay before checking again
                yield return new WaitForSeconds(delayTime);
            }
        }
    }

    internal Vector2 GetMousePos()
    {
        return Mouse.current.position.ReadValue();
    }
}
