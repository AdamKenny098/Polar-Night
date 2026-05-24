using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Resource Loot Table")]
public class ResourceLootTable : ScriptableObject
{
    public List<ResourceLootEntry> entries = new List<ResourceLootEntry>();

    public bool TryGetRandomLoot(float lootMultiplier, out Item item, out int amount)
    {
        item = null;
        amount = 0;

        float totalWeight = GetTotalWeight();

        if (totalWeight <= 0f)
        {
            return false;
        }

        float roll = Random.Range(0f, totalWeight);
        float runningWeight = 0f;

        for (int i = 0; i < entries.Count; i++)
        {
            ResourceLootEntry entry = entries[i];

            if (entry == null || !entry.IsValid())
            {
                continue;
            }

            runningWeight += entry.weight;

            if (roll <= runningWeight)
            {
                item = entry.item;
                amount = entry.RollAmount(lootMultiplier);
                return item != null && amount > 0;
            }
        }

        return false;
    }

    public bool ContainsItemId(string itemId)
    {
        if (string.IsNullOrWhiteSpace(itemId))
        {
            return false;
        }

        for (int i = 0; i < entries.Count; i++)
        {
            ResourceLootEntry entry = entries[i];

            if (entry != null && entry.item && entry.item.itemId == itemId)
            {
                return true;
            }
        }

        return false;
    }

    private float GetTotalWeight()
    {
        float total = 0f;

        for (int i = 0; i < entries.Count; i++)
        {
            ResourceLootEntry entry = entries[i];

            if (entry != null && entry.IsValid())
            {
                total += entry.weight;
            }
        }

        return total;
    }

    private void OnValidate()
    {
        for (int i = 0; i < entries.Count; i++)
        {
            ResourceLootEntry entry = entries[i];

            if (entry == null)
            {
                continue;
            }

            if (entry.minAmount < 1)
            {
                entry.minAmount = 1;
            }

            if (entry.maxAmount < entry.minAmount)
            {
                entry.maxAmount = entry.minAmount;
            }

            if (entry.weight < 0f)
            {
                entry.weight = 0f;
            }
        }
    }
}