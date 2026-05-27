using UnityEngine;
using UnityEngine.AI;

public class SnowEntityContext : MonoBehaviour, ITrapCaptureResponder
{
    [Header("Entity State")]
    public bool isCaptured;
    public bool canMove = true;

    [Header("Movement")]
    public NavMeshAgent agent;
    public Transform wanderOrigin;
    public float wanderRadius = 12f;

    [Header("Targeting")]
    public bool hasTargetPosition;
    public Vector3 currentTargetPosition;

    [Header("Capture")]
    public PlacedSnowTrap capturedByTrap;
    public TrapCaptureTarget captureTarget;

    [Header("Debug")]
    public bool debugEntity = true;

    private void Awake()
    {
        ResolveReferences();
    }

    private void Reset()
    {
        ResolveReferences();
    }

    private void Start()
    {
        if (!wanderOrigin)
        {
            wanderOrigin = transform;
        }

        ApplyMovementState();
    }

    public void SetTargetPosition(Vector3 position)
    {
        if (isCaptured)
        {
            return;
        }

        currentTargetPosition = position;
        hasTargetPosition = true;
    }

    public void ClearTargetPosition()
    {
        hasTargetPosition = false;
        currentTargetPosition = Vector3.zero;
    }

    public void SetCanMove(bool value)
    {
        canMove = value;
        ApplyMovementState();
    }

    public void OnCapturedByTrap(PlacedSnowTrap trap)
    {
        isCaptured = true;
        canMove = false;
        capturedByTrap = trap;

        ClearTargetPosition();
        StopMovement();

        if (debugEntity)
        {
            Debug.Log($"[SnowEntityContext] {name} captured by {trap.name}.");
        }
    }

    public Vector3 GetWanderOriginPosition()
    {
        if (wanderOrigin)
        {
            return wanderOrigin.position;
        }

        return transform.position;
    }

    public bool IsAvailableForBehaviour()
    {
        return !isCaptured && canMove;
    }

    private void ResolveReferences()
    {
        if (!agent)
        {
            agent = GetComponent<NavMeshAgent>();
        }

        if (!captureTarget)
        {
            captureTarget = GetComponent<TrapCaptureTarget>();
        }

        if (!captureTarget)
        {
            captureTarget = GetComponentInChildren<TrapCaptureTarget>();
        }
    }

    private void ApplyMovementState()
    {
        if (!agent)
        {
            return;
        }

        agent.enabled = canMove && !isCaptured;
    }

    private void StopMovement()
    {
        if (!agent)
        {
            return;
        }

        if (agent.enabled && agent.isOnNavMesh)
        {
            agent.ResetPath();
            agent.velocity = Vector3.zero;
        }

        agent.enabled = false;
    }
}