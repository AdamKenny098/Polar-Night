using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class PolarNightDemoLootTablePopulator
{
    private const string ItemsFolder = "Assets/Items";
    private const string ResourcesFolder = "Assets/Resources";
    private const string LootTablesFolder = "Assets/Resources/LootTables";
    private const string LootTablePath = "Assets/Resources/LootTables/OutdoorResourceLootTable.asset";

    private struct LootDefinition
    {
        public string ItemId;
        public int MinAmount;
        public int MaxAmount;
        public float Weight;
        public bool AffectedByLootMultiplier;

        public LootDefinition(
            string itemId,
            int minAmount,
            int maxAmount,
            float weight,
            bool affectedByLootMultiplier
        )
        {
            ItemId = itemId;
            MinAmount = minAmount;
            MaxAmount = maxAmount;
            Weight = weight;
            AffectedByLootMultiplier = affectedByLootMultiplier;
        }
    }

    [MenuItem("Tools/Polar Night/Populate Demo Loot Table")]
    public static void PopulateDemoLootTable()
    {
        EnsureFolderExists("Assets", "Resources");
        EnsureFolderExists(ResourcesFolder, "LootTables");

        Dictionary<string, Item> itemsById = LoadItemsById();

        ResourceLootTable table = AssetDatabase.LoadAssetAtPath<ResourceLootTable>(LootTablePath);

        if (!table)
        {
            table = ScriptableObject.CreateInstance<ResourceLootTable>();
            AssetDatabase.CreateAsset(table, LootTablePath);
        }

        List<LootDefinition> definitions = new List<LootDefinition>
        {
            new LootDefinition("rationed_meal", 1, 2, 12f, true),
            new LootDefinition("gasoline", 1, 2, 12f, true),
            new LootDefinition("materials", 1, 3, 14f, true),

            new LootDefinition("scrap_metal", 1, 4, 12f, true),
            new LootDefinition("wire", 1, 3, 10f, true),
            new LootDefinition("battery_cell", 1, 2, 7f, true),
            new LootDefinition("cloth", 1, 3, 8f, true),
            new LootDefinition("frozen_meat", 1, 2, 6f, true),
            new LootDefinition("anomaly_residue", 1, 1, 4f, true),
            new LootDefinition("broken_circuit", 1, 2, 6f, true)
        };

        table.entries.Clear();

        int added = 0;
        int skipped = 0;

        for (int i = 0; i < definitions.Count; i++)
        {
            LootDefinition definition = definitions[i];

            if (!itemsById.TryGetValue(definition.ItemId, out Item item))
            {
                Debug.LogWarning($"Skipped loot entry. Missing itemId: {definition.ItemId}");
                skipped++;
                continue;
            }

            table.entries.Add(new ResourceLootEntry
            {
                item = item,
                minAmount = Mathf.Max(1, definition.MinAmount),
                maxAmount = Mathf.Max(definition.MinAmount, definition.MaxAmount),
                weight = Mathf.Max(0f, definition.Weight),
                affectedByLootMultiplier = definition.AffectedByLootMultiplier
            });

            added++;
        }

        EditorUtility.SetDirty(table);

        int assignedPrefabs = AssignLootTableToResourceCachePrefabs(table);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Outdoor resource loot table populated. Added: {added}. Skipped: {skipped}. Assigned prefabs: {assignedPrefabs}. Path: {LootTablePath}");
    }

    private static Dictionary<string, Item> LoadItemsById()
    {
        Dictionary<string, Item> itemsById = new Dictionary<string, Item>();
        string[] guids = AssetDatabase.FindAssets("t:Item", new[] { ItemsFolder });

        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            Item item = AssetDatabase.LoadAssetAtPath<Item>(path);

            if (!item || string.IsNullOrWhiteSpace(item.itemId))
            {
                continue;
            }

            if (!itemsById.ContainsKey(item.itemId))
            {
                itemsById.Add(item.itemId, item);
            }
            else
            {
                Debug.LogWarning($"Duplicate itemId found while loading loot table items: {item.itemId}. Path: {path}");
            }
        }

        return itemsById;
    }

    private static int AssignLootTableToResourceCachePrefabs(ResourceLootTable table)
    {
        int assignedCount = 0;
        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets" });

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

            bool changed = false;

            for (int j = 0; j < caches.Length; j++)
            {
                if (caches[j].lootTable != table)
                {
                    caches[j].lootTable = table;
                    caches[j].rollOnStart = true;
                    EditorUtility.SetDirty(caches[j]);
                    changed = true;
                }
            }

            if (changed)
            {
                EditorUtility.SetDirty(prefab);
                assignedCount++;
            }
        }

        return assignedCount;
    }

    private static void EnsureFolderExists(string parentFolder, string childFolder)
    {
        string fullPath = $"{parentFolder}/{childFolder}";

        if (!AssetDatabase.IsValidFolder(fullPath))
        {
            AssetDatabase.CreateFolder(parentFolder, childFolder);
        }
    }
}