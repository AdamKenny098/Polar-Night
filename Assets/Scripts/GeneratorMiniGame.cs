// Author: Adam Kenny
// Student: Applied Computing (Game Development) 3rd Year (20102588)
// Date Created: 2025-08-15
// Description: Manages the generator minigame where players guess a word to maintain the generator.

using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GeneratorMinigame : MonoBehaviour
{
    public static GeneratorMinigame Instance;

    [Header("UI")]
    public TMP_InputField inputField;
    public Transform guessesContainer;
    public GameObject guessRowPrefab;

    [Header("Legacy Word List Fallback")]
    public string[] wordList;

    [Header("Word Database")]
    public GeneratorWordDatabase wordDatabase;
    public GeneratorWordCategory[] defaultWordCategories =
    {
        GeneratorWordCategory.Mechanical,
        GeneratorWordCategory.Emergency
    };

    private string targetWord;
    private int maxGuesses = 6;
    private int currentGuess = 0;
    private bool isActive = false;

    public bool cancelInput;

    [Header("External References")]
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

    private void Start()
    {
        if (inputField)
        {
            inputField.characterLimit = 5;
        }
    }

    private void Update()
    {
        if (!isActive)
        {
            return;
        }

        if (cancelInput)
        {
            if (interactionSystem)
            {
                interactionSystem.enabled = false;
            }

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void StartMinigame()
    {
        targetWord = ChooseTargetWord();

        if (string.IsNullOrWhiteSpace(targetWord))
        {
            Debug.LogError("Generator minigame could not find a valid 5-letter word.");
            return;
        }

        HUDManager.Instance.InventoryLocked = true;

        PauseMenu.Instance.SetIsPaused(true);
        cancelInput = true;
        HUDManager.Instance.ShowGeneratorHUD();

        currentGuess = 0;
        isActive = true;

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
        string guess = inputField.text.Trim().ToUpperInvariant();

        if (guess.Length != 5)
        {
            return;
        }

        GameObject row = Instantiate(guessRowPrefab, guessesContainer);
        TMP_Text[] letters = row.GetComponentsInChildren<TMP_Text>();

        for (int i = 0; i < 5; i++)
        {
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

    private string ChooseTargetWord()
    {
        if (wordDatabase)
        {
            string databaseWord = wordDatabase.GetRandomWord(5, defaultWordCategories);

            if (!string.IsNullOrWhiteSpace(databaseWord))
            {
                return databaseWord;
            }
        }

        List<string> validFallbackWords = new List<string>();

        if (wordList != null)
        {
            for (int i = 0; i < wordList.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(wordList[i]))
                {
                    continue;
                }

                string word = wordList[i].Trim().ToUpperInvariant();

                if (word.Length == 5)
                {
                    validFallbackWords.Add(word);
                }
            }
        }

        if (validFallbackWords.Count > 0)
        {
            return validFallbackWords[Random.Range(0, validFallbackWords.Count)];
        }

        return "FUSES";
    }

    private void PassMinigame()
    {
        EndMinigameInputLock();

        float fuelUse = 5 * GameManager.Instance.fuelUseMult;
        int fuelCost = Mathf.RoundToInt(fuelUse);

        if (!FuelStorage.Instance.TryConsumeFuel(fuelCost))
        {
            GameManager.Instance.GameOver();
            return;
        }

        GameManager.Instance.AdvanceStage();
        GameManager.Instance.successfulMaintenance++;

        GameManager.Instance.hasMaintainedGenerators = true;
        DayStageUI.Instance.UpdateStageDisplay(GameManager.Instance.currentStage);
    }

    private void FailMinigame()
    {
        EndMinigameInputLock();

        float fuelUse;

        if (GameManager.Instance.UpgradeUnlocked(GameManager.PlayerUpgrade.MaintenancePro))
        {
            fuelUse = 15 * GameManager.Instance.fuelPenaltyMult;
        }
        else
        {
            fuelUse = 20 * GameManager.Instance.fuelPenaltyMult;
        }

        int fuelCost = Mathf.RoundToInt(fuelUse);

        if (!FuelStorage.Instance.TryConsumeFuel(fuelCost))
        {
            GameManager.Instance.GameOver();
            return;
        }

        GameManager.Instance.AdvanceStage();

        GameManager.Instance.hasMaintainedGenerators = true;
        DayStageUI.Instance.UpdateStageDisplay(GameManager.Instance.currentStage);
    }

    public void CloseMinigame()
    {
        EndMinigameInputLock();
        DayStageUI.Instance.UpdateStageDisplay(GameManager.Instance.currentStage);
    }

    private void EndMinigameInputLock()
    {
        cancelInput = false;

        HUDManager.Instance.ShowDefaultHUD();
        isActive = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;

        if (interactionSystem)
        {
            interactionSystem.enabled = true;
        }

        PauseMenu.Instance.SetIsPaused(false);
        HUDManager.Instance.InventoryLocked = false;
    }
}