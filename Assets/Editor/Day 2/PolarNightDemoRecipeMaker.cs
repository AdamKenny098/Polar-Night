using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class PolarNightDemoRecipePopulator
{
    private const string ItemsFolder = "Assets/Items";
    private const string CraftingFolder = "Assets/Crafting";
    private const string RecipesFolder = "Assets/Crafting/Recipes";

    private struct IngredientDefinition
    {
        public string ItemId;
        public int Amount;

        public IngredientDefinition(string itemId, int amount)
        {
            ItemId = itemId;
            Amount = amount;
        }
    }

    private struct RecipeDefinition
    {
        public string AssetName;
        public string RecipeName;
        public CraftingRecipeCategory Category;
        public string OutputItemId;
        public int OutputAmount;
        public float CraftingTime;
        public string Description;
        public IngredientDefinition[] Ingredients;

        public RecipeDefinition(
            string assetName,
            string recipeName,
            CraftingRecipeCategory category,
            string outputItemId,
            int outputAmount,
            float craftingTime,
            string description,
            IngredientDefinition[] ingredients
        )
        {
            AssetName = assetName;
            RecipeName = recipeName;
            Category = category;
            OutputItemId = outputItemId;
            OutputAmount = outputAmount;
            CraftingTime = craftingTime;
            Description = description;
            Ingredients = ingredients;
        }
    }

    [MenuItem("Tools/Polar Night/Populate Demo Recipes")]
    public static void PopulateDemoRecipes()
    {
        EnsureFolderExists("Assets", "Crafting");
        EnsureFolderExists(CraftingFolder, "Recipes");

        Dictionary<string, Item> itemsById = LoadItemsById();

        List<RecipeDefinition> definitions = new List<RecipeDefinition>
        {
            new RecipeDefinition(
                "Basic Snow Trap Recipe",
                "Basic Snow Trap",
                CraftingRecipeCategory.Containment,
                "basic_snow_trap",
                1,
                2f,
                "Creates a crude deployable trap for restraining an entity in the snow.",
                new[]
                {
                    new IngredientDefinition("materials", 3),
                    new IngredientDefinition("wire", 2),
                    new IngredientDefinition("battery_cell", 1)
                }
            ),

            new RecipeDefinition(
                "Entity Bait Recipe",
                "Entity Bait",
                CraftingRecipeCategory.Anomaly,
                "entity_bait",
                1,
                1.5f,
                "Creates a lure that may draw unnatural movement toward a trap.",
                new[]
                {
                    new IngredientDefinition("frozen_meat", 1),
                    new IngredientDefinition("anomaly_residue", 1)
                }
            ),

            new RecipeDefinition(
                "Generator Repair Kit Recipe",
                "Generator Repair Kit",
                CraftingRecipeCategory.Generator,
                "generator_repair_kit",
                1,
                2f,
                "Creates a small kit for emergency generator maintenance.",
                new[]
                {
                    new IngredientDefinition("wire", 2),
                    new IngredientDefinition("broken_circuit", 1),
                    new IngredientDefinition("materials", 1)
                }
            ),

            new RecipeDefinition(
                "Containment Battery Recipe",
                "Containment Battery",
                CraftingRecipeCategory.Containment,
                "containment_battery",
                1,
                2.5f,
                "Creates a reinforced battery for future containment equipment.",
                new[]
                {
                    new IngredientDefinition("battery_cell", 2),
                    new IngredientDefinition("anomaly_residue", 1),
                    new IngredientDefinition("wire", 1)
                }
            )
        };

        int created = 0;
        int updated = 0;
        int skipped = 0;

        foreach (RecipeDefinition definition in definitions)
        {
            if (!itemsById.TryGetValue(definition.OutputItemId, out Item outputItem))
            {
                Debug.LogWarning($"Skipped recipe '{definition.RecipeName}'. Missing output item id: {definition.OutputItemId}");
                skipped++;
                continue;
            }

            CraftingRecipe recipe = FindExistingRecipe(definition);

            if (!recipe)
            {
                recipe = ScriptableObject.CreateInstance<CraftingRecipe>();

                string assetPath = $"{RecipesFolder}/{SanitizeFileName(definition.AssetName)}.asset";
                assetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);

                AssetDatabase.CreateAsset(recipe, assetPath);
                created++;
            }
            else
            {
                updated++;
            }

            recipe.recipeName = definition.RecipeName;
            recipe.category = definition.Category;
            recipe.outputItem = outputItem;
            recipe.outputAmount = Mathf.Max(1, definition.OutputAmount);
            recipe.craftingTime = Mathf.Max(0f, definition.CraftingTime);
            recipe.description = definition.Description;

            recipe.ingredients.Clear();

            for (int i = 0; i < definition.Ingredients.Length; i++)
            {
                IngredientDefinition ingredientDefinition = definition.Ingredients[i];

                if (!itemsById.TryGetValue(ingredientDefinition.ItemId, out Item ingredientItem))
                {
                    Debug.LogWarning($"Recipe '{definition.RecipeName}' is missing ingredient item id: {ingredientDefinition.ItemId}");
                    continue;
                }

                recipe.ingredients.Add(new CraftingIngredient
                {
                    item = ingredientItem,
                    amount = Mathf.Max(1, ingredientDefinition.Amount)
                });
            }

            EditorUtility.SetDirty(recipe);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Polar Night demo recipes populated. Created: {created}. Updated: {updated}. Skipped: {skipped}. Folder: {RecipesFolder}");
    }

    private static Dictionary<string, Item> LoadItemsById()
    {
        Dictionary<string, Item> itemsById = new Dictionary<string, Item>();
        string[] guids = AssetDatabase.FindAssets("t:Item", new[] { ItemsFolder });

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Item item = AssetDatabase.LoadAssetAtPath<Item>(path);

            if (!item)
            {
                continue;
            }

            if (string.IsNullOrWhiteSpace(item.itemId))
            {
                Debug.LogWarning($"Item at '{path}' has no itemId.");
                continue;
            }

            if (!itemsById.ContainsKey(item.itemId))
            {
                itemsById.Add(item.itemId, item);
            }
            else
            {
                Debug.LogWarning($"Duplicate itemId found: {item.itemId}. Path: {path}");
            }
        }

        return itemsById;
    }

    private static CraftingRecipe FindExistingRecipe(RecipeDefinition definition)
    {
        string[] guids = AssetDatabase.FindAssets("t:CraftingRecipe", new[] { RecipesFolder });

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            CraftingRecipe recipe = AssetDatabase.LoadAssetAtPath<CraftingRecipe>(path);

            if (!recipe)
            {
                continue;
            }

            if (recipe.recipeName == definition.RecipeName)
            {
                return recipe;
            }

            string fileName = Path.GetFileNameWithoutExtension(path);

            if (fileName == definition.AssetName)
            {
                return recipe;
            }
        }

        return null;
    }

    private static void EnsureFolderExists(string parentFolder, string childFolder)
    {
        string fullPath = $"{parentFolder}/{childFolder}";

        if (!AssetDatabase.IsValidFolder(fullPath))
        {
            AssetDatabase.CreateFolder(parentFolder, childFolder);
        }
    }

    private static string SanitizeFileName(string fileName)
    {
        foreach (char invalidChar in Path.GetInvalidFileNameChars())
        {
            fileName = fileName.Replace(invalidChar.ToString(), "");
        }

        return fileName.Trim();
    }
}