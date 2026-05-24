using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class PolarNightDemoItemPopulator
{
    private const string ItemsFolder = "Assets/Items";

    private struct ItemDefinition
    {
        public string AssetName;
        public string ItemId;
        public string DisplayName;
        public int MaxStack;
        public ItemCategory Category;
        public string Description;

        public ItemDefinition(
            string assetName,
            string itemId,
            string displayName,
            int maxStack,
            ItemCategory category,
            string description
        )
        {
            AssetName = assetName;
            ItemId = itemId;
            DisplayName = displayName;
            MaxStack = maxStack;
            Category = category;
            Description = description;
        }
    }

    [MenuItem("Tools/Polar Night/Populate Demo Items")]
    public static void PopulateDemoItems()
    {
        EnsureFolderExists("Assets", "Items");

        List<ItemDefinition> definitions = new List<ItemDefinition>
        {
            new ItemDefinition(
                "Rationed Meal",
                "rationed_meal",
                "Rationed Meal",
                10,
                ItemCategory.Food,
                "A sealed emergency meal. Not pleasant, but enough to keep the body moving."
            ),

            new ItemDefinition(
                "Gasoline",
                "gasoline",
                "Gasoline",
                20,
                ItemCategory.Fuel,
                "Fuel for the shelter generator. Without it, the cold wins."
            ),

            new ItemDefinition(
                "Materials",
                "materials",
                "Materials",
                50,
                ItemCategory.Material,
                "General salvage used for repairs, basic crafting, and shelter maintenance."
            ),

            new ItemDefinition(
                "Scrap Metal",
                "scrap_metal",
                "Scrap Metal",
                50,
                ItemCategory.CraftingComponent,
                "Twisted metal pulled from crates, wreckage, and broken equipment."
            ),

            new ItemDefinition(
                "Wire",
                "wire",
                "Wire",
                50,
                ItemCategory.CraftingComponent,
                "Usable wiring for traps, repairs, and improvised electrical systems."
            ),

            new ItemDefinition(
                "Battery Cell",
                "battery_cell",
                "Battery Cell",
                20,
                ItemCategory.CraftingComponent,
                "A small power cell. Useful for traps, containment tools, and emergency devices."
            ),

            new ItemDefinition(
                "Cloth",
                "cloth",
                "Cloth",
                30,
                ItemCategory.CraftingComponent,
                "Torn insulated fabric. Useful for patches, bindings, and basic survival crafting."
            ),

            new ItemDefinition(
                "Frozen Meat",
                "frozen_meat",
                "Frozen Meat",
                10,
                ItemCategory.Bait,
                "Frozen animal meat. It may attract things moving through the snow."
            ),

            new ItemDefinition(
                "Anomaly Residue",
                "anomaly_residue",
                "Anomaly Residue",
                25,
                ItemCategory.Anomaly,
                "A strange residue left behind by unnatural activity. It feels wrong to touch."
            ),

            new ItemDefinition(
                "Broken Circuit",
                "broken_circuit",
                "Broken Circuit",
                25,
                ItemCategory.CraftingComponent,
                "A damaged circuit board that still has usable components."
            ),

            new ItemDefinition(
                "Basic Snow Trap",
                "basic_snow_trap",
                "Basic Snow Trap",
                5,
                ItemCategory.Trap,
                "A crude deployable trap designed to restrain something moving through the snow."
            ),

            new ItemDefinition(
                "Entity Bait",
                "entity_bait",
                "Entity Bait",
                10,
                ItemCategory.Bait,
                "A disturbing lure made from meat and anomalous residue."
            ),

            new ItemDefinition(
                "Generator Repair Kit",
                "generator_repair_kit",
                "Generator Repair Kit",
                5,
                ItemCategory.Tool,
                "A small kit for emergency generator repairs and maintenance work."
            ),

            new ItemDefinition(
                "Containment Battery",
                "containment_battery",
                "Containment Battery",
                5,
                ItemCategory.Anomaly,
                "A reinforced battery prepared for containment equipment."
            )
        };

        int created = 0;
        int updated = 0;

        foreach (ItemDefinition definition in definitions)
        {
            Item item = FindExistingItem(definition);

            if (!item)
            {
                item = ScriptableObject.CreateInstance<Item>();

                string assetPath = $"{ItemsFolder}/{SanitizeFileName(definition.AssetName)}.asset";
                assetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);

                AssetDatabase.CreateAsset(item, assetPath);
                created++;
            }
            else
            {
                updated++;
            }

            item.itemId = definition.ItemId;
            item.Name = definition.DisplayName;
            item.maxStack = Mathf.Max(1, definition.MaxStack);
            item.category = definition.Category;
            item.description = definition.Description;

            EditorUtility.SetDirty(item);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Polar Night demo items populated. Created: {created}. Updated: {updated}. Folder: {ItemsFolder}");
    }

    private static Item FindExistingItem(ItemDefinition definition)
    {
        string[] guids = AssetDatabase.FindAssets("t:Item", new[] { ItemsFolder });

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Item item = AssetDatabase.LoadAssetAtPath<Item>(path);

            if (!item)
            {
                continue;
            }

            if (item.itemId == definition.ItemId)
            {
                return item;
            }

            if (item.Name == definition.DisplayName)
            {
                return item;
            }

            string fileName = Path.GetFileNameWithoutExtension(path);

            if (fileName == definition.AssetName)
            {
                return item;
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