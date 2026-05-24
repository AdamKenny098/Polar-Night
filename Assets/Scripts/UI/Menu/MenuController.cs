// Author: Adam Kenny
// Student: Applied Computing (Game Development) 3rd Year (20102588)
// Date Created: 2025-08-22
// Description: Handles menu UI, settings management, and game scene transitions.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [Header("Audio Sliders")]
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    public TMP_Text volumeTextValue;
    public TMP_Text musicVolumeTextValue;
    public TMP_Text SFXVolumeTextValue;

    public Slider mouseSensitivitySlider;
    public TMP_Text mouseSensitivityTextValue;

    [Header("Scene Loading")]
    public string newGameLevel;
    public GameObject noSaveGameDialog;
    public GameObject confirmationPrompt;
    private GameManager gameManager;
    public static MenuController Instance;

    [Header("Difficulty Buttons")]
    public Button easyButton;
    public Button normalButton;
    public Button hardButton;
    public Button customButton;
    public Button startButton;
    public TMP_Text subtitle;

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

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        gameManager.enabled = true; // Ensure GameManager is active, wasn't in main menu

        easyButton.onClick.AddListener(() => SelectDifficulty(0));
        normalButton.onClick.AddListener(() => SelectDifficulty(1));
        hardButton.onClick.AddListener(() => SelectDifficulty(2));
        customButton.onClick.AddListener(() => SelectDifficulty(3));

        startButton.interactable = false;
    }

    void SelectDifficulty(int d)
    {
        switch (d)
        {
            case 0:
                gameManager.currentDifficulty = GameManager.Difficulty.Easy;
                subtitle.text = "For a more relaxed experience with less resource management.";
                break;
            case 1:
                gameManager.currentDifficulty = GameManager.Difficulty.Normal;
                subtitle.text = "A balanced experience.";
                break;
            case 2:
                gameManager.currentDifficulty = GameManager.Difficulty.Hard;
                subtitle.text = "For a challenging experience with bleaker hopes.";
                break;

            case 3:
                gameManager.currentDifficulty = GameManager.Difficulty.Custom;
                subtitle.text = "Custom difficulty. Play your way.";
                break;
        }


        startButton.interactable = true;
        gameManager.SetDifficulty(d);
    }


    // Starts a new game and loads the scene.
    public void NewGame()
    {
        gameManager.StartNewGame();
        LoadScene();
    }

    // Continues a game from the save file.
    public void ContinueGame()
    {
        SaveSystem.LoadGame();
        LoadScene();
    }

    // Handles dialog to continue a game.
    public void LoadGameDialogYes()
    {
        string savePath = Application.persistentDataPath + "/savefile.json";
        if (System.IO.File.Exists(savePath))
        {
            ContinueGame();
        }

        else
        {
            noSaveGameDialog.SetActive(true);
        }

    }

    // Handles dialog to start a new game.
    public void NewGameDialogYes()
    {
        GameManager.Instance.StartNewGame();
        SceneManager.LoadScene(newGameLevel);
    }

    // Exits the game application.
    public void ExitGame()
    {
        Application.Quit();
    }

    // Loads the target scene asynchronously.
    private void LoadScene()
    {
        SceneManager.LoadSceneAsync(newGameLevel);
    }
    
    public void Settings()
    {
        GameObject settings = GameObject.Find("PauseMenuUICanvas");
        settings.transform.GetChild(0).gameObject.SetActive(true);
        settings.transform.GetChild(2).gameObject.SetActive(true);
    }
}
