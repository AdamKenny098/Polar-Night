using UnityEngine;

public class TrapCaptureTarget : MonoBehaviour
{
    [Header("Capture State")]
    public bool isCaptured;

    [Header("Optional Components")]
    public Rigidbody targetRigidbody;
    public Collider[] targetColliders;

    private void Awake()
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

        if (targetRigidbody)
        {
            targetRigidbody.linearVelocity = Vector3.zero;
            targetRigidbody.angularVelocity = Vector3.zero;
            targetRigidbody.isKinematic = true;
        }

        Debug.Log($"[TrapCaptureTarget] {name} captured by {trap.name}.");
    }
}