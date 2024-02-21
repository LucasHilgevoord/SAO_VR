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

    public void Update() {
        // Raycast hit to check which object is hit
        if (WasLMousePressedThisFrame() && false)
        {

            bool is2D = false;

            if (is2D)
            {
                Vector2 mousePosition = GetMousePos();
                Vector3 mousePosition3D = new Vector3(mousePosition.x, mousePosition.y, Camera.main.transform.position.z);

                // Construct a ray from the camera to the mouse position
                Ray ray = Camera.main.ScreenPointToRay(mousePosition3D);
                Debug.DrawLine(ray.origin, ray.origin + ray.direction * 100f, Color.red, 1f);

                // Perform the 2D raycast
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
                if (hit.collider != null)
                {
                    // If hit, log the name of the collided object
                    Debug.Log(hit.collider.gameObject.name);
                }
                else
                {
                    // If no hit, log "Nothing"
                    Debug.Log("Nothing");
                }
            }
            else
            {
                Ray ray = Camera.main.ScreenPointToRay(GetMousePos());
                Debug.DrawLine(ray.origin, ray.origin + ray.direction * 100f, Color.red, 1f);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.gameObject != null)
                    {
                        Debug.Log(hit.collider.gameObject.name);
                    }
                }
                else
                {
                    Debug.Log("Nothing");
                }
            }
        }
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
