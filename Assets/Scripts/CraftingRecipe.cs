using System.Collections.Generic;
using UnityEngine;

public enum CraftingRecipeCategory
{
    Survival,
    Generator,
    Containment,
    Anomaly,
    Utility
}

[CreateAssetMenu(menuName = "Data/Crafting Recipe")]
public class CraftingRecipe : ScriptableObject
{
    public string recipeName;
    public CraftingRecipeCategory category = CraftingRecipeCategory.Survival;

    [Header("Output")]
    public Item outputItem;
    [Min(1)] public int outputAmount = 1;

    [Header("Crafting")]
    [Min(0f)] public float craftingTime = 0f;
    public List<CraftingIngredient> ingredients = new List<CraftingIngredient>();

    [Header("Description")]
    [TextArea(2, 5)] public string description;

    public bool CanCraft(Inventory inventory)
    {
        if (!inventory || !outputItem || outputAmount <= 0)
        {
            return false;
        }

        if (!inventory.HasItems(ingredients))
        {
            return false;
        }

        return inventory.HasSpaceFor(outputItem, outputAmount);
    }

    public bool TryCraft(Inventory inventory)
    {
        if (!CanCraft(inventory))
        {
            return false;
        }

        if (!inventory.TryRemoveItems(ingredients))
        {
            return false;
        }

        return inventory.AddItem(outputItem, outputAmount);
    }

    private void OnValidate()
    {
        if (string.IsNullOrWhiteSpace(recipeName))
        {
            recipeName = name;
        }

        if (outputAmount < 1)
        {
            outputAmount = 1;
        }

        if (craftingTime < 0f)
        {
            craftingTime = 0f;
        }
    }
}