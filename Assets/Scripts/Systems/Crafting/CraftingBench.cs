using System.Collections.Generic;
using UnityEngine;

public class CraftingBench : MonoBehaviour, IInteractable
{
    [Header("Crafting Data")]
    public CraftingRecipeBook recipeBook;
    public List<CraftingRecipe> fallbackRecipes = new List<CraftingRecipe>();

    [Header("References")]
    public Inventory playerInventory;

    [Header("Display")]
    public string benchDisplayName = "Crafting Bench";

    private void Start()
    {
        if (!playerInventory)
        {
            FindPlayerInventory();
        }
    }

    public void Interact()
    {
        if (!playerInventory)
        {
            FindPlayerInventory();
        }

        if (!playerInventory)
        {
            Debug.LogWarning($"{name} could not find the player inventory.");
            return;
        }

        if (!CraftingBenchUI.Instance)
        {
            Debug.LogWarning("No CraftingBenchUI found in the scene.");
            return;
        }

        CraftingBenchUI.Instance.Open(this, playerInventory);
    }

    public List<CraftingRecipe> GetRecipes()
    {
        if (recipeBook)
        {
            return recipeBook.GetValidRecipes();
        }

        List<CraftingRecipe> validRecipes = new List<CraftingRecipe>();

        for (int i = 0; i < fallbackRecipes.Count; i++)
        {
            CraftingRecipe recipe = fallbackRecipes[i];

            if (recipe && recipe.outputItem)
            {
                validRecipes.Add(recipe);
            }
        }

        return validRecipes;
    }

    private void FindPlayerInventory()
    {
        GameObject player = GameObject.Find("Player");

        if (!player)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        if (player)
        {
            playerInventory = player.GetComponentInChildren<Inventory>();
        }
    }
}