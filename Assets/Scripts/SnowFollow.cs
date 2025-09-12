// Author: Adam Kenny
// Student: Applied Computing (Game Development) 3rd Year (20102588)
// Date Created: 2025-08-15
// Description: Positioned particle system that follows the player, creating a snow effect in the game world.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowFollow : MonoBehaviour
{
    public Transform player;
    public Vector3 particleOffset = new Vector3(0, 5f, 0);
    public ParticleSystem snowSystem;

    // Start is called before the first frame update
    void Start()
    {
        if (!player)
        {
            GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");
            if (foundPlayer)
            {
                player = foundPlayer.transform;
            }
        }

        if (!snowSystem)
        {
            snowSystem = GetComponent<ParticleSystem>();
        } 
    }

    // Update is called once per frame
    void Update()
    {
        if (player)
        {
            transform.position = player.position + particleOffset;
        }
    }
}
