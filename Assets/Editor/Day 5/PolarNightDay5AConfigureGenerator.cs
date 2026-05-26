using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class PolarNightDay5AConfigureGenerator
{
    private const string WordDatabasePath = "Assets/Generator/GeneratorWordDatabase.asset";

    [MenuItem("Tools/Polar Night/Day 5/5A Configure Generator Context")]
    public static void ConfigureGeneratorContext()
    {
        GeneratorMinigame generator = Object.FindObjectOfType<GeneratorMinigame>();

        if (!generator)
        {
            Debug.LogError("No GeneratorMinigame found in the current scene.");
            return;
        }

        GeneratorWordDatabase database =
            AssetDatabase.LoadAssetAtPath<GeneratorWordDatabase>(WordDatabasePath);

        if (!database)
        {
            Debug.LogError($"No GeneratorWordDatabase found at {WordDatabasePath}.");
            return;
        }

        generator.wordDatabase = database;
        generator.currentContext = GeneratorRepairContext.Normal;
        generator.useContextCategories = true;
        generator.autoResolveContext = true;
        generator.manualContextOverride = false;
        generator.lowFuelThreshold = 3;

        EditorUtility.SetDirty(generator);
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

        Debug.Log("Day 5A generator context configured.");
    }
}