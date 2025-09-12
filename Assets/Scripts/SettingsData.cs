// Author: Adam Kenny
// Student: Applied Computing (Game Development) 3rd Year (20102588)
// Date Created: 2025-07-16
// Description: Stores the player's audio settings for saving and loading.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Stores settings for the game.
[System.Serializable]
public class SettingsData
{
    // Audio
    public float masterVolume = 1f;
    public float musicVolume = 1f;
    public float sfxVolume = 1f;

    //Gameplay
    public float cameraSensitivity = 250f;

    public float fuelUseMultiplier;
    public float fuelPenaltyMultiplier;
    public float lootMultiplier;

    public float customFuelUseMultiplier;
    public float customFuelPenaltyMultiplier;
    public float customLootMultiplier;
}
