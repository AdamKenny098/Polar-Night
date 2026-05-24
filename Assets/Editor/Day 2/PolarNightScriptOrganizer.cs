using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class PolarNightScriptOrganizer
{
    private const string ScriptsRoot = "Assets/Scripts";

    private static readonly Dictionary<string, string> ScriptTargets = new Dictionary<string, string>
    {
        // Core / game flow
        { "GameManager", "Assets/Scripts/Core" },
        { "GameOverManager", "Assets/Scripts/Core" },
        { "DayStageUI", "Assets/Scripts/Core/DayCycle" },
        { "DayCounterUI", "Assets/Scripts/Core/DayCycle" },
        { "ScenarioManager", "Assets/Scripts/Core/Scenario" },

        // Saving / settings
        { "SaveData", "Assets/Scripts/Core/Saving" },
        { "SaveSystem", "Assets/Scripts/Core/Saving" },
        { "InventorySaveData", "Assets/Scripts/Core/Saving" },
        { "SettingsManager", "Assets/Scripts/Core/Settings" },

        // Player
        { "PlayerMovement", "Assets/Scripts/Player" },
        { "FirstPersonCamera", "Assets/Scripts/Player" },
        { "MovePlayerWeek3", "Assets/Scripts/Player" },
        { "PlayerNeedsUI", "Assets/Scripts/Player/Needs" },

        // UI
        { "HUDManager", "Assets/Scripts/UI" },
        { "MenuController", "Assets/Scripts/UI/Menu" },
        { "PauseMenu", "Assets/Scripts/UI/Menu" },
        { "SettingsUI", "Assets/Scripts/UI/Menu" },
        { "ItemPickUpUI", "Assets/Scripts/UI/Inventory" },

        // Inventory
        { "Item", "Assets/Scripts/Systems/Inventory" },
        { "Inventory", "Assets/Scripts/Systems/Inventory" },
        { "InventorySlot", "Assets/Scripts/Systems/Inventory" },
        { "InventoryUI", "Assets/Scripts/Systems/Inventory" },
        { "SlotUI", "Assets/Scripts/Systems/Inventory" },

        // Crafting
        { "CraftingIngredient", "Assets/Scripts/Systems/Crafting" },
        { "CraftingRecipe", "Assets/Scripts/Systems/Crafting" },

        // Resources / loot / survival items
        { "ResourceCache", "Assets/Scripts/Systems/Resources" },
        { "ResourceCacheSpawner", "Assets/Scripts/Systems/Resources" },
        { "ResourceLootEntry", "Assets/Scripts/Systems/Resources" },
        { "ResourceLootTable", "Assets/Scripts/Systems/Resources" },
        { "FuelStorage", "Assets/Scripts/Systems/Resources" },

        // Generator
        { "GeneratorComputer", "Assets/Scripts/Systems/Generator" },
        { "GeneratorMiniGame", "Assets/Scripts/Systems/Generator" },
        { "GeneratorWordDatabase", "Assets/Scripts/Systems/Generator" },

        // Noteboard
        { "NoteboardEntryData", "Assets/Scripts/Systems/Noteboard" },

        // Interaction / interactables
        { "InteractSystem", "Assets/Scripts/Systems/Interaction" },
        { "Bed", "Assets/Scripts/Systems/Interaction/Interactables" },
        { "Fridge", "Assets/Scripts/Systems/Interaction/Interactables" },
        { "FrontDoor", "Assets/Scripts/Systems/Interaction/Interactables" },

        // Environment / world
        { "SnowFollow", "Assets/Scripts/World/Weather" },
        { "RandomInsideSpawn", "Assets/Scripts/World/Spawning" },
        { "Radiation", "Assets/Scripts/World/Hazards" },

        // Audio
        { "ToggleAudioTrigger", "Assets/Scripts/Audio" },

        // Utilities
        { "SelfDestruct", "Assets/Scripts/Utility" }
    };

    [MenuItem("Tools/Polar Night/Scripts/Dry Run Script Organization")]
    public static void DryRunScriptOrganization()
    {
        OrganizeScripts(true);
    }

    [MenuItem("Tools/Polar Night/Scripts/Organize Scripts")]
    public static void ApplyScriptOrganization()
    {
        bool confirm = EditorUtility.DisplayDialog(
            "Organize Polar Night Scripts",
            "This will move known scripts into organized folders using AssetDatabase.MoveAsset. Unity references should be preserved because .meta GUIDs are kept.\n\nRun this only after committing or making sure git can roll it back.",
            "Organize Scripts",
            "Cancel"
        );

        if (!confirm)
        {
            Debug.Log("Script organization cancelled.");
            return;
        }

        OrganizeScripts(false);
    }

    private static void OrganizeScripts(bool dryRun)
    {
        int moved = 0;
        int skipped = 0;
        int missing = 0;
        int errors = 0;

        Debug.Log(dryRun
            ? "Starting Polar Night script organization dry run..."
            : "Starting Polar Night script organization...");

        foreach (KeyValuePair<string, string> pair in ScriptTargets)
        {
            string scriptName = pair.Key;
            string targetFolder = pair.Value;

            string currentPath = FindScriptPath(scriptName);

            if (string.IsNullOrWhiteSpace(currentPath))
            {
                Debug.LogWarning($"Missing script: {scriptName}.cs");
                missing++;
                continue;
            }

            string targetPath = $"{targetFolder}/{scriptName}.cs";

            if (NormalizePath(currentPath) == NormalizePath(targetPath))
            {
                Debug.Log($"Already organized: {scriptName}.cs");
                skipped++;
                continue;
            }

            if (AssetDatabase.LoadAssetAtPath<MonoScript>(targetPath))
            {
                Debug.LogWarning($"Target already exists, skipping {scriptName}.cs. Target: {targetPath}");
                skipped++;
                continue;
            }

            if (dryRun)
            {
                Debug.Log($"Would move: {currentPath} -> {targetPath}");
                moved++;
                continue;
            }

            EnsureFolderPath(targetFolder);

            string result = AssetDatabase.MoveAsset(currentPath, targetPath);

            if (string.IsNullOrWhiteSpace(result))
            {
                Debug.Log($"Moved: {currentPath} -> {targetPath}");
                moved++;
            }
            else
            {
                Debug.LogError($"Failed to move {scriptName}.cs: {result}");
                errors++;
            }
        }

        if (!dryRun)
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            RemoveEmptyFoldersUnderScripts();
        }

        Debug.Log(
            dryRun
                ? $"Script organization dry run complete. Would move: {moved}. Skipped: {skipped}. Missing: {missing}. Errors: {errors}."
                : $"Script organization complete. Moved: {moved}. Skipped: {skipped}. Missing: {missing}. Errors: {errors}."
        );
    }

    private static string FindScriptPath(string scriptName)
    {
        string[] guids = AssetDatabase.FindAssets($"{scriptName} t:MonoScript", new[] { ScriptsRoot });

        List<string> exactMatches = new List<string>();

        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);

            if (path.Contains("/Editor/"))
            {
                continue;
            }

            string fileName = Path.GetFileNameWithoutExtension(path);

            if (fileName == scriptName)
            {
                exactMatches.Add(path);
            }
        }

        if (exactMatches.Count == 0)
        {
            return "";
        }

        if (exactMatches.Count > 1)
        {
            Debug.LogWarning($"Multiple scripts found named {scriptName}.cs. Using first match: {exactMatches[0]}");
        }

        return exactMatches[0];
    }

    private static void EnsureFolderPath(string folderPath)
    {
        string[] parts = folderPath.Split('/');
        string current = parts[0];

        for (int i = 1; i < parts.Length; i++)
        {
            string next = $"{current}/{parts[i]}";

            if (!AssetDatabase.IsValidFolder(next))
            {
                AssetDatabase.CreateFolder(current, parts[i]);
            }

            current = next;
        }
    }

    private static void RemoveEmptyFoldersUnderScripts()
    {
        string scriptsAbsolutePath = Path.Combine(
            Directory.GetCurrentDirectory(),
            ScriptsRoot
        ).Replace("\\", "/");

        if (!Directory.Exists(scriptsAbsolutePath))
        {
            return;
        }

        string[] directories = Directory.GetDirectories(scriptsAbsolutePath, "*", SearchOption.AllDirectories);

        for (int i = directories.Length - 1; i >= 0; i--)
        {
            string absoluteDirectory = directories[i].Replace("\\", "/");
            string assetDirectory = "Assets" + absoluteDirectory.Replace(Application.dataPath.Replace("\\", "/"), "");

            if (assetDirectory.Contains("/Editor"))
            {
                continue;
            }

            if (!AssetDatabase.IsValidFolder(assetDirectory))
            {
                continue;
            }

            string[] files = Directory.GetFiles(absoluteDirectory);
            string[] childDirectories = Directory.GetDirectories(absoluteDirectory);

            bool hasRealFiles = false;

            for (int j = 0; j < files.Length; j++)
            {
                if (!files[j].EndsWith(".meta"))
                {
                    hasRealFiles = true;
                    break;
                }
            }

            if (!hasRealFiles && childDirectories.Length == 0)
            {
                AssetDatabase.DeleteAsset(assetDirectory);
                Debug.Log($"Removed empty folder: {assetDirectory}");
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static string NormalizePath(string path)
    {
        return path.Replace("\\", "/").Trim();
    }
}