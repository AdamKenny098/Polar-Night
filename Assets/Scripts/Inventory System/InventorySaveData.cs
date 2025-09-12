// Author: Adam Kenny
// Student: Applied Computing (Game Development) 3rd Year (20102588)
// Date Created: 2025-07-16
// Description: Contains serializable classes for saving inventory data, including a list of item stacks with items and amounts.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventorySaveData
{
    public List<ItemStack> items = new List<ItemStack>();
}

[System.Serializable]
public class ItemStack
{
    public Item item;
    public int amount;

    // Creates a new ItemStack with the given item and amount.
    public ItemStack(Item item, int amount)
    {
        this.item = item;
        this.amount = amount;
    }
}
