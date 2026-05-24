using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceCacheSpawner : MonoBehaviour
{
    public GameObject resourcePrefab;

    [Range(0f, 1f)]
    public float spawnChance = 0.75f;

    [Header("Grid Settings for Resource Nodes")]
    public float terrainSize = 1000f;
    public int gridSpacing = 50;


    // Start is called before the first frame update
    void Start()
    {
        GenerateSpawnPoints();
        AdjustDifficulty();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void GenerateSpawnPoints()
    {
        for (int x = 0; x <= terrainSize; x += gridSpacing)
        {
            for (int z = 0; z <= terrainSize; z += gridSpacing)
            {
                Vector3 spawnPosition = new Vector3(x, 0, z);
                if (Random.value <= spawnChance)
                {
                    GameObject newSpawnPoint = Instantiate(resourcePrefab, spawnPosition, Quaternion.Euler(270,0,0));
                    newSpawnPoint.transform.SetParent(transform);
                }
            }
        }
    }

    void AdjustDifficulty()
    {
        int day = GameManager.Instance.currentDay;
        spawnChance = Mathf.Clamp01(1f - ((day * 0.025f)* GameManager.Instance.lootMult)); // 2.5% chance reduction per day
        gridSpacing = Mathf.Clamp(50 + (day * 5), 50, 200); // Increase spacing with each day
    }

}
