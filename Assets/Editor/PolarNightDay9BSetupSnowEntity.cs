using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public static class PolarNightDay9BSetupSnowEntity
{
    private const string PrefabsFolder = "Assets/Prefabs";
    private const string EntitiesFolder = "Assets/Prefabs/Entities";
    private const string SnowEntityPrefabPath = "Assets/Prefabs/Entities/SnowEntity.prefab";

    private const string MaterialsFolder = "Assets/Materials";
    private const string EntityMaterialsFolder = "Assets/Materials/Entities";
    private const string SnowEntityMaterialPath = "Assets/Materials/Entities/SnowEntityPlaceholder.mat";

    [MenuItem("Tools/Polar Night/Day 9/9B Setup Snow Entity Prefab")]
    public static void SetupSnowEntityPrefab()
    {
        EnsureFolderExists("Assets", "Prefabs");
        EnsureFolderExists(PrefabsFolder, "Entities");
        EnsureFolderExists("Assets", "Materials");
        EnsureFolderExists(MaterialsFolder, "Entities");

        Material entityMaterial = GetOrCreateMaterial(
            SnowEntityMaterialPath,
            new Color(0.08f, 0.09f, 0.11f, 1f)
        );

        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(SnowEntityPrefabPath);

        if (prefab)
        {
            UpdateExistingPrefab(prefab, entityMaterial);
        }
        else
        {
            CreateNewPrefab(entityMaterial);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"SnowEntity prefab setup complete: {SnowEntityPrefabPath}");
    }

    private static void CreateNewPrefab(Material entityMaterial)
    {
        GameObject root = new GameObject("SnowEntity");

        GameObject body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        body.name = "Body";
        body.transform.SetParent(root.transform, false);
        body.transform.localPosition = new Vector3(0f, 1f, 0f);
        body.transform.localScale = new Vector3(0.85f, 1.15f, 0.85f);

        Renderer bodyRenderer = body.GetComponent<Renderer>();

        if (bodyRenderer)
        {
            bodyRenderer.sharedMaterial = entityMaterial;
        }

        Collider bodyCollider = body.GetComponent<Collider>();

        if (bodyCollider)
        {
            Object.DestroyImmediate(bodyCollider);
        }

        CapsuleCollider collider = root.AddComponent<CapsuleCollider>();
        collider.center = new Vector3(0f, 1f, 0f);
        collider.height = 2.2f;
        collider.radius = 0.42f;

        Rigidbody rb = root.AddComponent<Rigidbody>();
        rb.useGravity = true;
        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        NavMeshAgent agent = root.AddComponent<NavMeshAgent>();
        agent.speed = 2.2f;
        agent.angularSpeed = 180f;
        agent.acceleration = 8f;
        agent.stoppingDistance = 0.5f;
        agent.height = 2.2f;
        agent.radius = 0.42f;
        agent.baseOffset = 0f;

        TrapCaptureTarget captureTarget = root.AddComponent<TrapCaptureTarget>();
        captureTarget.targetRigidbody = rb;
        captureTarget.targetColliders = root.GetComponentsInChildren<Collider>();
        captureTarget.freezeRigidbodyOnCapture = true;
        captureTarget.notifyResponders = true;

        SnowEntityContext context = root.AddComponent<SnowEntityContext>();
        context.agent = agent;
        context.captureTarget = captureTarget;
        context.wanderRadius = 12f;
        context.canMove = true;
        context.isCaptured = false;
        context.debugEntity = true;

        PrefabUtility.SaveAsPrefabAsset(root, SnowEntityPrefabPath);
        Object.DestroyImmediate(root);
    }

    private static void UpdateExistingPrefab(GameObject prefab, Material entityMaterial)
    {
        GameObject instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;

        if (!instance)
        {
            Debug.LogError("Failed to instantiate existing SnowEntity prefab for update.");
            return;
        }

        EnsureComponents(instance, entityMaterial);

        PrefabUtility.SaveAsPrefabAsset(instance, SnowEntityPrefabPath);
        Object.DestroyImmediate(instance);
    }

    private static void EnsureComponents(GameObject root, Material entityMaterial)
    {
        CapsuleCollider collider = root.GetComponent<CapsuleCollider>();

        if (!collider)
        {
            collider = root.AddComponent<CapsuleCollider>();
        }

        collider.center = new Vector3(0f, 1f, 0f);
        collider.height = 2.2f;
        collider.radius = 0.42f;

        Rigidbody rb = root.GetComponent<Rigidbody>();

        if (!rb)
        {
            rb = root.AddComponent<Rigidbody>();
        }

        rb.useGravity = true;
        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        NavMeshAgent agent = root.GetComponent<NavMeshAgent>();

        if (!agent)
        {
            agent = root.AddComponent<NavMeshAgent>();
        }

        agent.speed = 2.2f;
        agent.angularSpeed = 180f;
        agent.acceleration = 8f;
        agent.stoppingDistance = 0.5f;
        agent.height = 2.2f;
        agent.radius = 0.42f;
        agent.baseOffset = 0f;

        TrapCaptureTarget captureTarget = root.GetComponent<TrapCaptureTarget>();

        if (!captureTarget)
        {
            captureTarget = root.AddComponent<TrapCaptureTarget>();
        }

        captureTarget.targetRigidbody = rb;
        captureTarget.targetColliders = root.GetComponentsInChildren<Collider>();
        captureTarget.freezeRigidbodyOnCapture = true;
        captureTarget.notifyResponders = true;

        SnowEntityContext context = root.GetComponent<SnowEntityContext>();

        if (!context)
        {
            context = root.AddComponent<SnowEntityContext>();
        }

        context.agent = agent;
        context.captureTarget = captureTarget;
        context.wanderRadius = 12f;
        context.canMove = true;
        context.isCaptured = false;
        context.debugEntity = true;

        Renderer[] renderers = root.GetComponentsInChildren<Renderer>(true);

        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].sharedMaterial = entityMaterial;
        }
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