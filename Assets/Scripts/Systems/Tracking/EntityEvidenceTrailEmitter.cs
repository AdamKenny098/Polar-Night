using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EntityEvidenceSpawnEntry
{
    public GameObject prefab;
    [Min(0f)] public float weight = 1f;
}

public class EntityEvidenceTrailEmitter : MonoBehaviour
{
    [Header("References")]
    public SnowEntityContext context;

    [Header("Evidence Prefabs")]
    public List<EntityEvidenceSpawnEntry> evidencePrefabs = new List<EntityEvidenceSpawnEntry>();

    [Header("Fallback")]
    public GameObject footprintPrefab;

    [Header("Emission")]
    public bool emitEvidence = true;
    public float emitInterval = 1.25f;
    public float minMoveDistance = 0.75f;
    public int maxActiveMarkers = 45;

    [Header("Placement")]
    public LayerMask groundMask = ~0;
    public float raycastHeight = 2f;
    public float raycastDistance = 5f;
    public float surfaceOffset = 0.025f;

    [Header("Footprint Shape")]
    public float sideOffset = 0.22f;
    public float backOffset = 0.35f;
    public float randomYaw = 12f;

    [Header("Debug")]
    public bool debugEmitter;

    private readonly Queue<GameObject> spawnedMarkers = new Queue<GameObject>();

    private float nextEmitTime;
    private Vector3 lastEmitPosition;
    private bool hasEmittedBefore;
    private bool leftFoot;

    private void Awake()
    {
        ResolveReferences();
    }

    private void Reset()
    {
        ResolveReferences();
    }

    private void Update()
    {
        if (!CanEmit())
        {
            return;
        }

        if (Time.time < nextEmitTime)
        {
            return;
        }

        if (hasEmittedBefore)
        {
            float distance = Vector3.Distance(transform.position, lastEmitPosition);

            if (distance < minMoveDistance)
            {
                return;
            }
        }

        TryEmitEvidence();

        nextEmitTime = Time.time + emitInterval;
        lastEmitPosition = transform.position;
        hasEmittedBefore = true;
    }

    private bool CanEmit()
    {
        if (!emitEvidence)
        {
            return false;
        }

        if (context && context.isCaptured)
        {
            return false;
        }

        if (GetEvidencePrefab() == null)
        {
            return false;
        }

        return true;
    }

    private void TryEmitEvidence()
    {
        GameObject prefab = GetEvidencePrefab();

        if (!prefab)
        {
            return;
        }

        Vector3 side = transform.right * (leftFoot ? -sideOffset : sideOffset);
        Vector3 back = -transform.forward * backOffset;

        Vector3 rayStart = transform.position + side + back + Vector3.up * raycastHeight;

        if (!Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, raycastDistance, groundMask, QueryTriggerInteraction.Ignore))
        {
            if (debugEmitter)
            {
                Debug.LogWarning($"[EntityEvidenceTrailEmitter] Failed to find ground below {name}.");
            }

            return;
        }

        Vector3 position = hit.point + hit.normal * surfaceOffset;

        Vector3 flatForward = Vector3.ProjectOnPlane(transform.forward, hit.normal);

        if (flatForward.sqrMagnitude < 0.01f)
        {
            flatForward = Vector3.forward;
        }

        Quaternion rotation = Quaternion.LookRotation(flatForward.normalized, hit.normal);
        rotation *= Quaternion.Euler(0f, Random.Range(-randomYaw, randomYaw), 0f);

        GameObject marker = Instantiate(prefab, position, rotation);
        marker.name = GetMarkerName(prefab);

        EntityEvidenceMarker evidence = marker.GetComponent<EntityEvidenceMarker>();

        if (evidence)
        {
            evidence.sourceEntityName = name;
        }

        spawnedMarkers.Enqueue(marker);
        leftFoot = !leftFoot;

        TrimOldMarkers();

        if (debugEmitter)
        {
            Debug.Log($"[EntityEvidenceTrailEmitter] Spawned evidence marker: {marker.name}");
        }
    }

    private GameObject GetEvidencePrefab()
    {
        if (evidencePrefabs != null && evidencePrefabs.Count > 0)
        {
            float totalWeight = 0f;

            for (int i = 0; i < evidencePrefabs.Count; i++)
            {
                EntityEvidenceSpawnEntry entry = evidencePrefabs[i];

                if (entry != null && entry.prefab && entry.weight > 0f)
                {
                    totalWeight += entry.weight;
                }
            }

            if (totalWeight > 0f)
            {
                float roll = Random.Range(0f, totalWeight);
                float running = 0f;

                for (int i = 0; i < evidencePrefabs.Count; i++)
                {
                    EntityEvidenceSpawnEntry entry = evidencePrefabs[i];

                    if (entry == null || !entry.prefab || entry.weight <= 0f)
                    {
                        continue;
                    }

                    running += entry.weight;

                    if (roll <= running)
                    {
                        return entry.prefab;
                    }
                }
            }
        }

        return footprintPrefab;
    }

    private string GetMarkerName(GameObject prefab)
    {
        if (!prefab)
        {
            return "Entity Evidence";
        }

        EntityEvidenceMarker marker = prefab.GetComponent<EntityEvidenceMarker>();

        if (!marker)
        {
            return prefab.name;
        }

        switch (marker.evidenceType)
        {
            case EntityEvidenceType.Residue:
                return "Entity Residue";

            case EntityEvidenceType.Scratch:
                return "Entity Scratch";

            case EntityEvidenceType.DisturbedSnow:
                return "Disturbed Snow";

            default:
                return leftFoot ? "Entity Footprint L" : "Entity Footprint R";
        }
    }

    private void TrimOldMarkers()
    {
        while (spawnedMarkers.Count > maxActiveMarkers)
        {
            GameObject oldMarker = spawnedMarkers.Dequeue();

            if (oldMarker)
            {
                Destroy(oldMarker);
            }
        }
    }

    private void ResolveReferences()
    {
        if (!context)
        {
            context = GetComponent<SnowEntityContext>();
        }
    }
}