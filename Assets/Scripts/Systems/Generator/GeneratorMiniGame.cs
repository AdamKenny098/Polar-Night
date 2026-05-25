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

    [Header("Generator Context")]
    public GeneratorRepairContext currentContext = GeneratorRepairContext.Normal;
    public bool useContextCategories = true;
    public bool autoResolveContext = true;
    public bool manualContextOverride = false;
    public int lowFuelThreshold = 3;

    [Header("Sticky Note Board")]
    public bool populateStickyNoteBoard = true;
    public int falseBoardWordCount = 5;

    [Header("Prepared Generator Round")]
    public bool roundPrepared;

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
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void PrepareRound()
    {
        if (roundPrepared)
        {
            return;
        }

        if (autoResolveContext && !manualContextOverride)
        {
            currentContext = ResolveContext();
        }

        targetWord = ChooseTargetWord();

        if (populateStickyNoteBoard)
        {
            PopulateStickyNoteBoard();
        }

        roundPrepared = true;

        Debug.Log($"Generator round prepared. Context: {currentContext}");
    }

    public void StartMinigame()
    {
        if (!roundPrepared)
        {
            PrepareRound();
        }

        if (string.IsNullOrWhiteSpace(targetWord))
        {
            Debug.LogError("Generator minigame could not find a valid 5-letter word.");
            return;
        }

        HUDManager.Instance.InventoryLocked = true;

        PauseMenu.Instance.SetIsPaused(true);
        cancelInput = true;
        LockInteractionInput();

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
        if (!isActive || string.IsNullOrWhiteSpace(targetWord))
        {
            return;
        }

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
            GeneratorWordCategory[] categories = useContextCategories
                ? GetCategoriesForContext(currentContext)
                : defaultWordCategories;

            string databaseWord = wordDatabase.GetRandomWord(5, categories);

            if (!string.IsNullOrWhiteSpace(databaseWord))
            {
                Debug.Log($"Generator context: {currentContext}. Word category pool selected.");
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
            Debug.LogWarning("Generator word database failed or had no valid word. Using fallback wordList.");
            return validFallbackWords[Random.Range(0, validFallbackWords.Count)];
        }

        Debug.LogWarning("Generator has no valid database word or fallback word. Using emergency fallback word: FUSES.");
        return "FUSES";
    }

    private GeneratorRepairContext ResolveContext()
    {
        int fuelCount = GetCurrentFuelCount();

        if (fuelCount <= lowFuelThreshold)
        {
            return GeneratorRepairContext.LowFuel;
        }

        if (GameManager.Instance && GameManager.Instance.currentStage == GameManager.DayStage.Evening)
        {
            return GeneratorRepairContext.Emergency;
        }

        return GeneratorRepairContext.Normal;
    }

    private int GetCurrentFuelCount()
    {
        if (FuelStorage.Instance)
        {
            return FuelStorage.Instance.CountFuelInInventory();
        }

        if (GameManager.Instance)
        {
            return GameManager.Instance.amountOfFuel;
        }

        return lowFuelThreshold + 1;
    }

    private GeneratorWordCategory[] GetCategoriesForContext(GeneratorRepairContext context)
    {
        switch (context)
        {
            case GeneratorRepairContext.LowFuel:
                return new[]
                {
                    GeneratorWordCategory.Emergency,
                    GeneratorWordCategory.Mechanical
                };

            case GeneratorRepairContext.Storm:
                return new[]
                {
                    GeneratorWordCategory.ColdWeather,
                    GeneratorWordCategory.Emergency
                };

            case GeneratorRepairContext.Anomaly:
                return new[]
                {
                    GeneratorWordCategory.Anomaly,
                    GeneratorWordCategory.Emergency
                };

            case GeneratorRepairContext.Containment:
                return new[]
                {
                    GeneratorWordCategory.Containment,
                    GeneratorWordCategory.Anomaly,
                    GeneratorWordCategory.Emergency
                };

            case GeneratorRepairContext.Emergency:
                return new[]
                {
                    GeneratorWordCategory.Emergency,
                    GeneratorWordCategory.Mechanical
                };

            default:
                return new[]
                {
                    GeneratorWordCategory.Mechanical,
                    GeneratorWordCategory.Emergency
                };
        }
    }

    public void SetRepairContext(GeneratorRepairContext context, bool lockContext = true)
    {
        currentContext = context;
        manualContextOverride = lockContext;
    }

    public void ClearRepairContextOverride()
    {
        manualContextOverride = false;
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
        ResetPreparedRound();

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
        ResetPreparedRound();

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
        isActive = false;

        HUDManager.Instance.ShowDefaultHUD();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;

        PauseMenu.Instance.SetIsPaused(false);
        HUDManager.Instance.InventoryLocked = false;

        UnlockInteractionInput();
    }

    private void LockInteractionInput()
    {
        InteractSystem.SetInputLocked(true);
    }

    private void UnlockInteractionInput()
    {
        InteractSystem.SetInputLocked(false, 0.35f);
    }

    private void PopulateStickyNoteBoard()
    {
        if (!GeneratorStickyNoteBoard.Instance)
        {
            Debug.LogWarning("No GeneratorStickyNoteBoard found in the scene.");
            return;
        }

        List<string> falseWords = GetFalseBoardWords();

        GeneratorStickyNoteBoard.Instance.FillBoard(targetWord, falseWords);
    }

    private List<string> GetFalseBoardWords()
    {
        List<string> falseWords = new List<string>();

        if (wordDatabase)
        {
            GeneratorWordCategory[] categories = useContextCategories
                ? GetCategoriesForContext(currentContext)
                : defaultWordCategories;

            List<string> possibleWords = wordDatabase.GetWords(5, categories);

            for (int i = 0; i < possibleWords.Count; i++)
            {
                string word = possibleWords[i].Trim().ToUpperInvariant();

                if (word == targetWord)
                {
                    continue;
                }

                if (falseWords.Contains(word))
                {
                    continue;
                }

                falseWords.Add(word);
            }
        }

        if (wordList != null)
        {
            for (int i = 0; i < wordList.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(wordList[i]))
                {
                    continue;
                }

                string word = wordList[i].Trim().ToUpperInvariant();

                if (word.Length != 5)
                {
                    continue;
                }

                if (word == targetWord)
                {
                    continue;
                }

                if (falseWords.Contains(word))
                {
                    continue;
                }

                falseWords.Add(word);
            }
        }

        ShuffleWords(falseWords);

        if (falseWords.Count > falseBoardWordCount)
        {
            falseWords.RemoveRange(falseBoardWordCount, falseWords.Count - falseBoardWordCount);
        }

        return falseWords;
    }

    private void ShuffleWords(List<string> words)
    {
        for (int i = 0; i < words.Count; i++)
        {
            int randomIndex = Random.Range(i, words.Count);

            string temp = words[i];
            words[i] = words[randomIndex];
            words[randomIndex] = temp;
        }
    }

    public void ResetPreparedRound()
    {
        roundPrepared = false;
        targetWord = "";

        if (GeneratorStickyNoteBoard.Instance)
        {
            GeneratorStickyNoteBoard.Instance.ClearBoard();
        }
    }
}