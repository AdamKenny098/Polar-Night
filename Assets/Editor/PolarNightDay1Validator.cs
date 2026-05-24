using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class PolarNightDay1Validator
{
    private const string ItemsFolder = "Assets/Items";
    private const string RecipesFolder = "Assets/Crafting/Recipes";
    private const string WordDatabasePath = "Assets/Generator/GeneratorWordDatabase.asset";
    private const string NoteboardEntriesFolder = "Assets/Noteboard/Entries";

    private static readonly string[] RequiredItemIds =
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
        "broken_circuit",
        "basic_snow_trap",
        "entity_bait",
        "generator_repair_kit",
        "containment_battery"
    };

    private static readonly string[] RequiredRecipeNames =
    {
        "Basic Snow Trap",
        "Entity Bait",
        "Generator Repair Kit",
        "Containment Battery"
    };

    [MenuItem("Tools/Polar Night/Validate Day 1 Data")]
    public static void ValidateDay1Data()
    {
        int errors = 0;
        int warnings = 0;

        Debug.Log("Starting Polar Night Day 1 data validation...");

        Dictionary<string, Item> itemsById = LoadItemsById(ref errors, ref warnings);

        ValidateRequiredItems(itemsById, ref errors);
        ValidateRecipes(itemsById, ref errors, ref warnings);
        ValidateGeneratorWordDatabase(ref errors, ref warnings);
        ValidateNoteboardEntries(ref errors, ref warnings);

        if (errors == 0 && warnings == 0)
        {
            Debug.Log("Polar Night Day 1 validation passed with no errors or warnings.");
        }
        else if (errors == 0)
        {
            Debug.LogWarning($"Polar Night Day 1 validation completed with {warnings} warning(s), but no errors.");
        }
        else
        {
            Debug.LogError($"Polar Night Day 1 validation failed with {errors} error(s) and {warnings} warning(s).");
        }
    }

    private static Dictionary<string, Item> LoadItemsById(ref int errors, ref int warnings)
    {
        Dictionary<string, Item> itemsById = new Dictionary<string, Item>();
        string[] guids = AssetDatabase.FindAssets("t:Item", new[] { ItemsFolder });

        if (guids.Length == 0)
        {
            Debug.LogError($"No Item assets found in {ItemsFolder}.");
            errors++;
            return itemsById;
        }

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Item item = AssetDatabase.LoadAssetAtPath<Item>(path);

            if (!item)
            {
                Debug.LogError($"Failed to load Item asset at {path}.");
                errors++;
                continue;
            }

            if (string.IsNullOrWhiteSpace(item.itemId))
            {
                Debug.LogError($"Item at {path} has no itemId.");
                errors++;
                continue;
            }

            if (string.IsNullOrWhiteSpace(item.Name))
            {
                Debug.LogError($"Item '{item.itemId}' has no display Name.");
                errors++;
            }

            if (item.maxStack < 1)
            {
                Debug.LogError($"Item '{item.itemId}' has invalid maxStack: {item.maxStack}.");
                errors++;
            }

            if (!item.icon)
            {
                Debug.LogWarning($"Item '{item.itemId}' has no icon assigned. Fine for Day 1, but should be fixed later.");
                warnings++;
            }

            if (itemsById.ContainsKey(item.itemId))
            {
                Debug.LogError($"Duplicate itemId found: {item.itemId}. Duplicate path: {path}");
                errors++;
                continue;
            }

            itemsById.Add(item.itemId, item);
        }

        Debug.Log($"Loaded {itemsById.Count} Item asset(s).");
        return itemsById;
    }

    private static void ValidateRequiredItems(Dictionary<string, Item> itemsById, ref int errors)
    {
        foreach (string requiredId in RequiredItemIds)
        {
            if (!itemsById.ContainsKey(requiredId))
            {
                Debug.LogError($"Missing required itemId: {requiredId}");
                errors++;
            }
        }
    }

    private static void ValidateRecipes(Dictionary<string, Item> itemsById, ref int errors, ref int warnings)
    {
        string[] guids = AssetDatabase.FindAssets("t:CraftingRecipe", new[] { RecipesFolder });

        if (guids.Length == 0)
        {
            Debug.LogError($"No CraftingRecipe assets found in {RecipesFolder}.");
            errors++;
            return;
        }

        Dictionary<string, CraftingRecipe> recipesByName = new Dictionary<string, CraftingRecipe>();

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            CraftingRecipe recipe = AssetDatabase.LoadAssetAtPath<CraftingRecipe>(path);

            if (!recipe)
            {
                Debug.LogError($"Failed to load CraftingRecipe at {path}.");
                errors++;
                continue;
            }

            if (string.IsNullOrWhiteSpace(recipe.recipeName))
            {
                Debug.LogError($"Recipe at {path} has no recipeName.");
                errors++;
                continue;
            }

            if (recipesByName.ContainsKey(recipe.recipeName))
            {
                Debug.LogError($"Duplicate recipeName found: {recipe.recipeName}. Duplicate path: {path}");
                errors++;
                continue;
            }

            recipesByName.Add(recipe.recipeName, recipe);

            if (!recipe.outputItem)
            {
                Debug.LogError($"Recipe '{recipe.recipeName}' has no output item.");
                errors++;
            }

            if (recipe.outputAmount < 1)
            {
                Debug.LogError($"Recipe '{recipe.recipeName}' has invalid outputAmount: {recipe.outputAmount}.");
                errors++;
            }

            if (recipe.ingredients == null || recipe.ingredients.Count == 0)
            {
                Debug.LogWarning($"Recipe '{recipe.recipeName}' has no ingredients.");
                warnings++;
                continue;
            }

            for (int i = 0; i < recipe.ingredients.Count; i++)
            {
                CraftingIngredient ingredient = recipe.ingredients[i];

                if (ingredient == null)
                {
                    Debug.LogError($"Recipe '{recipe.recipeName}' has a null ingredient at index {i}.");
                    errors++;
                    continue;
                }

                if (!ingredient.item)
                {
                    Debug.LogError($"Recipe '{recipe.recipeName}' has a missing ingredient item at index {i}.");
                    errors++;
                    continue;
                }

                if (ingredient.amount < 1)
                {
                    Debug.LogError($"Recipe '{recipe.recipeName}' has invalid ingredient amount for '{ingredient.item.Name}'.");
                    errors++;
                }

                if (!itemsById.ContainsKey(ingredient.item.itemId))
                {
                    Debug.LogError($"Recipe '{recipe.recipeName}' references an item not found in required item dictionary: {ingredient.item.itemId}");
                    errors++;
                }
            }
        }

        foreach (string requiredRecipe in RequiredRecipeNames)
        {
            if (!recipesByName.ContainsKey(requiredRecipe))
            {
                Debug.LogError($"Missing required recipe: {requiredRecipe}");
                errors++;
            }
        }

        Debug.Log($"Validated {recipesByName.Count} CraftingRecipe asset(s).");
    }

    private static void ValidateGeneratorWordDatabase(ref int errors, ref int warnings)
    {
        GeneratorWordDatabase database = AssetDatabase.LoadAssetAtPath<GeneratorWordDatabase>(WordDatabasePath);

        if (!database)
        {
            Debug.LogError($"Missing GeneratorWordDatabase at {WordDatabasePath}.");
            errors++;
            return;
        }

        if (database.wordSets == null || database.wordSets.Count == 0)
        {
            Debug.LogError("GeneratorWordDatabase has no word sets.");
            errors++;
            return;
        }

        HashSet<GeneratorWordCategory> categoriesFound = new HashSet<GeneratorWordCategory>();
        HashSet<string> globalWords = new HashSet<string>();

        foreach (GeneratorWordSet set in database.wordSets)
        {
            if (set == null)
            {
                Debug.LogError("GeneratorWordDatabase contains a null word set.");
                errors++;
                continue;
            }

            categoriesFound.Add(set.category);

            if (set.words == null || set.words.Count == 0)
            {
                Debug.LogError($"Generator word category '{set.category}' has no words.");
                errors++;
                continue;
            }

            foreach (string rawWord in set.words)
            {
                if (string.IsNullOrWhiteSpace(rawWord))
                {
                    Debug.LogError($"Generator word category '{set.category}' contains an empty word.");
                    errors++;
                    continue;
                }

                string word = rawWord.Trim().ToUpperInvariant();

                if (word.Length != 5)
                {
                    Debug.LogError($"Generator word '{word}' in category '{set.category}' is not 5 letters.");
                    errors++;
                }

                if (globalWords.Contains(word))
                {
                    Debug.LogWarning($"Duplicate generator word found across database: {word}");
                    warnings++;
                }
                else
                {
                    globalWords.Add(word);
                }
            }
        }

        foreach (GeneratorWordCategory category in System.Enum.GetValues(typeof(GeneratorWordCategory)))
        {
            if (!categoriesFound.Contains(category))
            {
                Debug.LogWarning($"GeneratorWordDatabase is missing category: {category}");
                warnings++;
            }
        }

        Debug.Log($"Validated GeneratorWordDatabase. Categories: {database.wordSets.Count}. Unique words: {globalWords.Count}.");
    }

    private static void ValidateNoteboardEntries(ref int errors, ref int warnings)
    {
        string[] guids = AssetDatabase.FindAssets("t:NoteboardEntryData", new[] { NoteboardEntriesFolder });

        if (guids.Length == 0)
        {
            Debug.LogError($"No NoteboardEntryData assets found in {NoteboardEntriesFolder}.");
            errors++;
            return;
        }

        HashSet<string> entryIds = new HashSet<string>();

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            NoteboardEntryData entry = AssetDatabase.LoadAssetAtPath<NoteboardEntryData>(path);

            if (!entry)
            {
                Debug.LogError($"Failed to load NoteboardEntryData at {path}.");
                errors++;
                continue;
            }

            if (string.IsNullOrWhiteSpace(entry.entryId))
            {
                Debug.LogError($"Noteboard entry at {path} has no entryId.");
                errors++;
                continue;
            }

            if (entryIds.Contains(entry.entryId))
            {
                Debug.LogError($"Duplicate noteboard entryId found: {entry.entryId}");
                errors++;
                continue;
            }

            entryIds.Add(entry.entryId);

            if (string.IsNullOrWhiteSpace(entry.title))
            {
                Debug.LogError($"Noteboard entry '{entry.entryId}' has no title.");
                errors++;
            }

            if (string.IsNullOrWhiteSpace(entry.body))
            {
                Debug.LogError($"Noteboard entry '{entry.entryId}' has no body text.");
                errors++;
            }

            if (entry.body != null && entry.body.Length < 20)
            {
                Debug.LogWarning($"Noteboard entry '{entry.entryId}' body is very short.");
                warnings++;
            }
        }

        Debug.Log($"Validated {entryIds.Count} NoteboardEntryData asset(s).");
    }
}