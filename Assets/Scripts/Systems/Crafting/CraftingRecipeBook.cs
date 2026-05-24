using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Crafting Recipe Book")]
public class CraftingRecipeBook : ScriptableObject
{
    public List<CraftingRecipe> recipes = new List<CraftingRecipe>();

    public List<CraftingRecipe> GetValidRecipes()
    {
        List<CraftingRecipe> validRecipes = new List<CraftingRecipe>();

        for (int i = 0; i < recipes.Count; i++)
        {
            CraftingRecipe recipe = recipes[i];

            if (recipe && recipe.outputItem)
            {
                validRecipes.Add(recipe);
            }
        }

        return validRecipes;
    }
}