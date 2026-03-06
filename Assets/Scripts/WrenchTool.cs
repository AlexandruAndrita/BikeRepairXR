using UnityEngine;
using UnityEngine.InputSystem;

public class WrenchTool : MonoBehaviour
{
    [Header("Input")]
    public Key turnKey = Key.Backspace;

    [Header("Hold Detection")]
    public bool isHeld = false;

    private WheelUnlocker currentWheel;

    private void Update()
    {
        if (Keyboard.current == null)
        {
            Debug.Log("No keyboard detected");
            return;
        }

        if (Keyboard.current.anyKey.wasPressedThisFrame)
        {
            Debug.Log("SOME KEY WAS PRESSED");
        }

        bool pressed = false;

        try
        {
            pressed = Keyboard.current[turnKey].wasPressedThisFrame;
        }
        catch
        {
            Debug.LogWarning("Invalid Turn Key in Inspector. Re-select Backspace.");
            return;
        }

        if (pressed)
        {
            Debug.Log("KEY PRESSED -> " + turnKey +
                      " | isHeld=" + isHeld +
                      " | currentWheel=" + (currentWheel != null));
        }

        if (isHeld && currentWheel != null && pressed)
        {
            Debug.Log("Sending turn to wheel...");
            currentWheel.RegisterTurn();
        }
    }

    public void SetHeld(bool held)
    {
        isHeld = held;
        Debug.Log("Wrench isHeld = " + isHeld);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Wrench entered trigger: " + other.name);

        WheelUnlocker wheel = other.GetComponentInParent<WheelUnlocker>();
        if (wheel != null)
        {
            currentWheel = wheel;
            wheel.SetWrenchInPlace(true, this);
            Debug.Log("Wrench is in place");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Wrench exited trigger: " + other.name);

        WheelUnlocker wheel = other.GetComponentInParent<WheelUnlocker>();
        if (wheel != null && wheel == currentWheel)
        {
            wheel.SetWrenchInPlace(false, this);
            currentWheel = null;
        }
    }
}