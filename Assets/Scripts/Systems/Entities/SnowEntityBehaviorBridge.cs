using UnityEngine;
using UnityEngine.AI;

public class SnowEntityBehaviourBridge : MonoBehaviour
{
    [Header("References")]
    public SnowEntityContext context;
    public NavMeshAgent agent;

    [Header("Movement")]
    public float reachedDistance = 0.75f;
    public int wanderSampleAttempts = 12;

    [Header("Debug")]
    public bool debugBridge;

    private void Awake()
    {
        ResolveReferences();
    }

    private void Reset()
    {
        ResolveReferences();
    }

    public bool IsCaptured()
    {
        return context && context.isCaptured;
    }

    public bool CanMove()
    {
        if (!context)
        {
            return false;
        }

        if (context.isCaptured)
        {
            return false;
        }

        if (!context.canMove)
        {
            return false;
        }

        if (!agent)
        {
            return false;
        }

        return agent.enabled && agent.isOnNavMesh;
    }

    public bool HasTargetPosition()
    {
        return context && context.hasTargetPosition;
    }

    public void ClearTargetPosition()
    {
        if (context)
        {
            context.ClearTargetPosition();
        }
    }

    public bool PickRandomWanderPosition()
    {
        if (!context)
        {
            return false;
        }

        Vector3 origin = context.GetWanderOriginPosition();
        float radius = Mathf.Max(1f, context.wanderRadius);

        for (int i = 0; i < wanderSampleAttempts; i++)
        {
            Vector2 randomCircle = Random.insideUnitCircle * radius;
            Vector3 candidate = origin + new Vector3(randomCircle.x, 0f, randomCircle.y);

            if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, radius, NavMesh.AllAreas))
            {
                context.SetTargetPosition(hit.position);

                if (debugBridge)
                {
                    Debug.Log($"[SnowEntityBehaviourBridge] Picked wander target: {hit.position}");
                }

                return true;
            }
        }

        if (debugBridge)
        {
            Debug.LogWarning("[SnowEntityBehaviourBridge] Failed to pick wander target.");
        }

        return false;
    }

    public bool MoveToCurrentTarget()
    {
        if (!CanMove())
        {
            return false;
        }

        if (!context.hasTargetPosition)
        {
            return false;
        }

        agent.SetDestination(context.currentTargetPosition);
        return true;
    }

    public bool HasReachedTarget()
    {
        if (!agent || !agent.enabled || !agent.isOnNavMesh)
        {
            return false;
        }

        if (agent.pathPending)
        {
            return false;
        }

        if (agent.remainingDistance > reachedDistance)
        {
            return false;
        }

        return true;
    }

    public void StopMovement()
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
    }

    private void ResolveReferences()
    {
        if (!context)
        {
            context = GetComponent<SnowEntityContext>();
        }

        if (!agent)
        {
            agent = GetComponent<NavMeshAgent>();
        }
    }
}