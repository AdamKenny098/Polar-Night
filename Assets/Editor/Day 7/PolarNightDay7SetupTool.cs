using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class PolarNightDay7SetupTool
{
    private const string TrapItemPath = "Assets/Items/Basic Snow Trap.asset";
    private const string TrapPrefabFolder = "Assets/Prefabs/Traps";
    private const string TrapPrefabPath = "Assets/Prefabs/Traps/PlacedSnowTrap.prefab";

    [MenuItem("Tools/Polar Night/Day 7/Setup Trap Placement")]
    public static void SetupTrapPlacement()
    {
        EnsureFolderExists("Assets", "Prefabs");
        EnsureFolderExists("Assets/Prefabs", "Traps");

        GameObject trapPrefab = CreateOrUpdateTrapPrefab();
        TrapPlacementSystem placementSystem = FindOrCreatePlacementSystem();

        placementSystem.placedTrapPrefab = trapPrefab;
        placementSystem.previewTrapPrefab = trapPrefab;
        placementSystem.trapItem = AssetDatabase.LoadAssetAtPath<Item>(TrapItemPath);
        placementSystem.requireTrapItem = true;
        placementSystem.placementRange = 4f;
        placementSystem.maxSurfaceAngle = 35f;
        placementSystem.collisionCheckRadius = 0.45f;

        if (!placementSystem.trapItem)
        {
            Debug.LogWarning($"Could not find Basic Snow Trap item at {TrapItemPath}.");
        }

        EditorUtility.SetDirty(placementSystem);
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Day 7 trap placement setup complete.");
    }

    private static GameObject CreateOrUpdateTrapPrefab()
    {
        GameObject existingPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(TrapPrefabPath);

        if (existingPrefab)
        {
            return existingPrefab;
        }

        GameObject root = new GameObject("PlacedSnowTrap");

        GameObject baseObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        baseObject.name = "Trap Base";
        baseObject.transform.SetParent(root.transform);
        baseObject.transform.localPosition = Vector3.zero;
        baseObject.transform.localRotation = Quaternion.identity;
        baseObject.transform.localScale = new Vector3(1.2f, 0.08f, 1.2f);

        GameObject crossA = GameObject.CreatePrimitive(PrimitiveType.Cube);
        crossA.name = "Trap Cross A";
        crossA.transform.SetParent(root.transform);
        crossA.transform.localPosition = new Vector3(0f, 0.08f, 0f);
        crossA.transform.localRotation = Quaternion.identity;
        crossA.transform.localScale = new Vector3(1.4f, 0.08f, 0.18f);

        GameObject crossB = GameObject.CreatePrimitive(PrimitiveType.Cube);
        crossB.name = "Trap Cross B";
        crossB.transform.SetParent(root.transform);
        crossB.transform.localPosition = new Vector3(0f, 0.1f, 0f);
        crossB.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
        crossB.transform.localScale = new Vector3(1.4f, 0.08f, 0.18f);

        PlacedSnowTrap trap = root.AddComponent<PlacedSnowTrap>();
        trap.renderers = root.GetComponentsInChildren<Renderer>();
        trap.armOnStart = true;

        SphereCollider trigger = root.AddComponent<SphereCollider>();
        trigger.isTrigger = true;
        trigger.radius = 1.2f;
        trigger.center = Vector3.up * 0.2f;

        BoxCollider bodyCollider = root.AddComponent<BoxCollider>();
        bodyCollider.isTrigger = false;
        bodyCollider.size = new Vector3(1.4f, 0.2f, 1.4f);
        bodyCollider.center = new Vector3(0f, 0.1f, 0f);

        ApplyTrapMaterials(root);

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, TrapPrefabPath);
        Object.DestroyImmediate(root);

        return prefab;
    }

    private static TrapPlacementSystem FindOrCreatePlacementSystem()
    {
        TrapPlacementSystem existing = Object.FindObjectOfType<TrapPlacementSystem>();

        if (existing)
        {
            return existing;
        }

        GameObject objectInstance = new GameObject("TrapPlacementSystem");
        return objectInstance.AddComponent<TrapPlacementSystem>();
    }

    private static void ApplyTrapMaterials(GameObject root)
    {
        Material metal = new Material(Shader.Find("Standard"));
        metal.color = new Color(0.22f, 0.22f, 0.22f, 1f);

        Renderer[] renderers = root.GetComponentsInChildren<Renderer>();

        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].sharedMaterial = metal;
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