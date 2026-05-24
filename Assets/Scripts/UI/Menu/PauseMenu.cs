// Author: Adam Kenny
// Student: Applied Computing (Game Development) 3rd Year (20102588)
// Date Created: 2025-07-16
// Description: Manages the pause menu, scene handling, and player state during pausing and resuming.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance;
    public GameObject pauseMenuUI;
    public Button resumeButton;
    public Button settingsButton;
    public Button menuButton;
    public GameObject settingsMenuUI;
    public GameObject player;
    public bool isPaused = false;

    public List<MonoBehaviour> scriptsToDisable = new List<MonoBehaviour>();

    public GameObject pauseMenuUICanvas;
    public GameObject settingsMenuTabs;

    // Sets up the singleton and makes the pause menu persistent.
    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persistent across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Initializes object references and sets the initial pause state based on the scene.
    void Start()
    {

        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == "Inside" || currentScene == "Outside")
        {
            player = GameObject.FindWithTag("Player");
        }
    }

    // Checks for pause/resume input and updates references.
    void Update()
    {
        if (isPaused) return;
        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene == "Main Menu" || currentScene == "GameOver")
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Time.timeScale == 0f)
            {
                Resume();
            }
            else
            {
                PauseGame();
            }
        }
    }

    // Registers scene loaded callback.
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Unregisters scene loaded callback.
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Handles logic when a new scene is loaded.
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        scriptsToDisable.Clear(); // Clear previous scripts to disable

        if (scene.name == "Outside" || scene.name == "Inside")
        {
            player = GameObject.FindWithTag("Player");
            if (player)
            {
                PlayerMovement movement = player.GetComponent<PlayerMovement>();
                FirstPersonCamera camera = player.GetComponentInChildren<FirstPersonCamera>();


                scriptsToDisable.Add(movement);
                scriptsToDisable.Add(camera);
            }

            settingsMenuTabs = pauseMenuUICanvas.transform.GetChild(4).gameObject;
        }

        if (scene.name == "Main Menu" || scene.name == "GameOver")
        {
            scriptsToDisable.Clear(); // Clear scripts to disable in main menu or game over
        }
    }

    // Resumes the game from pause.
    public void Resume()
    {
        if (isPaused) return;
        if (settingsMenuUI && settingsMenuUI.activeSelf)
        {
            return;
        }

        foreach (MonoBehaviour script in scriptsToDisable)
        {
            script.enabled = true; // Disable scripts to pause their functionality
        }

        HUDManager.Instance.ClosePauseMenu();

    }

    // Pauses the game.
    public void PauseGame()
    {
        if (isPaused) return;
        foreach (MonoBehaviour script in scriptsToDisable)
        {
            script.enabled = false; // Disable scripts to pause their functionality
        }

        HUDManager.Instance.OpenPauseMenu();
    }

    // Closes the settings menu and returns to pause menu.
    public void CloseSettings()
    {

    }

    public void OnBackPressed()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == "Main Menu")
        {
            GameObject menu = GameObject.Find("Main Menu Container");
            GameObject settings = GameObject.Find("PauseMenuUICanvas");
            menu.transform.GetChild(0).gameObject.SetActive(true);
            settings.transform.GetChild(0).gameObject.SetActive(false);
            settings.transform.GetChild(2).gameObject.SetActive(false);
            settings.transform.GetChild(3).gameObject.SetActive(false);
            settings.transform.GetChild(4).gameObject.SetActive(false);


        }
        else if (currentScene == "Inside" || currentScene == "Outside")
        {
            HUDManager.Instance.CloseSettingsToPause();
            GameObject settings = GameObject.Find("PauseMenuUICanvas");
            settings.transform.GetChild(3).gameObject.SetActive(false);
            settings.transform.GetChild(4).gameObject.SetActive(false);
        }
    }

    // Saves game and returns to the main menu.
    public void MainMenu()
    {
        SaveSystem.SaveGame();
        Time.timeScale = 1f;

        // Register scene-loaded callback
        SceneManager.sceneLoaded += OnMainMenuLoaded;

        SceneManager.LoadScene("Main Menu");
        HUDManager.Instance.HideAllOverlays();
    }

    public void ToggleInstructionsPanel()
    {
        if (HUDManager.Instance.instructionsPanel.activeSelf)
        {
            HUDManager.Instance.OpenPauseMenu();
        }
        else
        {
            HUDManager.Instance.HideAllOverlays();
            HUDManager.Instance.ShowInstructions();

        }
    }

    // Handles logic after loading the main menu scene.
    private void OnMainMenuLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Main Menu")
        {
            GameObject menu = GameObject.Find("MainMenuCanvas");
            if (menu)
            {
                menu.transform.GetChild(1).gameObject.SetActive(true);
            }

            // Unsubscribe so it doesn't run every time
            SceneManager.sceneLoaded -= OnMainMenuLoaded;
        }
    }

    public void SetIsPaused(bool paused)
    {
        isPaused = paused;
        if (paused && Time.timeScale == 0f)
        {
            HUDManager.Instance.ClosePauseMenu();
        }
    }

    public void DisplaySettingsTabs()
    {
        settingsMenuTabs = pauseMenuUICanvas.transform.GetChild(4).gameObject;
        if (GameManager.Instance.currentDifficulty == GameManager.Difficulty.Custom)
        {
            settingsMenuTabs.SetActive(true);
        }
    }

    public void HideSettingsTabs()
    {
        settingsMenuTabs = pauseMenuUICanvas.transform.GetChild(4).gameObject;
        if (GameManager.Instance.currentDifficulty == GameManager.Difficulty.Custom)
        {
            settingsMenuTabs.SetActive(false);
        }
    }
}
