// Author: Adam Kenny
// Student: Applied Computing (Game Development) 3rd Year (20102588)
// Date Created: 2025-07-16
// Description: Handles the player's inventory system, including adding, removing, and transferring items.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    // Number of slots in the inventory.
    public int slots = 10;
    public InventorySlot[] invSlots;

    // Initializes inventory slots.
    void Awake()
    {
        invSlots = new InventorySlot[slots];
        for (int i = 0; i < slots; i++)
        {
            invSlots[i] = new InventorySlot();
        }
    }

    // Adds an item to the inventory, stacking if possible.
    public bool AddItem(Item item, int amount)
    {
        // First try stacking
        for (int i = 0; i < invSlots.Length; i++)
        {
            if (invSlots[i].item == item && invSlots[i].amount < item.maxStack)
            {
                int spaceLeft = item.maxStack - invSlots[i].amount;
                int addAmount = Mathf.Min(spaceLeft, amount);

                invSlots[i].amount += addAmount;
                amount -= addAmount;

                if (amount <= 0)
                    return true;
            }
        }

        // Find first empty slot
        for (int i = 0; i < invSlots.Length; i++)
        {
            if (invSlots[i].IsEmpty)
            {
                invSlots[i].item = item;
                invSlots[i].amount = amount;
                return true;
            }
        }

        return false;
    }

    // Removes an item from a given slot.
    public void RemoveItemAt(int index)
    {
        if (index >= 0 && index < invSlots.Length)
        {
            invSlots[index].ClearSlot();
        }
    }

    // Transfers an item from this inventory to another.
    public bool TransferTo(Inventory targetInventory, int oldIndex)
    {
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
}
