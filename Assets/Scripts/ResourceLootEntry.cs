using UnityEngine;

[System.Serializable]
public class ResourceLootEntry
{
    public Item item;

    [Min(1)] public int minAmount = 1;
    [Min(1)] public int maxAmount = 1;

    [Min(0f)] public float weight = 1f;

    public bool affectedByLootMultiplier = true;

    public int RollAmount(float lootMultiplier)
    {
        int safeMin = Mathf.Max(1, minAmount);
        int safeMax = Mathf.Max(safeMin, maxAmount);

        int rolledAmount = Random.Range(safeMin, safeMax + 1);

        if (affectedByLootMultiplier)
        {
            rolledAmount = Mathf.RoundToInt(rolledAmount * lootMultiplier);
        }

        return Mathf.Max(1, rolledAmount);
    }

    public bool IsValid()
    {
        return item != null && weight > 0f && minAmount > 0 && maxAmount > 0;
    }
}