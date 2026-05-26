using UnityEditor;
using UnityEngine;

public static class PolarNightDay7Validator
{
    private const string TrapItemPath = "Assets/Items/Basic Snow Trap.asset";
    private const string TrapPrefabPath = "Assets/Prefabs/Traps/PlacedSnowTrap.prefab";

    [MenuItem("Tools/Polar Night/Day 7/Validate Trap Placement")]
    public static void ValidateTrapPlacement()
    {
        int errors = 0;
        int warnings = 0;

        Item trapItem = AssetDatabase.LoadAssetAtPath<Item>(TrapItemPath);

        if (!trapItem)
        {
            Debug.LogError($"Missing Basic Snow Trap item at {TrapItemPath}");
            errors++;
        }

        GameObject trapPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(TrapPrefabPath);

        if (!trapPrefab)
        {
            Debug.LogError($"Missing trap prefab at {TrapPrefabPath}");
            errors++;
        }
        else
        {
            if (!trapPrefab.GetComponent<PlacedSnowTrap>())
            {
                Debug.LogError("Trap prefab is missing PlacedSnowTrap component.");
                errors++;
            }

            Collider[] colliders = trapPrefab.GetComponentsInChildren<Collider>();

            if (colliders.Length == 0)
            {
                Debug.LogWarning("Trap prefab has no colliders.");
                warnings++;
            }
        }

        TrapPlacementSystem placementSystem = Object.FindObjectOfType<TrapPlacementSystem>();

        if (!placementSystem)
        {
            Debug.LogError("No TrapPlacementSystem found in the current scene.");
            errors++;
        }
        else
        {
            if (!placementSystem.trapItem)
            {
                Debug.LogError("TrapPlacementSystem missing trapItem.");
                errors++;
            }

            if (!placementSystem.placedTrapPrefab)
            {
                Debug.LogError("TrapPlacementSystem missing placedTrapPrefab.");
                errors++;
            }

            if (!placementSystem.playerCamera && !Camera.main)
            {
                Debug.LogWarning("TrapPlacementSystem has no playerCamera and no Camera.main was found.");
                warnings++;
            }

            if (!placementSystem.playerInventory)
            {
                Debug.LogWarning("TrapPlacementSystem has no playerInventory assigned. It will try to find Player at runtime.");
                warnings++;
            }
        }

        if (errors == 0 && warnings == 0)
        {
            Debug.Log("Day 7 trap placement validation passed.");
        }
        else if (errors == 0)
        {
            Debug.LogWarning($"Day 7 trap placement validation passed with {warnings} warning(s).");
        }
        else
        {
            Debug.LogError($"Day 7 trap placement validation failed with {errors} error(s) and {warnings} warning(s).");
        }
    }
}