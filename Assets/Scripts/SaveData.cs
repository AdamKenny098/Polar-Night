// Author: Adam Kenny
// Student: Applied Computing (Game Development) 3rd Year (20102588)
// Date Created: 2025-07-16
// Description: Serializable class for storing the game's save data including player stats, position, and inventory.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Stores all relevant game data to be saved and loaded.
[System.Serializable]
public class SaveData
{

    //Core game data
    public int currentDay;
    public int amountOfFuel;
    public int amountOfFood;
    public Vector3 playerPosition;

    // Game stages
    public GameManager.HungerStage currentHungerStage;
    public GameManager.DayStage currentDayStage;

    // Flags
    public bool hasGoneOutside;
    public bool hasEaten;
    public bool hasMaintainedGenerators;
    public bool gaveStartingFuel = false;

    // Progression
    public float sprintTime;
    public int successfulMaintenance;
    public int playerMaterials;

    public List<GameManager.PlayerUpgrade> unlockedUpgrades = new List<GameManager.PlayerUpgrade>();

    public InventorySaveData playerInventory;
    public InventorySaveData fridgeInventory;
    public InventorySaveData fuelStoreInventory;

    // Difficulty
    public GameManager.Difficulty currentDifficulty;
    public float fuelUseMultiplier;
    public float fuelPenaltyMultiplier;
    public float lootMultiplier;
}
