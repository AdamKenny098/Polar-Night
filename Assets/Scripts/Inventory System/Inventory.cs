// Author: Adam Kenny
// Student: Applied Computing (Game Development) 3rd Year (20102588)
// Date Created: 2025-07-16
// Description: Handles the player's inventory system, including adding, removing, transferring, and checking items.

using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public int slots = 10;
    public InventorySlot[] invSlots;

    private void Awake()
    {
        EnsureSlots();
    }

    private void EnsureSlots()
    {
        if (slots < 1)
        {
            slots = 1;
        }

        if (invSlots == null || invSlots.Length != slots)
        {
            invSlots = new InventorySlot[slots];
        }

        for (int i = 0; i < invSlots.Length; i++)
        {
            if (invSlots[i] == null)
            {
                invSlots[i] = new InventorySlot();
            }
        }
    }

    public bool AddItem(Item item, int amount)
    {
        EnsureSlots();

        if (!item || amount <= 0)
        {
            return false;
        }

        if (!HasSpaceFor(item, amount))
        {
            return false;
        }

        int remaining = amount;
        int maxStack = Mathf.Max(1, item.maxStack);

        for (int i = 0; i < invSlots.Length; i++)
        {
            InventorySlot slot = invSlots[i];

            if (!slot.IsEmpty && slot.item == item && slot.amount < maxStack)
            {
                int spaceLeft = maxStack - slot.amount;
                int addAmount = Mathf.Min(spaceLeft, remaining);

                slot.amount += addAmount;
                remaining -= addAmount;

                if (remaining <= 0)
                {
                    return true;
                }
            }
        }

        for (int i = 0; i < invSlots.Length; i++)
        {
            InventorySlot slot = invSlots[i];

            if (slot.IsEmpty)
            {
                int addAmount = Mathf.Min(maxStack, remaining);

                slot.item = item;
                slot.amount = addAmount;
                remaining -= addAmount;

                if (remaining <= 0)
                {
                    return true;
                }
            }
        }

        return remaining <= 0;
    }

    public bool HasSpaceFor(Item item, int amount)
    {
        EnsureSlots();

        if (!item || amount <= 0)
        {
            return false;
        }

        int availableSpace = 0;
        int maxStack = Mathf.Max(1, item.maxStack);

        for (int i = 0; i < invSlots.Length; i++)
        {
            InventorySlot slot = invSlots[i];

            if (slot.IsEmpty)
            {
                availableSpace += maxStack;
            }
            else if (slot.item == item && slot.amount < maxStack)
            {
                availableSpace += maxStack - slot.amount;
            }

            if (availableSpace >= amount)
            {
                return true;
            }
        }

        return false;
    }

    public int CountItem(Item item)
    {
        EnsureSlots();

        if (!item)
        {
            return 0;
        }

        int count = 0;

        for (int i = 0; i < invSlots.Length; i++)
        {
            InventorySlot slot = invSlots[i];

            if (!slot.IsEmpty && slot.item == item)
            {
                count += slot.amount;
            }
        }

        return count;
    }

    public int CountCategory(ItemCategory category)
    {
        EnsureSlots();

        int count = 0;

        for (int i = 0; i < invSlots.Length; i++)
        {
            InventorySlot slot = invSlots[i];

            if (!slot.IsEmpty && slot.item.category == category)
            {
                count += slot.amount;
            }
        }

        return count;
    }

    public bool HasItem(Item item, int amount)
    {
        if (!item || amount <= 0)
        {
            return false;
        }

        return CountItem(item) >= amount;
    }

    public bool HasItems(List<CraftingIngredient> ingredients)
    {
        if (ingredients == null || ingredients.Count == 0)
        {
            return true;
        }

        Dictionary<Item, int> requiredItems = BuildRequirementMap(ingredients);

        foreach (KeyValuePair<Item, int> pair in requiredItems)
        {
            if (!HasItem(pair.Key, pair.Value))
            {
                return false;
            }
        }

        return true;
    }

    public bool TryRemoveItem(Item item, int amount)
    {
        EnsureSlots();

        if (!HasItem(item, amount))
        {
            return false;
        }

        int remaining = amount;

        for (int i = 0; i < invSlots.Length; i++)
        {
            InventorySlot slot = invSlots[i];

            if (!slot.IsEmpty && slot.item == item)
            {
                int removeAmount = Mathf.Min(slot.amount, remaining);

                slot.amount -= removeAmount;
                remaining -= removeAmount;

                if (slot.amount <= 0)
                {
                    slot.ClearSlot();
                }

                if (remaining <= 0)
                {
                    return true;
                }
            }
        }

        return remaining <= 0;
    }

    public bool TryRemoveItems(List<CraftingIngredient> ingredients)
    {
        if (!HasItems(ingredients))
        {
            return false;
        }

        Dictionary<Item, int> requiredItems = BuildRequirementMap(ingredients);

        foreach (KeyValuePair<Item, int> pair in requiredItems)
        {
            TryRemoveItem(pair.Key, pair.Value);
        }

        return true;
    }

    public void RemoveItemAt(int index)
    {
        EnsureSlots();

        if (index >= 0 && index < invSlots.Length)
        {
            invSlots[index].ClearSlot();
        }
    }

    public bool TransferTo(Inventory targetInventory, int oldIndex)
    {
        EnsureSlots();

        if (!targetInventory || oldIndex < 0 || oldIndex >= invSlots.Length)
        {
            return false;
        }

        InventorySlot oldSlot = invSlots[oldIndex];

        if (oldSlot.IsEmpty)
        {
            return false;
        }

        bool wasAdded = targetInventory.AddItem(oldSlot.item, oldSlot.amount);

        if (wasAdded)
        {
            oldSlot.ClearSlot();
            return true;
        }

        return false;
    }

    private Dictionary<Item, int> BuildRequirementMap(List<CraftingIngredient> ingredients)
    {
        Dictionary<Item, int> requiredItems = new Dictionary<Item, int>();

        for (int i = 0; i < ingredients.Count; i++)
        {
            CraftingIngredient ingredient = ingredients[i];

            if (ingredient == null || !ingredient.item || ingredient.amount <= 0)
            {
                continue;
            }

            if (!requiredItems.ContainsKey(ingredient.item))
            {
                requiredItems.Add(ingredient.item, 0);
            }

            requiredItems[ingredient.item] += ingredient.amount;
        }

        return requiredItems;
    }
}