using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class PolarNightDay7FixTrapMaterials
{
    private const string MaterialsFolder = "Assets/Materials";
    private const string TrapMaterialsFolder = "Assets/Materials/Traps";

    private const string TrapMetalPath = "Assets/Materials/Traps/TrapMetal.mat";
    private const string PreviewValidPath = "Assets/Materials/Traps/TrapPreviewValid.mat";
    private const string PreviewInvalidPath = "Assets/Materials/Traps/TrapPreviewInvalid.mat";

    private const string TrapPrefabPath = "Assets/Prefabs/Traps/PlacedSnowTrap.prefab";

    [MenuItem("Tools/Polar Night/Day 7/Fix Trap Materials")]
    public static void FixTrapMaterials()
    {
        EnsureFolderExists("Assets", "Materials");
        EnsureFolderExists(MaterialsFolder, "Traps");

        Material trapMetal = GetOrCreateMaterial(
            TrapMetalPath,
            "TrapMetal",
            new Color(0.22f, 0.22f, 0.22f, 1f)
        );

        Material previewValid = GetOrCreateMaterial(
            PreviewValidPath,
            "TrapPreviewValid",
            new Color(0.1f, 0.8f, 0.25f, 0.45f)
        );

        Material previewInvalid = GetOrCreateMaterial(
            PreviewInvalidPath,
            "TrapPreviewInvalid",
            new Color(0.9f, 0.1f, 0.1f, 0.45f)
        );

        FixTrapPrefab(trapMetal);
        FixScenePlacedTraps(trapMetal);
        AssignPreviewMaterials(previewValid, previewInvalid);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

        Debug.Log("Trap materials fixed.");
    }

    private static void FixTrapPrefab(Material trapMetal)
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(TrapPrefabPath);

        if (!prefab)
        {
            Debug.LogWarning($"No trap prefab found at {TrapPrefabPath}");
            return;
        }

        Renderer[] renderers = prefab.GetComponentsInChildren<Renderer>(true);

        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].sharedMaterial = trapMetal;
            EditorUtility.SetDirty(renderers[i]);
        }

        PlacedSnowTrap trap = prefab.GetComponent<PlacedSnowTrap>();

        if (trap)
        {
            trap.renderers = renderers;
            EditorUtility.SetDirty(trap);
        }

        EditorUtility.SetDirty(prefab);
    }

    private static void FixScenePlacedTraps(Material trapMetal)
    {
        PlacedSnowTrap[] traps = Object.FindObjectsOfType<PlacedSnowTrap>(true);

        for (int i = 0; i < traps.Length; i++)
        {
            Renderer[] renderers = traps[i].GetComponentsInChildren<Renderer>(true);

            for (int j = 0; j < renderers.Length; j++)
            {
                renderers[j].sharedMaterial = trapMetal;
                EditorUtility.SetDirty(renderers[j]);
            }

            traps[i].renderers = renderers;
            EditorUtility.SetDirty(traps[i]);
        }

        Debug.Log($"Fixed materials on {traps.Length} scene trap(s).");
    }

    private static void AssignPreviewMaterials(Material valid, Material invalid)
    {
        TrapPlacementSystem placementSystem = Object.FindObjectOfType<TrapPlacementSystem>(true);

        if (!placementSystem)
        {
            return;
        }

        EditorUtility.SetDirty(placementSystem);
    }

    private static Material GetOrCreateMaterial(string path, string materialName, Color color)
    {
        Material material = AssetDatabase.LoadAssetAtPath<Material>(path);

        if (!material)
        {
            Shader shader = GetCompatibleShader();

            material = new Material(shader);
            material.name = materialName;

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