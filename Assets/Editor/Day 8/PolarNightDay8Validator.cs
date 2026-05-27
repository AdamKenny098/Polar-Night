using UnityEditor;
using UnityEngine;

public static class PolarNightDay8Validator
{
    private const string TrapPrefabPath = "Assets/Prefabs/Traps/PlacedSnowTrap.prefab";

    [MenuItem("Tools/Polar Night/Day 8/Validate Trap Trigger Logic")]
    public static void ValidateTrapTriggerLogic()
    {
        int errors = 0;
        int warnings = 0;

        ValidateTrapPrefab(ref errors, ref warnings);
        ValidateSceneTargets(ref errors, ref warnings);

        if (errors == 0 && warnings == 0)
        {
            Debug.Log("Day 8 trap trigger validation passed.");
        }
        else if (errors == 0)
        {
            Debug.LogWarning($"Day 8 trap trigger validation passed with {warnings} warning(s).");
        }
        else
        {
            Debug.LogError($"Day 8 trap trigger validation failed with {errors} error(s) and {warnings} warning(s).");
        }
    }

    private static void ValidateTrapPrefab(ref int errors, ref int warnings)
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(TrapPrefabPath);

        if (!prefab)
        {
            Debug.LogError($"Missing trap prefab at {TrapPrefabPath}");
            errors++;
            return;
        }

        PlacedSnowTrap trap = prefab.GetComponent<PlacedSnowTrap>();

        if (!trap)
        {
            Debug.LogError("Trap prefab missing PlacedSnowTrap.");
            errors++;
            return;
        }

        Collider[] colliders = prefab.GetComponentsInChildren<Collider>(true);

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
            Debug.LogError("Trap prefab has no trigger collider.");
            errors++;
        }

        if (!trap.armedMaterial)
        {
            Debug.LogWarning("Trap prefab missing armedMaterial.");
            warnings++;
        }

        if (!trap.triggeredMaterial)
        {
            Debug.LogWarning("Trap prefab missing triggeredMaterial.");
            warnings++;
        }

        if (!trap.capturedMaterial)
        {
            Debug.LogWarning("Trap prefab missing capturedMaterial.");
            warnings++;
        }

        if (!trap.failedMaterial)
        {
            Debug.LogWarning("Trap prefab missing failedMaterial.");
            warnings++;
        }

        Debug.Log("Trap prefab validated.");
    }

    private static void ValidateSceneTargets(ref int errors, ref int warnings)
    {
        TrapCaptureTarget[] targets = Object.FindObjectsOfType<TrapCaptureTarget>(true);

        if (targets.Length == 0)
        {
            Debug.LogWarning("No TrapCaptureTarget found in the current scene. Fine if you removed the test target.");
            warnings++;
            return;
        }

        for (int i = 0; i < targets.Length; i++)
        {
            Rigidbody rb = targets[i].GetComponent<Rigidbody>();

            if (!rb)
            {
                Debug.LogWarning($"TrapCaptureTarget '{targets[i].name}' has no Rigidbody. Triggering may not work unless the trap has one.");
                warnings++;
            }

            Collider collider = targets[i].GetComponent<Collider>();

            if (!collider)
            {
                Debug.LogWarning($"TrapCaptureTarget '{targets[i].name}' has no Collider.");
                warnings++;
            }
        }

        Debug.Log($"Validated TrapCaptureTarget count: {targets.Length}");
    }
}