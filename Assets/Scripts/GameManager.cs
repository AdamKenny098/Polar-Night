// Author: Adam Kenny
// Student: Applied Computing (Game Development) 3rd Year (20102588)
// Date Created: 2025-07-16
// Description: Manages the core game state, stages, days, inventory, saving/loading, and transitions.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum DayStage
    {
        Morning,
        MidDay,
        Evening
    }

    public enum HungerStage
    {
        Full,
        Hungry,
        Famished,
        Starving
    }

    public enum PlayerUpgrade
    {
        BetterBoots,
        SlowMetabolism,
        MaintenancePro,
        AdaptiveClothing
    }

    public DayStage currentStage;
    public HungerStage currentHungerStage;
    public static GameManager Instance;

    public int currentDay;
    public int amountOfFuel;
    public int amountOfFood;

    public Vector3 playerPosition;
    public Inventory playerInventory;

    public bool hasGoneOutside = false;
    public bool hasEaten = false;
    public bool hasMaintainedGenerators = false;

    public string gameOverReason;

    public InventorySaveData playerInventoryData = new InventorySaveData();
    public InventorySaveData fridgeInventoryData = new InventorySaveData();
    public InventorySaveData fuelStoreInventoryData = new InventorySaveData();

    public Inventory fuelStoreInventory;

    public List<PlayerUpgrade> unlockedUpgrades = new List<PlayerUpgrade>();

    public float sprintTime = 0f;
    public int successfulMaintenance = 0;
    public int playerMaterials = 0;

    public bool hasGivenStarterFuel = false;

    public enum Difficulty
    {
        Easy,
        Normal,
        Hard,
        Custom
    }

    public Difficulty currentDifficulty = Difficulty.Normal;

    [Range(0.5f, 5f)] public float fuelUseMult = 1f;
    [Range(0.5f, 5f)] public float fuelPenaltyMult = 1f;
    [Range(0.5f, 5f)] public float lootMult = 1f;


    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Inside" || scene.name == "Outside") // Or check for other game scenes as needed
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player)
            {
                playerInventory = player.GetComponentInChildren<Inventory>();
            }

            if (scene.name == "Inside")
            {
                Inventory fuelStorage = GameObject.FindWithTag("FuelStorage")?.GetComponent<Inventory>();
                if (fuelStorage)
                {
                    fuelStoreInventory = fuelStorage;

                    LoadInventory(fuelStoreInventory, fuelStoreInventoryData);
                }
            }

            DayStageUI.Instance.UpdateStageDisplay(currentStage);
        }
    }


    // Sets up the singleton and initializes the game.
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        enabled = true;
        InitializeGame();

    }




    // Handles player respawn and HUD refresh on scene load.
    void Start()
    {
        if (SceneManager.GetActiveScene().name == "Inside" && GameManager.Instance.hasGoneOutside)
        {
            GameObject player = GameObject.FindWithTag("Player");

            playerInventory = player.GetComponentInChildren<Inventory>();
            if (playerInventory == null)
            {
                return;
            }
            DayStageUI.Instance.UpdateStageDisplay(currentStage);
        }


    }

    // Starts a new game and loads the first scene.
    public void StartNewGame()
    {
        SaveSystem.DeleteSave();

        currentDay = 1;
        amountOfFuel = 0;
        amountOfFood = 0;
        playerPosition = Vector3.zero;

        currentStage = DayStage.Morning;
        currentHungerStage = HungerStage.Full;

        hasGoneOutside = false;
        hasEaten = false;
        hasMaintainedGenerators = false;

        sprintTime = 0f;
        successfulMaintenance = 0;
        playerMaterials = 0;
        unlockedUpgrades = new List<PlayerUpgrade>();

        playerInventoryData = new InventorySaveData();
        fridgeInventoryData = new InventorySaveData();
        fuelStoreInventoryData = new InventorySaveData();
        
    }

    // Initializes the game and loads the save system.
    private void InitializeGame()
    {
        SaveSystem.LoadGame(); // Will create and save new file if missing
        ApplyLoadedData(); // Apply loaded data to GameManager
    }

    // Applies data from the loaded save to GameManager fields.
    private void ApplyLoadedData()
    {
        // Fills the GameManager fields with data from the current save
        if (SaveSystem.CurrentSave != null)
        {
            currentDay = SaveSystem.CurrentSave.currentDay;
            amountOfFuel = SaveSystem.CurrentSave.amountOfFuel;
            amountOfFood = SaveSystem.CurrentSave.amountOfFood;
            playerPosition = SaveSystem.CurrentSave.playerPosition;

            currentStage = SaveSystem.CurrentSave.currentDayStage;
            currentHungerStage = SaveSystem.CurrentSave.currentHungerStage;

            hasGoneOutside = SaveSystem.CurrentSave.hasGoneOutside;
            hasEaten = SaveSystem.CurrentSave.hasEaten;
            hasMaintainedGenerators = SaveSystem.CurrentSave.hasMaintainedGenerators;

            sprintTime = SaveSystem.CurrentSave.sprintTime;
            successfulMaintenance = SaveSystem.CurrentSave.successfulMaintenance;
            playerMaterials = SaveSystem.CurrentSave.playerMaterials;
            unlockedUpgrades = new List<PlayerUpgrade>(SaveSystem.CurrentSave.unlockedUpgrades);

            playerInventoryData = SaveSystem.CurrentSave.playerInventory;
            fridgeInventoryData = SaveSystem.CurrentSave.fridgeInventory;
            fuelStoreInventoryData = SaveSystem.CurrentSave.fuelStoreInventory;
        }
        // Ready it for a new game with default values
        else
        {
            StartNewGame(); // Start a new game if no save exists
        }
    }

    // Ends the current day and updates resources.
    public void EndDay()
    {
        currentDay++;

        currentStage = DayStage.Morning;

        hasMaintainedGenerators = false;
        hasGoneOutside = false;
        hasEaten = false;

        DayStageUI.Instance.UpdateStageDisplay(currentStage);
        DayCounterUI.Instance.UpdateDayText(currentDay);

        if (FuelStorage.Instance != null)
        {
            amountOfFuel = FuelStorage.Instance.CountFuelInInventory();
        }

        SaveSystem.SaveGame();

        GameOver(); // Check if the game is over after the day ends
    }

    // Checks for game over conditions.
    public void GameOver()
    {
        if (currentHungerStage == HungerStage.Starving)
        {
            Starve();
        }

        else if (amountOfFuel <= 0)
        {
            Freeze();
        }

        else if (currentDay > 14)
        {
            WinGame();
        }
    }

    public void Starve()
    {
        gameOverReason = "starve";
        SceneManager.LoadScene("GameOver");
    }

    public void Freeze()
    {
        gameOverReason = "freeze";
        SceneManager.LoadScene("GameOver");
    }

    public void WinGame()
    {
        gameOverReason = "win";
        SceneManager.LoadScene("GameOver");
    }


    // Saves the inventory data.
    public InventorySaveData SaveInventory(Inventory inventory)
    {
        InventorySaveData data = new InventorySaveData();

        foreach (var slot in inventory.invSlots)
        {
            if (!slot.IsEmpty)
                data.items.Add(new ItemStack(slot.item, slot.amount));
        }

        return data;
    }

    // Loads inventory data into an inventory.
    public void LoadInventory(Inventory inventory, InventorySaveData data)
    {
        // Clear all current items before loading
        foreach (var slot in inventory.invSlots)
        {
            slot.ClearSlot();
        }

        foreach (var stack in data.items)
        {
            inventory.AddItem(stack.item, stack.amount);
        }
    }

    // Advances the game stage (Morning → MidDay → Evening).
    public void AdvanceStage()
    {
        SaveSystem.SaveGame();
        UpgradeConditions();

        if (FuelStorage.Instance != null)
        {
            amountOfFuel = FuelStorage.Instance.CountFuelInInventory();
        }

        // 🔻 Check if the player forgot to eat during this stage
        if (!hasEaten)
        {
            switch (currentHungerStage)
            {
                case HungerStage.Full:
                    currentHungerStage = HungerStage.Hungry;
                    break;
                case HungerStage.Hungry:
                    currentHungerStage = HungerStage.Famished;
                    break;
                case HungerStage.Famished:
                    currentHungerStage = HungerStage.Starving;
                    break;
                case HungerStage.Starving:
                    Starve(); // Game over from starvation
                    break;
            }
        }

        // 🔁 Reset eating flag for the next stage
        hasEaten = false;

        // 🔄 Advance the day stage
        switch (currentStage)
        {
            case DayStage.Morning:
                currentStage = DayStage.MidDay;
                break;

            case DayStage.MidDay:
                currentStage = DayStage.Evening;
                hasMaintainedGenerators = false;
                break;

            case DayStage.Evening:
                break;
        }

        // 🔃 Update UI
        int fuelLevel = FuelStorage.Instance.CountFuelInInventory();
        PlayerNeedsUI.Instance.UpdateTempUI(fuelLevel);
        PlayerNeedsUI.Instance.UpdateHungerUI(currentHungerStage);
        DayStageUI.Instance.UpdateStageDisplay(currentStage);
    }

    public bool UpgradeUnlocked(PlayerUpgrade upgrade)
    {
        return unlockedUpgrades.Contains(upgrade);
    }

    public void UnlockUpgrade(PlayerUpgrade upgrade)
    {
        if (!unlockedUpgrades.Contains(upgrade))
        {
            unlockedUpgrades.Add(upgrade);
        }
    }

    public void UpgradeConditions()
    {
        if (!unlockedUpgrades.Contains(PlayerUpgrade.BetterBoots) && sprintTime >= 300f)
        {
            UnlockUpgrade(PlayerUpgrade.BetterBoots);
        }

        if (!unlockedUpgrades.Contains(PlayerUpgrade.SlowMetabolism) && currentDay >= 5)
        {
            UnlockUpgrade(PlayerUpgrade.SlowMetabolism);
        }

        if (!unlockedUpgrades.Contains(PlayerUpgrade.MaintenancePro) && successfulMaintenance >= 10)
        {
            UnlockUpgrade(PlayerUpgrade.MaintenancePro);
        }

        CountMaterialsInInventory();
        if (!unlockedUpgrades.Contains(PlayerUpgrade.AdaptiveClothing) && playerMaterials >= 20)
        {
            UnlockUpgrade(PlayerUpgrade.AdaptiveClothing);
        }
    }

    public int CountMaterialsInInventory()
    {
        playerMaterials = 0;

        foreach (var slot in playerInventory.invSlots)
        {
            if (!slot.IsEmpty && slot.item.Name == "Materials") // or use a tag/type check
            {
                playerMaterials += slot.amount;
            }
        }

        // Clamp to max visual indicator length
        return playerMaterials;
    }

    public void RestoreHunger(int stages)
    {
        int current = (int)currentHungerStage;
        current -= stages;

        if (current < 0)
        {
            current = 0; // Clamp to Full
        }
        currentHungerStage = (HungerStage)current;
    }

    public void SetDifficulty(int index)
    {
        currentDifficulty = (Difficulty)index;

        switch (currentDifficulty)
        {
            case Difficulty.Easy:
                fuelUseMult = 0.75f;
                fuelPenaltyMult = 0.75f;
                lootMult = 1.5f;
                break;

            case Difficulty.Normal:
                fuelUseMult = 1f;
                fuelPenaltyMult = 1f;
                lootMult = 1f;
                break;

            case Difficulty.Hard:
                fuelUseMult = 1.5f;
                fuelPenaltyMult = 1.5f;
                lootMult = 0.5f;
                break;

            case Difficulty.Custom:
                fuelUseMult = SettingsManager.Instance.settings.customFuelUseMultiplier;
                fuelPenaltyMult = SettingsManager.Instance.settings.customFuelPenaltyMultiplier;
                lootMult = SettingsManager.Instance.settings.customLootMultiplier;
                break;
        }
    }
}
