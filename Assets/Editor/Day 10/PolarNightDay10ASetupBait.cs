using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class PolarNightDay10ASetupBait
{
    private const string BaitItemPath = "Assets/Items/Entity Bait.asset";

    private const string PrefabsFolder = "Assets/Prefabs";
    private const string BaitPrefabFolder = "Assets/Prefabs/Bait";
    private const string BaitPrefabPath = "Assets/Prefabs/Bait/PlacedEntityBait.prefab";

    private const string MaterialsFolder = "Assets/Materials";
    private const string BaitMaterialsFolder = "Assets/Materials/Bait";
    private const string BaitMaterialPath = "Assets/Materials/Bait/EntityBait.mat";

    [MenuItem("Tools/Polar Night/Day 10/10A Setup Bait Placement")]
    public static void SetupBaitPlacement()
    {
        EnsureFolderExists("Assets", "Prefabs");
        EnsureFolderExists(PrefabsFolder, "Bait");
        EnsureFolderExists("Assets", "Materials");
        EnsureFolderExists(MaterialsFolder, "Bait");

        Material baitMaterial = GetOrCreateMaterial(
            BaitMaterialPath,
            new Color(0.38f, 0.05f, 0.08f, 1f)
        );

        GameObject baitPrefab = CreateOrUpdateBaitPrefab(baitMaterial);
        BaitPlacementSystem placementSystem = FindOrCreateBaitPlacementSystem();

        placementSystem.placedBaitPrefab = baitPrefab;
        placementSystem.previewBaitPrefab = baitPrefab;
        placementSystem.baitItem = AssetDatabase.LoadAssetAtPath<Item>(BaitItemPath);
        placementSystem.requireBaitItem = true;
        placementSystem.placementRange = 4f;
        placementSystem.maxSurfaceAngle = 35f;
        placementSystem.collisionCheckRadius = 0.35f;

        if (!placementSystem.baitItem)
        {
            Debug.LogWarning($"Could not find Entity Bait item at {BaitItemPath}");
        }

        EditorUtility.SetDirty(placementSystem);
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Day 10A bait placement setup complete.");
    }

    private static GameObject CreateOrUpdateBaitPrefab(Material baitMaterial)
    {
        GameObject existingPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(BaitPrefabPath);

        if (existingPrefab)
        {
            ApplyBaitPrefabSetup(existingPrefab, baitMaterial);
            return existingPrefab;
        }

        GameObject root = new GameObject("PlacedEntityBait");

        GameObject baitObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        baitObject.name = "Bait Mesh";
        baitObject.transform.SetParent(root.transform, false);
        baitObject.transform.localPosition = new Vector3(0f, 0.12f, 0f);
        baitObject.transform.localScale = new Vector3(0.45f, 0.18f, 0.45f);

        Renderer renderer = baitObject.GetComponent<Renderer>();

        if (renderer)
        {
            renderer.sharedMaterial = baitMaterial;
        }

        Collider meshCollider = baitObject.GetComponent<Collider>();

        if (meshCollider)
        {
            Object.DestroyImmediate(meshCollider);
        }

        SphereCollider collider = root.AddComponent<SphereCollider>();
        collider.radius = 0.35f;
        collider.center = new Vector3(0f, 0.12f, 0f);
        collider.isTrigger = true;

        PlacedEntityBait bait = root.AddComponent<PlacedEntityBait>();
        bait.attractionRadius = 18f;
        bait.isActive = true;
        bait.hasBeenConsumed = false;

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, BaitPrefabPath);
        Object.DestroyImmediate(root);

        return prefab;
    }

    private static void ApplyBaitPrefabSetup(GameObject prefab, Material baitMaterial)
    {
        PlacedEntityBait bait = prefab.GetComponent<PlacedEntityBait>();

        if (!bait)
        {
            bait = prefab.AddComponent<PlacedEntityBait>();
        }

        bait.attractionRadius = 18f;
        bait.isActive = true;
        bait.hasBeenConsumed = false;

        Collider collider = prefab.GetComponent<Collider>();

        if (!collider)
        {
            SphereCollider sphere = prefab.AddComponent<SphereCollider>();
            sphere.radius = 0.35f;
            sphere.center = new Vector3(0f, 0.12f, 0f);
            sphere.isTrigger = true;
        }

        Renderer[] renderers = prefab.GetComponentsInChildren<Renderer>(true);

        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].sharedMaterial = baitMaterial;
            EditorUtility.SetDirty(renderers[i]);
        }

        EditorUtility.SetDirty(bait);
        EditorUtility.SetDirty(prefab);
    }

    private static BaitPlacementSystem FindOrCreateBaitPlacementSystem()
    {
        BaitPlacementSystem existing = Object.FindObjectOfType<BaitPlacementSystem>();

        if (existing)
        {
            return existing;
        }

        GameObject objectInstance = new GameObject("BaitPlacementSystem");
        return objectInstance.AddComponent<BaitPlacementSystem>();
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