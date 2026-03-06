using UnityEngine;

public class WheelUnlocker : MonoBehaviour
{
    [Header("Unlock Settings")]
    public int requiredTurns = 5;

    [Header("References")]
    public Rigidbody wheelRigidbody;
    public MonoBehaviour grabbableScript;

    [Header("State")]
    public int currentTurns = 0;
    public bool wrenchInPlace = false;
    public bool wheelUnlocked = false;

    private WrenchTool activeWrench;

    private void Start()
    {
        LockWheel();
    }

    public void SetWrenchInPlace(bool inPlace, WrenchTool wrench)
    {
        wrenchInPlace = inPlace;

        if (inPlace)
            activeWrench = wrench;
        else if (activeWrench == wrench)
            activeWrench = null;
    }

    public void RegisterTurn()
    {
        Debug.Log("RegisterTurn called | unlocked=" + wheelUnlocked +
              " | wrenchInPlace=" + wrenchInPlace +
              " | activeWrench=" + (activeWrench != null) +
              " | isHeld=" + (activeWrench != null && activeWrench.isHeld) +
              " | currentTurns=" + currentTurns + "/" + requiredTurns);

        if (wheelUnlocked)
        {
            Debug.Log("Turn ignored: wheel already unlocked");
            return;
        }

        if (!wrenchInPlace)
        {
            Debug.Log("Turn ignored: wrench not in place");
            return;
        }

        if (activeWrench == null || !activeWrench.isHeld)
        {
            Debug.Log("Turn ignored: wrench not held");
            return;
        }

        currentTurns++;
        Debug.Log("Wheel turn count: " + currentTurns + " / " + requiredTurns);

        if (currentTurns >= requiredTurns)
        {
            UnlockWheel();
        }
    }

    private void LockWheel()
    {
        if (wheelRigidbody != null)
        {
            wheelRigidbody.isKinematic = true;
            wheelRigidbody.useGravity = false;
        }

        if (grabbableScript != null)
        {
            grabbableScript.enabled = false;
        }
    }

    private void UnlockWheel()
    {
        wheelUnlocked = true;
        Debug.Log("Front wheel unlocked. You can now remove it.");

        if (wheelRigidbody != null)
        {
            wheelRigidbody.isKinematic = false;
            wheelRigidbody.useGravity = true;
        }

        if (grabbableScript != null)
        {
            grabbableScript.enabled = true;
        }
    }
}