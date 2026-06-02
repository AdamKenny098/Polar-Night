using UnityEditor;
using UnityEngine;

public static class PolarNightDay11BSetupEvidenceVariety
{
    private const string SnowEntityPrefabPath = "Assets/Prefabs/Entities/SnowEntity.prefab";

    private const string TrackingPrefabFolder = "Assets/Prefabs/Tracking";
    private const string FootprintPrefabPath = "Assets/Prefabs/Tracking/EntityFootprintMarker.prefab";
    private const string DisturbedSnowPrefabPath = "Assets/Prefabs/Tracking/DisturbedSnowMarker.prefab";
    private const string ResiduePrefabPath = "Assets/Prefabs/Tracking/AnomalyResidueMarker.prefab";

    private const string MaterialsFolder = "Assets/Materials";
    private const string TrackingMaterialFolder = "Assets/Materials/Tracking";

    private const string DisturbedSnowMaterialPath = "Assets/Materials/Tracking/DisturbedSnow.mat";
    private const string ResidueMaterialPath = "Assets/Materials/Tracking/AnomalyResidueMarker.mat";

    [MenuItem("Tools/Polar Night/Day 11/11B Setup Evidence Variety")]
    public static void SetupEvidenceVariety()
    {
        EnsureFolderExists("Assets", "Prefabs");
        EnsureFolderExists("Assets/Prefabs", "Tracking");
        EnsureFolderExists("Assets", "Materials");
        EnsureFolderExists(MaterialsFolder, "Tracking");

        Material disturbedSnowMaterial = GetOrCreateMaterial(
            DisturbedSnowMaterialPath,
            new Color(0.11f, 0.13f, 0.15f, 1f)
        );

        Material residueMaterial = GetOrCreateMaterial(
            ResidueMaterialPath,
            new Color(0.18f, 0.02f, 0.24f, 1f)
        );

        GameObject disturbedSnow = CreateOrUpdateFlatMarker(
            DisturbedSnowPrefabPath,
            "DisturbedSnowMarker",
            EntityEvidenceType.DisturbedSnow,
            disturbedSnowMaterial,
            new Vector3(0.65f, 0.018f, 0.42f),
            140f
        );

        GameObject residue = CreateOrUpdateFlatMarker(
            ResiduePrefabPath,
            "AnomalyResidueMarker",
            EntityEvidenceType.Residue,
            residueMaterial,
            new Vector3(0.38f, 0.02f, 0.38f),
            180f
        );

        GameObject footprint = AssetDatabase.LoadAssetAtPath<GameObject>(FootprintPrefabPath);

        if (!footprint)
        {
            Debug.LogError($"Missing footprint prefab at {FootprintPrefabPath}. Run 11A setup first.");
            return;
        }

        ApplyToSnowEntity(footprint, disturbedSnow, residue);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Day 11B evidence variety setup complete.");
    }

    private static GameObject CreateOrUpdateFlatMarker(
        string path,
        string objectName,
        EntityEvidenceType evidenceType,
        Material material,
        Vector3 scale,
        float lifetime
    )
    {
        GameObject existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);

        if (existing)
        {
            SetupMarker(existing, evidenceType, material, lifetime);
            return existing;
        }

        GameObject root = new GameObject(objectName);

        GameObject mesh = GameObject.CreatePrimitive(PrimitiveType.Cube);
        mesh.name = "Marker Mesh";
        mesh.transform.SetParent(root.transform, false);
        mesh.transform.localPosition = Vector3.zero;
        mesh.transform.localScale = scale;

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

        EntityEvidenceMarker marker = root.AddComponent<EntityEvidenceMarker>();
        marker.evidenceType = evidenceType;
        marker.destroyAfterLifetime = true;
        marker.lifetime = lifetime;

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
        Object.DestroyImmediate(root);

        return prefab;
    }

    private static void SetupMarker(
        GameObject prefab,
        EntityEvidenceType evidenceType,
        Material material,
        float lifetime
    )
    {
        EntityEvidenceMarker marker = prefab.GetComponent<EntityEvidenceMarker>();

        if (!marker)
        {
            marker = prefab.AddComponent<EntityEvidenceMarker>();
        }

        marker.evidenceType = evidenceType;
        marker.destroyAfterLifetime = true;
        marker.lifetime = lifetime;

        Renderer[] renderers = prefab.GetComponentsInChildren<Renderer>(true);

        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].sharedMaterial = material;
            EditorUtility.SetDirty(renderers[i]);
        }

        EditorUtility.SetDirty(marker);
        EditorUtility.SetDirty(prefab);
    }

    private static void ApplyToSnowEntity(GameObject footprint, GameObject disturbedSnow, GameObject residue)
    {
        GameObject instance = PrefabUtility.LoadPrefabContents(SnowEntityPrefabPath);

        if (!instance)
        {
            Debug.LogError($"Missing SnowEntity prefab at {SnowEntityPrefabPath}");
            return;
        }

        EntityEvidenceTrailEmitter emitter = instance.GetComponent<EntityEvidenceTrailEmitter>();

        if (!emitter)
        {
            emitter = instance.AddComponent<EntityEvidenceTrailEmitter>();
        }

        emitter.footprintPrefab = footprint;

        emitter.evidencePrefabs.Clear();

        emitter.evidencePrefabs.Add(new EntityEvidenceSpawnEntry
        {
            prefab = footprint,
            weight = 8f
        });

        emitter.evidencePrefabs.Add(new EntityEvidenceSpawnEntry
        {
            prefab = disturbedSnow,
            weight = 3f
        });

        emitter.evidencePrefabs.Add(new EntityEvidenceSpawnEntry
        {
            prefab = residue,
            weight = 1f
        });

        emitter.maxActiveMarkers = 45;
        emitter.emitInterval = 1.25f;
        emitter.minMoveDistance = 0.75f;

        EditorUtility.SetDirty(emitter);

        PrefabUtility.SaveAsPrefabAsset(instance, SnowEntityPrefabPath);
        PrefabUtility.UnloadPrefabContents(instance);

        Debug.Log("SnowEntity evidence emitter updated with variety.");
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