
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Author: Adam Kenny
// Student: Applied Computing (Game Development) 3rd Year (20102588)
// Date Created: 2025-08-15
// Description: Manages the Heads-Up Display (HUD) in the game, including toggling visibility of various UI elements like instructions, countdown timer, task prompts, and day count.

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance;

    public GameObject hudRootObject;

    [Header("HUD Children")]
    public GameObject generatorHUD;
    public GameObject instructionsPanel;
    public GameObject interactHUD;
    public GameObject itemPickUpUI;
    public GameObject dayStateUI;
    public GameObject dayCounterText;
    public GameObject statIcons;

    public GameObject inventoryUICanvas;
    public GameObject pauseMenuUICanvas;

    [Header("Overlay Panels")]
    public GameObject pauseMenuOverlay;
    public GameObject settingsOverlay;
    public GameObject playerInventoryOverlay;
    public GameObject otherInventoryOverlay;
    public GameObject inventoryCloseButton;
    public GameObject upgradeUI;
    public GameObject pauseMenuUIDarkPanel;

    public GameObject settingsMenuTabs;

    public bool InventoryLocked = false;
    public void SetInventoryLocked(bool locked)
    {
        InventoryLocked = locked;
    }

    void Awake()
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
            hudRootObject = GameObject.Find("HUD");
            generatorHUD = hudRootObject.transform.GetChild(0).gameObject;
            instructionsPanel = hudRootObject.transform.GetChild(1).gameObject;
            interactHUD = hudRootObject.transform.GetChild(2).gameObject;
            itemPickUpUI = hudRootObject.transform.GetChild(3).gameObject;
            dayStateUI = hudRootObject.transform.GetChild(4).gameObject;
            dayCounterText = hudRootObject.transform.GetChild(5).gameObject;
            statIcons = hudRootObject.transform.GetChild(6).gameObject;


            pauseMenuUICanvas = GameObject.Find("PauseMenuUICanvas");
            pauseMenuUIDarkPanel = pauseMenuUICanvas.transform.GetChild(0).gameObject;
            pauseMenuOverlay = pauseMenuUICanvas.transform.GetChild(1).gameObject;
            settingsOverlay = pauseMenuUICanvas.transform.GetChild(2).gameObject;
            settingsMenuTabs = pauseMenuUICanvas.transform.GetChild(4).gameObject;

            inventoryUICanvas = GameObject.Find("InventoryUICanvas");
            playerInventoryOverlay = inventoryUICanvas.transform.GetChild(0).gameObject;
            otherInventoryOverlay = inventoryUICanvas.transform.GetChild(1).gameObject;
            inventoryCloseButton = inventoryUICanvas.transform.GetChild(2).gameObject;
            upgradeUI = inventoryUICanvas.transform.GetChild(3).gameObject;
        }
    }

    public void Start()
    {
        ShowDefaultHUD();
        HideAllOverlays();
    }

    public void SetHUDChildren(bool showGeneratorHUD, bool showInstructions, bool showInteractHUD, bool showItemPickUpUI, bool showDayStateUI, bool showDayCounterText, bool showStatIcons)
    {
        if (generatorHUD) generatorHUD.SetActive(showGeneratorHUD);
        if (instructionsPanel) instructionsPanel.SetActive(showInstructions);
        if (interactHUD) interactHUD.SetActive(showInteractHUD);
        if (itemPickUpUI) itemPickUpUI.SetActive(showItemPickUpUI);
        if (dayStateUI) dayStateUI.SetActive(showDayStateUI);
        if (dayCounterText) dayCounterText.SetActive(showDayCounterText);
        if (statIcons) statIcons.SetActive(showStatIcons);
    }

    public void HideAllHUD()
    {
        SetHUDChildren(false, false, false, false, false, false, false);
    }

    public void ShowDefaultHUD()
    {
        SetHUDChildren(false, false, true, true, true, true, true);
    }

    public void ShowGeneratorHUD()
    {
        SetHUDChildren(true, false, false, false, false, false, false);
    }

    public void ShowInstructions()
    {
        SetHUDChildren(false, true, false, false, false, false, false);
    }

    public void HideAllOverlays()
    {
        if (pauseMenuOverlay) pauseMenuOverlay.SetActive(false);
        if (settingsOverlay) settingsOverlay.SetActive(false);
        if (playerInventoryOverlay) playerInventoryOverlay.SetActive(false);
        if (otherInventoryOverlay) otherInventoryOverlay.SetActive(false);
        if (inventoryCloseButton) inventoryCloseButton.SetActive(false);
        if (upgradeUI) upgradeUI.SetActive(false);
        if (pauseMenuUIDarkPanel) pauseMenuUIDarkPanel.SetActive(false);
    }

    public void OpenPauseMenu()
    {
        SetInventoryLocked(true);
        HideAllOverlays();
        if (pauseMenuOverlay) pauseMenuOverlay.SetActive(true);
        if (pauseMenuUIDarkPanel) pauseMenuUIDarkPanel.SetActive(true);
        HideAllHUD();
        SetPausedState(true);
    }

    public void ClosePauseMenu()
    {
        HideAllOverlays();
        ShowDefaultHUD();
        SetPausedState(false);

        SetInventoryLocked(false);
        if (pauseMenuUIDarkPanel) pauseMenuUIDarkPanel.SetActive(false);
    }

    public void OpenSettings()
    {
        SetInventoryLocked(true);
        if (pauseMenuOverlay) pauseMenuOverlay.SetActive(false);
        if (settingsOverlay) settingsOverlay.SetActive(true);
        
    }

    public void CloseSettingsToPause()
    {
        SetInventoryLocked(true);
        if (settingsOverlay) settingsOverlay.SetActive(false);
        if (pauseMenuOverlay) pauseMenuOverlay.SetActive(true);

        
    }

    public void OpenPlayerInventory()
    {
        HideAllOverlays();
        if (playerInventoryOverlay) playerInventoryOverlay.SetActive(true);
        upgradeUI.SetActive(true);
        HideAllHUD();
        SetPausedState(true);
    }

    public void OpenDualInventory()
    {
        HideAllOverlays();
        if (playerInventoryOverlay) playerInventoryOverlay.SetActive(true);
        if (otherInventoryOverlay) otherInventoryOverlay.SetActive(true);
        if (inventoryCloseButton) inventoryCloseButton.SetActive(true);
        upgradeUI.SetActive(true);
        HideAllHUD();
        SetPausedState(true);
    }

    public void CloseInventory()
    {
        if (playerInventoryOverlay) playerInventoryOverlay.SetActive(false);
        if (otherInventoryOverlay) otherInventoryOverlay.SetActive(false);
        if (inventoryCloseButton) inventoryCloseButton.SetActive(false);
        if (upgradeUI) upgradeUI.SetActive(false);
        ShowDefaultHUD();
        SetPausedState(false);
    }


    private void SetPausedState(bool paused)
    {
        if (paused)
        {
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    
}
