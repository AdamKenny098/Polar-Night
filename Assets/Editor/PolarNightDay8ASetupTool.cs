using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class PolarNightDay8SetupTool
{
    private const string TrapPrefabPath = "Assets/Prefabs/Traps/PlacedSnowTrap.prefab";

    [MenuItem("Tools/Polar Night/Day 8/Setup Trap Trigger Test")]
    public static void SetupTrapTriggerTest()
    {
        UpdateTrapPrefab();
        CreateTestTargetInScene();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

        Debug.Log("Day 8 trap trigger test setup complete.");
    }

    private static void UpdateTrapPrefab()
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

        trap.armOnStart = true;
        trap.captureDelay = 1.25f;
        trap.requireCaptureTarget = true;
        trap.validTargetTag = "";
        trap.validTargetLayers = ~0;
        trap.debugTrap = true;
        trap.renderers = prefab.GetComponentsInChildren<Renderer>(true);

        Collider[] colliders = prefab.GetComponents<Collider>();

        bool hasTrigger = false;

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].isTrigger)
            {
                hasTrigger = true;
                break;
            }
        }

        if (!hasTrigger)
        {
            SphereCollider trigger = prefab.AddComponent<SphereCollider>();
            trigger.isTrigger = true;
            trigger.radius = 1.2f;
            trigger.center = Vector3.up * 0.2f;
        }

        EditorUtility.SetDirty(trap);
        EditorUtility.SetDirty(prefab);

        Debug.Log("Trap prefab updated for Day 8 triggering.");
    }

    private static void CreateTestTargetInScene()
    {
        GameObject existing = GameObject.Find("Trap Test Target");

        if (existing)
        {
            Debug.Log("Trap Test Target already exists in scene.");
            return;
        }

        GameObject target = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        target.name = "Trap Test Target";
        target.transform.position = new Vector3(0f, 1f, 4f);
        target.transform.localScale = new Vector3(0.8f, 1f, 0.8f);

        Rigidbody rb = target.AddComponent<Rigidbody>();
        rb.useGravity = true;
        rb.isKinematic = false;

        TrapCaptureTarget captureTarget = target.AddComponent<TrapCaptureTarget>();
        captureTarget.targetRigidbody = rb;
        captureTarget.targetColliders = target.GetComponentsInChildren<Collider>();

        Renderer renderer = target.GetComponent<Renderer>();

        if (renderer)
        {
            Material material = new Material(GetCompatibleShader());
            SetMaterialColor(material, new Color(0.15f, 0.2f, 0.28f, 1f));
            renderer.sharedMaterial = material;
        }

        Debug.Log("Created Trap Test Target in scene.");
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
}