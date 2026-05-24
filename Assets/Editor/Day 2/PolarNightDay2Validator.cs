using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class PolarNightDay2Validator
{
    private const string LootTablePath = "Assets/Resources/LootTables/OutdoorResourceLootTable.asset";
    private const string RecipesFolder = "Assets/Crafting/Recipes";

    private static readonly string[] RequiredLootItemIds =
    {
        "rationed_meal",
        "gasoline",
        "materials",
        "scrap_metal",
        "wire",
        "battery_cell",
        "cloth",
        "frozen_meat",
        "anomaly_residue",
        "broken_circuit"
    };

    private static readonly string[] RequiredRecipeOutputIds =
    {
        "basic_snow_trap",
        "entity_bait",
        "generator_repair_kit",
        "containment_battery"
    };

    [MenuItem("Tools/Polar Night/Validate Day 2 Resource Integration")]
    public static void ValidateDay2ResourceIntegration()
    {
        int errors = 0;
        int warnings = 0;

        Debug.Log("Starting Polar Night Day 2 resource integration validation...");

        ValidateLootTable(ref errors, ref warnings);
        ValidateRecipeOutputs(ref errors, ref warnings);
        ValidateResourceCachePrefabs(ref errors, ref warnings);

        if (errors == 0 && warnings == 0)
        {
            Debug.Log("Polar Night Day 2 validation passed with no errors or warnings.");
        }
        else if (errors == 0)
        {
            Debug.LogWarning($"Polar Night Day 2 validation completed with {warnings} warning(s), but no errors.");
        }
        else
        {
            Debug.LogError($"Polar Night Day 2 validation failed with {errors} error(s) and {warnings} warning(s).");
        }
    }

    private static void ValidateLootTable(ref int errors, ref int warnings)
    {
        ResourceLootTable table = AssetDatabase.LoadAssetAtPath<ResourceLootTable>(LootTablePath);

        if (!table)
        {
            Debug.LogError($"Missing ResourceLootTable at {LootTablePath}");
            errors++;
            return;
        }

        if (table.entries == null || table.entries.Count == 0)
        {
            Debug.LogError("OutdoorResourceLootTable has no entries.");
            errors++;
            return;
        }

        HashSet<string> foundItemIds = new HashSet<string>();

        for (int i = 0; i < table.entries.Count; i++)
        {
            ResourceLootEntry entry = table.entries[i];

            if (entry == null)
            {
                Debug.LogError($"Loot table has null entry at index {i}.");
                errors++;
                continue;
            }

            if (!entry.item)
            {
                Debug.LogError($"Loot table entry at index {i} has no item.");
                errors++;
                continue;
            }

            if (string.IsNullOrWhiteSpace(entry.item.itemId))
            {
                Debug.LogError($"Loot table item '{entry.item.name}' has no itemId.");
                errors++;
                continue;
            }

            foundItemIds.Add(entry.item.itemId);

            if (entry.minAmount < 1)
            {
                Debug.LogError($"Loot entry '{entry.item.itemId}' has invalid minAmount: {entry.minAmount}");
                errors++;
            }

            if (entry.maxAmount < entry.minAmount)
            {
                Debug.LogError($"Loot entry '{entry.item.itemId}' maxAmount is lower than minAmount.");
                errors++;
            }

            if (entry.weight <= 0f)
            {
                Debug.LogError($"Loot entry '{entry.item.itemId}' has invalid weight: {entry.weight}");
                errors++;
            }
        }

        for (int i = 0; i < RequiredLootItemIds.Length; i++)
        {
            string requiredId = RequiredLootItemIds[i];

            if (!foundItemIds.Contains(requiredId))
            {
                Debug.LogError($"OutdoorResourceLootTable is missing required itemId: {requiredId}");
                errors++;
            }
        }

        Debug.Log($"Validated loot table entries: {table.entries.Count}");
    }

    private static void ValidateRecipeOutputs(ref int errors, ref int warnings)
    {
        string[] guids = AssetDatabase.FindAssets("t:CraftingRecipe", new[] { RecipesFolder });

        if (guids.Length == 0)
        {
            Debug.LogError($"No CraftingRecipe assets found in {RecipesFolder}.");
            errors++;
            return;
        }

        HashSet<string> outputIds = new HashSet<string>();

        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            CraftingRecipe recipe = AssetDatabase.LoadAssetAtPath<CraftingRecipe>(path);

            if (!recipe)
            {
                Debug.LogError($"Failed to load recipe at {path}.");
                errors++;
                continue;
            }

            if (!recipe.outputItem)
            {
                Debug.LogError($"Recipe '{recipe.recipeName}' has no output item.");
                errors++;
                continue;
            }

            outputIds.Add(recipe.outputItem.itemId);

            if (recipe.ingredients == null || recipe.ingredients.Count == 0)
            {
                Debug.LogWarning($"Recipe '{recipe.recipeName}' has no ingredients.");
                warnings++;
            }
        }

        for (int i = 0; i < RequiredRecipeOutputIds.Length; i++)
        {
            string requiredOutputId = RequiredRecipeOutputIds[i];

            if (!outputIds.Contains(requiredOutputId))
            {
                Debug.LogError($"Missing recipe output itemId: {requiredOutputId}");
                errors++;
            }
        }

        Debug.Log($"Validated recipe outputs: {outputIds.Count}");
    }

    private static void ValidateResourceCachePrefabs(ref int errors, ref int warnings)
    {
        ResourceLootTable expectedTable = AssetDatabase.LoadAssetAtPath<ResourceLootTable>(LootTablePath);

        if (!expectedTable)
        {
            return;
        }

        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets" });
        int resourceCachePrefabCount = 0;
        int assignedCount = 0;

        for (int i = 0; i < prefabGuids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(prefabGuids[i]);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            if (!prefab)
            {
                continue;
            }

            ResourceCache[] caches = prefab.GetComponentsInChildren<ResourceCache>(true);

            if (caches == null || caches.Length == 0)
            {
                continue;
            }

            resourceCachePrefabCount++;

            for (int j = 0; j < caches.Length; j++)
            {
                if (caches[j].lootTable == expectedTable)
                {
                    assignedCount++;
                }
                else
                {
                    Debug.LogWarning($"ResourceCache prefab does not use OutdoorResourceLootTable: {path}");
                    warnings++;
                }
            }
        }

        if (resourceCachePrefabCount == 0)
        {
            Debug.LogWarning("No ResourceCache prefabs found. If your resource caches are scene-only, assign the loot table manually.");
            warnings++;
        }

        Debug.Log($"Validated ResourceCache prefabs. Prefabs found: {resourceCachePrefabCount}. Assigned caches: {assignedCount}.");
    }
}