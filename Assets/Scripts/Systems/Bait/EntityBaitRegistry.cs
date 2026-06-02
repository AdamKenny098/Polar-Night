using System.Collections.Generic;
using UnityEngine;

public static class EntityBaitRegistry
{
    private static readonly List<PlacedEntityBait> activeBaits = new List<PlacedEntityBait>();

    public static void Register(PlacedEntityBait bait)
    {
        if (!bait)
        {
            return;
        }

        if (!activeBaits.Contains(bait))
        {
            activeBaits.Add(bait);
        }
    }

    public static void Unregister(PlacedEntityBait bait)
    {
        if (!bait)
        {
            return;
        }

        activeBaits.Remove(bait);
    }

    public static bool TryGetNearestBait(Vector3 position, float searchRadius, out PlacedEntityBait nearestBait)
    {
        nearestBait = null;

        float bestDistanceSqr = searchRadius * searchRadius;

        for (int i = activeBaits.Count - 1; i >= 0; i--)
        {
            PlacedEntityBait bait = activeBaits[i];

            if (!bait)
            {
                activeBaits.RemoveAt(i);
                continue;
            }

            if (!bait.CanAttractEntity())
            {
                continue;
            }

            Vector3 baitPosition = bait.GetAttractionPosition();
            float distanceSqr = (baitPosition - position).sqrMagnitude;

            if (distanceSqr <= bestDistanceSqr)
            {
                bestDistanceSqr = distanceSqr;
                nearestBait = bait;
            }
        }

        return nearestBait != null;
    }

    public static List<PlacedEntityBait> GetActiveBaits()
    {
        List<PlacedEntityBait> baits = new List<PlacedEntityBait>();

        for (int i = 0; i < activeBaits.Count; i++)
        {
            if (activeBaits[i] && activeBaits[i].CanAttractEntity())
            {
                baits.Add(activeBaits[i]);
            }
        }

        return baits;
    }

    public static void ClearNullEntries()
    {
        for (int i = activeBaits.Count - 1; i >= 0; i--)
        {
            if (!activeBaits[i])
            {
                activeBaits.RemoveAt(i);
            }
        }
    }
}