// Author: Adam Kenny
// Student: Applied Computing (Game Development) 3rd Year (20102588)
// Date Created: 2025-08-15
// Description: Manages the generator minigame where players guess a word to maintain the generator.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class GeneratorMinigame : MonoBehaviour
{
    public static GeneratorMinigame Instance;
    public TMP_InputField inputField;
    public Transform guessesContainer;
    public GameObject guessRowPrefab;
    public string[] wordList;
    private string targetWord;
    private int maxGuesses = 6;
    private int currentGuess = 0;
    private bool isActive = false;
    
    public bool cancelInput;

    public GameObject inventoryUI;
    public InteractSystem interactionSystem;
    public GameObject pauseMenu;

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
    }

    // Start is called before the first frame update
    void Start()
    {
        inputField.characterLimit = 5;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isActive) return;

        if (isActive && cancelInput)
        {
            interactionSystem.enabled = false;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void StartMinigame()
    {
        HUDManager.Instance.InventoryLocked = true;
        
        PauseMenu.Instance.SetIsPaused(true);
        cancelInput = true;
        HUDManager.Instance.ShowGeneratorHUD();
        targetWord = wordList[Random.Range(0, wordList.Length)];
        currentGuess = 0;
        isActive = true;

        //erase previous guesses from previous games
        foreach (Transform child in guessesContainer)
        {
            Destroy(child.gameObject);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;

        inputField.text = "";
        inputField.ActivateInputField();


    }

    public void OnSubmitGuess()
    {
        string guess = inputField.text.ToUpper();
        if (guess.Length != 5)
        {
            return;
        }
        
        GameObject row = Instantiate(guessRowPrefab, guessesContainer);
        TMP_Text[] letters = row.GetComponentsInChildren<TMP_Text>();

        for (int i = 0; i < 5; i++)
        {
            //Tmp.text [i] = word[i].ToString();
            letters[i].text = guess[i].ToString();
            if (guess[i] == targetWord[i])
            {
                letters[i].color = Color.green;
            }
            else if (targetWord.Contains(guess[i].ToString()))
            {
                letters[i].color = Color.yellow;
            }
            else
            {
                letters[i].color = Color.gray;
            }
                
        }

        currentGuess++;
        inputField.text = "";
        inputField.ActivateInputField();

        if (guess == targetWord)
        {
            PassMinigame();
        }
        else if (currentGuess >= maxGuesses)
        {
            FailMinigame();
        }
    }

    private void PassMinigame()
    {
        HUDManager.Instance.ShowDefaultHUD();
        isActive = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
        
        float fuelUse = 5 * GameManager.Instance.fuelUseMult;
        int fuelCost = Mathf.RoundToInt(fuelUse);

        if (!FuelStorage.Instance.TryConsumeFuel(fuelCost))
        {
            // Not enough fuel to pay the penalty, game over
            GameManager.Instance.GameOver();
            return;
        }
        
        GameManager.Instance.AdvanceStage();
        GameManager.Instance.successfulMaintenance++;

        interactionSystem.enabled = true;

        GameManager.Instance.hasMaintainedGenerators = true;
        DayStageUI.Instance.UpdateStageDisplay(GameManager.Instance.currentStage);

        PauseMenu.Instance.SetIsPaused(false);
        HUDManager.Instance.InventoryLocked = false;
    }

    private void FailMinigame()
    {
        HUDManager.Instance.ShowDefaultHUD();
        isActive = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;

        if (GameManager.Instance.UpgradeUnlocked(GameManager.PlayerUpgrade.MaintenancePro))
        {
            float fuelUse = 15 * GameManager.Instance.fuelPenaltyMult;
            int fuelCost = Mathf.RoundToInt(fuelUse);

            if (!FuelStorage.Instance.TryConsumeFuel(fuelCost))
            {
                // Not enough fuel to pay the penalty, game over
                GameManager.Instance.GameOver();
                return;
            }
            
        }

        else
        {
            float fuelUse = 20 * GameManager.Instance.fuelPenaltyMult;
            int fuelCost = Mathf.RoundToInt(fuelUse);

            if (!FuelStorage.Instance.TryConsumeFuel(fuelCost))
            {
                // Not enough fuel to pay the penalty, game over
                GameManager.Instance.GameOver();
                return;
            }
        }

        GameManager.Instance.AdvanceStage();

        interactionSystem.enabled = true;

        GameManager.Instance.hasMaintainedGenerators = true;
        DayStageUI.Instance.UpdateStageDisplay(GameManager.Instance.currentStage);

        PauseMenu.Instance.SetIsPaused(false);
        HUDManager.Instance.InventoryLocked = false;
    }


    public void CloseMinigame()
    {
        cancelInput = false;

        HUDManager.Instance.ShowDefaultHUD();
        isActive = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;

        interactionSystem.enabled = true;

        PauseMenu.Instance.SetIsPaused(false);
        HUDManager.Instance.InventoryLocked = false;
        
        DayStageUI.Instance.UpdateStageDisplay(GameManager.Instance.currentStage);
    }
}
