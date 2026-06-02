using UnityEditor;
using UnityEngine;

public static class PolarNightDay11ASetupEvidenceTrail
{
    private const string SnowEntityPrefabPath = "Assets/Prefabs/Entities/SnowEntity.prefab";

    private const string PrefabsFolder = "Assets/Prefabs";
    private const string TrackingPrefabFolder = "Assets/Prefabs/Tracking";
    private const string FootprintPrefabPath = "Assets/Prefabs/Tracking/EntityFootprintMarker.prefab";

    private const string MaterialsFolder = "Assets/Materials";
    private const string TrackingMaterialFolder = "Assets/Materials/Tracking";
    private const string FootprintMaterialPath = "Assets/Materials/Tracking/EntityFootprint.mat";

    [MenuItem("Tools/Polar Night/Day 11/11A Setup Entity Evidence Trail")]
    public static void SetupEntityEvidenceTrail()
    {
        EnsureFolderExists("Assets", "Prefabs");
        EnsureFolderExists(PrefabsFolder, "Tracking");
        EnsureFolderExists("Assets", "Materials");
        EnsureFolderExists(MaterialsFolder, "Tracking");

        Material footprintMaterial = GetOrCreateMaterial(
            FootprintMaterialPath,
            new Color(0.055f, 0.06f, 0.065f, 1f)
        );

        GameObject footprintPrefab = CreateOrUpdateFootprintPrefab(footprintMaterial);

        AddTrailEmitterToSnowEntity(footprintPrefab);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Day 11A entity evidence trail setup complete.");
    }

    private static GameObject CreateOrUpdateFootprintPrefab(Material material)
    {
        GameObject existing = AssetDatabase.LoadAssetAtPath<GameObject>(FootprintPrefabPath);

        if (existing)
        {
            Renderer[] renderers = existing.GetComponentsInChildren<Renderer>(true);

            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].sharedMaterial = material;
                EditorUtility.SetDirty(renderers[i]);
            }

            EntityEvidenceMarker marker = existing.GetComponent<EntityEvidenceMarker>();

            if (!marker)
            {
                marker = existing.AddComponent<EntityEvidenceMarker>();
            }

            marker.evidenceType = EntityEvidenceType.Footprint;
            marker.destroyAfterLifetime = true;
            marker.lifetime = 180f;

            EditorUtility.SetDirty(marker);
            EditorUtility.SetDirty(existing);

            return existing;
        }

        GameObject root = new GameObject("EntityFootprintMarker");

        GameObject mesh = GameObject.CreatePrimitive(PrimitiveType.Cube);
        mesh.name = "Footprint Mesh";
        mesh.transform.SetParent(root.transform, false);
        mesh.transform.localPosition = Vector3.zero;
        mesh.transform.localRotation = Quaternion.identity;
        mesh.transform.localScale = new Vector3(0.22f, 0.015f, 0.5f);

        Renderer renderer = mesh.GetComponent<Renderer>();

        if (renderer)
        {
            renderer.sharedMaterial = material;
        }

        Collider collider = mesh.GetComponent<Collider>();

        if (collider)
        {
            Object.DestroyImmediate(collider);
        }

        EntityEvidenceMarker evidence = root.AddComponent<EntityEvidenceMarker>();
        evidence.evidenceType = EntityEvidenceType.Footprint;
        evidence.destroyAfterLifetime = true;
        evidence.lifetime = 180f;

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, FootprintPrefabPath);
        Object.DestroyImmediate(root);

        return prefab;
    }

    private static void AddTrailEmitterToSnowEntity(GameObject footprintPrefab)
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(SnowEntityPrefabPath);

        if (!prefab)
        {
            Debug.LogError($"Missing SnowEntity prefab at {SnowEntityPrefabPath}");
            return;
        }

        GameObject instance = PrefabUtility.LoadPrefabContents(SnowEntityPrefabPath);

        EntityEvidenceTrailEmitter emitter = instance.GetComponent<EntityEvidenceTrailEmitter>();

        if (!emitter)
        {
            emitter = instance.AddComponent<EntityEvidenceTrailEmitter>();
        }

        SnowEntityContext context = instance.GetComponent<SnowEntityContext>();

        emitter.context = context;
        emitter.footprintPrefab = footprintPrefab;
        emitter.emitEvidence = true;
        emitter.emitInterval = 1.25f;
        emitter.minMoveDistance = 0.75f;
        emitter.maxActiveMarkers = 35;
        emitter.raycastHeight = 2f;
        emitter.raycastDistance = 5f;
        emitter.surfaceOffset = 0.025f;
        emitter.sideOffset = 0.22f;
        emitter.backOffset = 0.35f;
        emitter.randomYaw = 8f;

        EditorUtility.SetDirty(emitter);

        PrefabUtility.SaveAsPrefabAsset(instance, SnowEntityPrefabPath);
        PrefabUtility.UnloadPrefabContents(instance);

        Debug.Log("Added/updated EntityEvidenceTrailEmitter on SnowEntity prefab.");
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