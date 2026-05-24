// Author: Adam Kenny
// Student: Applied Computing (Game Development) 3rd Year (20102588)
// Date Created: 2025-07-16
// Description: Defines a ScriptableObject item that can be created and used in the inventory system.

using UnityEngine;

public enum ItemCategory
{
    General,
    Food,
    Fuel,
    Material,
    CraftingComponent,
    Trap,
    Bait,
    Tool,
    Anomaly,
    Quest
}

[CreateAssetMenu(menuName = "Data/Item")]
public class Item : ScriptableObject
{
    [Header("Identity")]
    public string itemId;
    public string Name;

    [Header("Inventory")]
    [Min(1)] public int maxStack = 100;
    public Sprite icon;
    public ItemCategory category = ItemCategory.General;

    [Header("Description")]
    [TextArea(2, 4)] public string description;

    public bool IsCategory(ItemCategory targetCategory)
    {
        return category == targetCategory;
    }

    private void OnValidate()
    {
        if (string.IsNullOrWhiteSpace(itemId))
        {
            itemId = name.ToLowerInvariant().Replace(" ", "_");
        }

        if (string.IsNullOrWhiteSpace(Name))
        {
            Name = name;
        }

        if (maxStack < 1)
        {
            maxStack = 1;
        }
    }
}