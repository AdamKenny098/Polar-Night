using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class PolarNightDay8BTrapFeedbackSetup
{
    private const string TrapPrefabPath = "Assets/Prefabs/Traps/PlacedSnowTrap.prefab";

    private const string MaterialsFolder = "Assets/Materials";
    private const string TrapMaterialsFolder = "Assets/Materials/Traps";

    private const string ArmedPath = "Assets/Materials/Traps/TrapState_Armed.mat";
    private const string TriggeredPath = "Assets/Materials/Traps/TrapState_Triggered.mat";
    private const string CapturedPath = "Assets/Materials/Traps/TrapState_Captured.mat";
    private const string FailedPath = "Assets/Materials/Traps/TrapState_Failed.mat";

    [MenuItem("Tools/Polar Night/Day 8/8B Setup Trap Feedback")]
    public static void SetupTrapFeedback()
    {
        EnsureFolderExists("Assets", "Materials");
        EnsureFolderExists(MaterialsFolder, "Traps");

        Material armed = GetOrCreateMaterial(ArmedPath, new Color(0.22f, 0.22f, 0.22f, 1f));
        Material triggered = GetOrCreateMaterial(TriggeredPath, new Color(0.95f, 0.65f, 0.08f, 1f));
        Material captured = GetOrCreateMaterial(CapturedPath, new Color(0.1f, 0.55f, 0.95f, 1f));
        Material failed = GetOrCreateMaterial(FailedPath, new Color(0.65f, 0.08f, 0.08f, 1f));

        ApplyToPrefab(armed, triggered, captured, failed);
        ApplyToSceneTraps(armed, triggered, captured, failed);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

        Debug.Log("Day 8B trap feedback setup complete.");
    }

    private static void ApplyToPrefab(Material armed, Material triggered, Material captured, Material failed)
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(TrapPrefabPath);

        if (!prefab)
        {
            Debug.LogError($"Missing trap prefab at {TrapPrefabPath}");
            return;
        }

        PlacedSnowTrap trap = prefab.GetComponent<PlacedSnowTrap>();

        if (!trap)
        {
            trap = prefab.AddComponent<PlacedSnowTrap>();
        }

        trap.armedMaterial = armed;
        trap.triggeredMaterial = triggered;
        trap.capturedMaterial = captured;
        trap.failedMaterial = failed;
        trap.renderers = prefab.GetComponentsInChildren<Renderer>(true);

        EditorUtility.SetDirty(trap);
        EditorUtility.SetDirty(prefab);
    }

    private static void ApplyToSceneTraps(Material armed, Material triggered, Material captured, Material failed)
    {
        PlacedSnowTrap[] traps = Object.FindObjectsOfType<PlacedSnowTrap>(true);

        for (int i = 0; i < traps.Length; i++)
        {
            traps[i].armedMaterial = armed;
            traps[i].triggeredMaterial = triggered;
            traps[i].capturedMaterial = captured;
            traps[i].failedMaterial = failed;
            traps[i].renderers = traps[i].GetComponentsInChildren<Renderer>(true);

            EditorUtility.SetDirty(traps[i]);
        }

        Debug.Log($"Updated feedback materials on {traps.Length} scene trap(s).");
    }

    private static Material GetOrCreateMaterial(string path, Color color)
    {
        Material material = AssetDatabase.LoadAssetAtPath<Material>(path);

        if (!material)
        {
            material = new Material(GetCompatibleShader());
            AssetDatabase.CreateAsset(material, path);
        }

        SetMaterialColor(material, color);
        EditorUtility.SetDirty(material);

        return material;
    }

    private static Shader GetCompatibleShader()
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");

        if (shader)
        {
            return shader;
        }

        shader = Shader.Find("HDRP/Lit");

        if (shader)
        {
            return shader;
        }

        shader = Shader.Find("Standard");

        if (shader)
        {
            return shader;
        }

        return Shader.Find("Sprites/Default");
    }

    private static void SetMaterialColor(Material material, Color color)
    {
        if (!material)
        {
            return;
        }

        if (material.HasProperty("_BaseColor"))
        {
            material.SetColor("_BaseColor", color);
            return;
        }

        if (material.HasProperty("_Color"))
        {
            material.SetColor("_Color", color);
        }
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