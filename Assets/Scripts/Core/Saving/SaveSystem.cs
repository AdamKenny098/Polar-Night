// Author: Adam Kenny
// Student: Applied Computing (Game Development) 3rd Year (20102588)
// Date Created: 2025-07-16
// Description: Manages saving, loading, and deleting game data to and from a JSON file.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class SaveSystem
{
    private static string savePath = Application.persistentDataPath + "/savefile.json";

    public static SaveData CurrentSave { get; private set; }

    // Saves the current game state to a file.
    public static void SaveGame()
    {
        Debug.Log("Saving game...");
        GameManager gm = GameManager.Instance;

        if (CurrentSave == null)
        {
            CurrentSave = new SaveData();

            //Core game data
            CurrentSave.currentDay = gm.currentDay;
            CurrentSave.amountOfFuel = gm.amountOfFuel;
            CurrentSave.amountOfFood = gm.amountOfFood;
            CurrentSave.playerPosition = gm.playerPosition;

            // Game stages
            CurrentSave.currentDayStage = gm.currentStage;
            CurrentSave.currentHungerStage = gm.currentHungerStage;

            // Flags
            CurrentSave.hasGoneOutside = gm.hasGoneOutside;
            CurrentSave.hasEaten = gm.hasEaten;
            CurrentSave.hasMaintainedGenerators = gm.hasMaintainedGenerators;

            // Progression
            CurrentSave.sprintTime = gm.sprintTime;
            CurrentSave.successfulMaintenance = gm.successfulMaintenance;
            CurrentSave.playerMaterials = gm.playerMaterials;
            CurrentSave.unlockedUpgrades = new List<GameManager.PlayerUpgrade>(gm.unlockedUpgrades);

            // Inventories
            CurrentSave.playerInventory = gm.playerInventoryData;
            CurrentSave.fridgeInventory = gm.fridgeInventoryData;
            CurrentSave.fuelStoreInventory = gm.fuelStoreInventoryData;

            //Difficulty
            CurrentSave.currentDifficulty = gm.currentDifficulty;
            CurrentSave.fuelUseMultiplier = gm.fuelUseMult;
            CurrentSave.fuelPenaltyMultiplier = gm.fuelPenaltyMult;
            CurrentSave.lootMultiplier = gm.lootMult;

            string json = JsonUtility.ToJson(CurrentSave, true);
            File.WriteAllText(savePath, json);
        }

        else if (CurrentSave != null)
        {
            Debug.Log("Updating existing save...");

            //Core game data
            CurrentSave.currentDay = gm.currentDay;
            CurrentSave.amountOfFuel = gm.amountOfFuel;
            CurrentSave.amountOfFood = gm.amountOfFood;
            CurrentSave.playerPosition = gm.playerPosition;

            // Game stages
            CurrentSave.currentDayStage = gm.currentStage;
            CurrentSave.currentHungerStage = gm.currentHungerStage;

            // Flags
            CurrentSave.hasGoneOutside = gm.hasGoneOutside;
            CurrentSave.hasEaten = gm.hasEaten;
            CurrentSave.hasMaintainedGenerators = gm.hasMaintainedGenerators;

            // Progression
            CurrentSave.sprintTime = gm.sprintTime;
            CurrentSave.successfulMaintenance = gm.successfulMaintenance;
            CurrentSave.playerMaterials = gm.playerMaterials;
            CurrentSave.unlockedUpgrades = new List<GameManager.PlayerUpgrade>(gm.unlockedUpgrades);

            // Inventories
            CurrentSave.playerInventory = gm.playerInventoryData;
            CurrentSave.fridgeInventory = gm.fridgeInventoryData;
            CurrentSave.fuelStoreInventory = gm.fuelStoreInventoryData;

            //Difficulty
            CurrentSave.currentDifficulty = gm.currentDifficulty;
            CurrentSave.fuelUseMultiplier = gm.fuelUseMult;
            CurrentSave.fuelPenaltyMultiplier = gm.fuelPenaltyMult;
            CurrentSave.lootMultiplier = gm.lootMult;

            string json = JsonUtility.ToJson(CurrentSave, true);
            File.WriteAllText(savePath, json);
        }
    }

    // Loads the game state from the save file, or creates a new save if none exists.
    public static void LoadGame()
    {
        GameManager gm = GameManager.Instance;

        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            CurrentSave = JsonUtility.FromJson<SaveData>(json);

            // Restore core state
            gm.currentDay = CurrentSave.currentDay;
            gm.amountOfFuel = CurrentSave.amountOfFuel;
            gm.amountOfFood = CurrentSave.amountOfFood;
            gm.playerPosition = CurrentSave.playerPosition;

            // Restore game stages
            gm.currentStage = CurrentSave.currentDayStage;
            gm.currentHungerStage = CurrentSave.currentHungerStage;


            // Restore flags
            gm.hasGoneOutside = CurrentSave.hasGoneOutside;
            gm.hasEaten = CurrentSave.hasEaten;
            gm.hasMaintainedGenerators = CurrentSave.hasMaintainedGenerators;

            // Restore progression
            gm.sprintTime = CurrentSave.sprintTime;
            gm.successfulMaintenance = CurrentSave.successfulMaintenance;
            gm.playerMaterials = CurrentSave.playerMaterials;
            gm.unlockedUpgrades = new List<GameManager.PlayerUpgrade>(CurrentSave.unlockedUpgrades);

            // Restore inventory data
            gm.playerInventoryData = CurrentSave.playerInventory;
            gm.fridgeInventoryData = CurrentSave.fridgeInventory;
            gm.fuelStoreInventoryData = CurrentSave.fuelStoreInventory;

            //Difficulty
            gm.currentDifficulty = CurrentSave.currentDifficulty;
            gm.fuelUseMult = CurrentSave.fuelUseMultiplier;
            gm.fuelPenaltyMult = CurrentSave.fuelPenaltyMultiplier;
            gm.lootMult = CurrentSave.lootMultiplier;

        }
        else
        {
            CurrentSave = new SaveData();
            SaveGame(); // Save the new file immediately
        }
    }

    // Deletes the save file and resets CurrentSave.
    public static void DeleteSave()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
        }

        CurrentSave = new SaveData();
    }
}
