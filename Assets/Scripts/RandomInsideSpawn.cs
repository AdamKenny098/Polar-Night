// Author: Adam Kenny
// Student: Applied Computing (Game Development) 3rd Year (20102588)
// Date Created: 2025-08-22
// Description: Spawns the player at a random spawn point from a list when the scene loads.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomInsideSpawn : MonoBehaviour
{
    public List<Transform> spawnPoints = new List<Transform>();
    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        if (spawnPoints.Count == 0 || !player)
        {
            return;
        }

        Transform randomSpawn = spawnPoints[Random.Range(0, spawnPoints.Count)];
        player.transform.position = randomSpawn.position;
        player.transform.rotation = randomSpawn.rotation;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
