using UnityEngine;

public interface ITrapCaptureResponder
{
    void OnCapturedByTrap(PlacedSnowTrap trap);
}

public class TrapCaptureTarget : MonoBehaviour
{
    [Header("Capture State")]
    public bool isCaptured;

    [Header("Optional Components")]
    public Rigidbody targetRigidbody;
    public Collider[] targetColliders;

    [Header("Capture Behaviour")]
    public bool freezeRigidbodyOnCapture = true;
    public bool notifyResponders = true;

    private void Awake()
    {
        RefreshReferences();
    }

    private void Reset()
    {
        RefreshReferences();
    }

    public bool CanBeCaptured()
    {
        return !isCaptured;
    }

    public void Capture(PlacedSnowTrap trap)
    {
        if (isCaptured)
        {
            return;
        }

        isCaptured = true;

        if (freezeRigidbodyOnCapture && targetRigidbody)
        {
#if UNITY_6000_0_OR_NEWER
            targetRigidbody.linearVelocity = Vector3.zero;
#else
            targetRigidbody.velocity = Vector3.zero;
#endif
            targetRigidbody.angularVelocity = Vector3.zero;
            targetRigidbody.isKinematic = true;
        }

        if (notifyResponders)
        {
            NotifyCaptureResponders(trap);
        }

        Debug.Log($"[TrapCaptureTarget] {name} captured by {trap.name}.");
    }

    public void RefreshReferences()
    {
        if (!targetRigidbody)
        {
            targetRigidbody = GetComponent<Rigidbody>();
        }

        if (targetColliders == null || targetColliders.Length == 0)
        {
            targetColliders = GetComponentsInChildren<Collider>();
        }
    }

    private void NotifyCaptureResponders(PlacedSnowTrap trap)
    {
        ITrapCaptureResponder[] responders = GetComponentsInParent<ITrapCaptureResponder>();

        for (int i = 0; i < responders.Length; i++)
        {
            responders[i].OnCapturedByTrap(trap);
        }

        ITrapCaptureResponder[] childResponders = GetComponentsInChildren<ITrapCaptureResponder>();

        for (int i = 0; i < childResponders.Length; i++)
        {
            if (HasAlreadyNotified(responders, childResponders[i]))
            {
                continue;
            }

            childResponders[i].OnCapturedByTrap(trap);
        }
    }

    private bool HasAlreadyNotified(ITrapCaptureResponder[] responders, ITrapCaptureResponder responder)
    {
        for (int i = 0; i < responders.Length; i++)
        {
            if (responders[i] == responder)
            {
                return true;
            }
        }

        return false;
    }
}