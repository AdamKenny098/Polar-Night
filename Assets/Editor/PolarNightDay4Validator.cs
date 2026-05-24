using UnityEditor;
using UnityEngine;

public static class PolarNightDay4Validator
{
    [MenuItem("Tools/Polar Night/Day 4/Validate Crafting Output")]
    public static void ValidateCraftingOutput()
    {
        int errors = 0;
        int warnings = 0;

        CraftingBenchUI ui = Object.FindObjectOfType<CraftingBenchUI>();

        if (!ui)
        {
            Debug.LogError("No CraftingBenchUI found in the scene.");
            errors++;
        }
        else
        {
            if (!ui.panelRoot)
            {
                Debug.LogError("CraftingBenchUI missing panelRoot.");
                errors++;
            }

            if (!ui.recipeListParent)
            {
                Debug.LogError("CraftingBenchUI missing recipeListParent.");
                errors++;
            }

            if (!ui.recipeDetailsText)
            {
                Debug.LogError("CraftingBenchUI missing recipeDetailsText.");
                errors++;
            }

            if (!ui.craftButton)
            {
                Debug.LogError("CraftingBenchUI missing craftButton.");
                errors++;
            }

            if (!ui.craftButtonLabel)
            {
                Debug.LogError("CraftingBenchUI missing craftButtonLabel.");
                errors++;
            }

            if (!ui.craftingFeedbackText)
            {
                Debug.LogWarning("CraftingBenchUI missing craftingFeedbackText. Crafting still works, but feedback will be weaker.");
                warnings++;
            }
        }

        CraftingBench[] benches = Object.FindObjectsOfType<CraftingBench>();

        if (benches.Length == 0)
        {
            Debug.LogError("No CraftingBench found in the current scene.");
            errors++;
        }

        for (int i = 0; i < benches.Length; i++)
        {
            CraftingBench bench = benches[i];

            if (!bench.recipeBook && (bench.fallbackRecipes == null || bench.fallbackRecipes.Count == 0))
            {
                Debug.LogError($"CraftingBench '{bench.name}' has no recipeBook or fallback recipes.");
                errors++;
            }

            if (!bench.GetComponent<Collider>())
            {
                Debug.LogWarning($"CraftingBench '{bench.name}' has no Collider. Interaction may not work.");
                warnings++;
            }
        }

        if (errors == 0 && warnings == 0)
        {
            Debug.Log("Day 4 crafting output validation passed.");
        }
        else if (errors == 0)
        {
            Debug.LogWarning($"Day 4 crafting output validation passed with {warnings} warning(s).");
        }
        else
        {
            Debug.LogError($"Day 4 crafting output validation failed with {errors} error(s) and {warnings} warning(s).");
        }
    }
}