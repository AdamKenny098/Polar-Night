// Author: Adam Kenny
// Student: Applied Computing (Game Development) 3rd Year (20102588)
// Date Created: 2025-07-16
// Description: Interactable front door that manages entering/exiting the building, saving/loading inventories, and scene transitions.

using UnityEngine;
using UnityEngine.SceneManagement;

public class FrontDoor : MonoBehaviour, IInteractable
{
    [Header("Inventories")]
    public Inventory playerInventory;
    public Inventory fridgeInventory;
    public Inventory fuelStoreInventory;

    [Header("Audio")]
    public AudioSource doorAudio;
    public AudioClip doorOpenClip;

    [Header("Debug")]
    public bool debugDoor = true;

    private string currentSceneName;

    private void Start()
    {
        currentSceneName = SceneManager.GetActiveScene().name;

        ResolveInventories();
        LoadRelevantInventories();
    }

    public void Interact()
    {
        if (debugDoor)
        {
            Debug.Log($"[FrontDoor] Interact called. Scene={currentSceneName}, hasGoneOutside={GameManager.Instance.hasGoneOutside}");
        }

        if (currentSceneName == "Outside")
        {
            PlayDoorAudio();
            ReturnToBase();
            return;
        }

        if (currentSceneName == "Inside")
        {
            TryGoOutside();
            return;
        }

        Debug.LogWarning($"[FrontDoor] Unsupported scene: {currentSceneName}");
    }

    private void ResolveInventories()
    {
        if (GameManager.Instance && GameManager.Instance.playerInventory)
        {
            playerInventory = GameManager.Instance.playerInventory;
        }

        if (!playerInventory)
        {
            GameObject player = GameObject.Find("Player");

            if (player)
            {
                playerInventory = player.GetComponentInChildren<Inventory>();
            }
        }
    }

    private void LoadRelevantInventories()
    {
        if (!GameManager.Instance)
        {
            Debug.LogWarning("[FrontDoor] No GameManager found.");
            return;
        }

        if (playerInventory)
        {
            GameManager.Instance.LoadInventory(playerInventory, GameManager.Instance.playerInventoryData);
        }
        else
        {
            Debug.LogWarning("[FrontDoor] No player inventory assigned/found.");
        }

        if (currentSceneName != "Inside")
        {
            return;
        }

        if (fridgeInventory)
        {
            GameManager.Instance.LoadInventory(fridgeInventory, GameManager.Instance.fridgeInventoryData);
        }
        else if (debugDoor)
        {
            Debug.Log("[FrontDoor] No fridge inventory assigned. Skipping fridge load.");
        }

        if (fuelStoreInventory && GameManager.Instance.fuelStoreInventoryData != null)
        {
            GameManager.Instance.LoadInventory(fuelStoreInventory, GameManager.Instance.fuelStoreInventoryData);
        }
        else if (debugDoor)
        {
            Debug.Log("[FrontDoor] No fuel storage inventory data or inventory assigned. Skipping fuel load.");
        }
    }

    private void TryGoOutside()
    {
        if (!GameManager.Instance)
        {
            Debug.LogWarning("[FrontDoor] Cannot go outside. GameManager missing.");
            return;
        }

        if (GameManager.Instance.hasGoneOutside)
        {
            if (debugDoor)
            {
                Debug.Log("[FrontDoor] Door blocked. Player has already gone outside this cycle.");
            }

            return;
        }

        if (!playerInventory)
        {
            Debug.LogWarning("[FrontDoor] Cannot go outside. Player inventory missing.");
            return;
        }

        PlayDoorAudio();

        GameManager.Instance.playerInventoryData = GameManager.Instance.SaveInventory(playerInventory);

        if (fridgeInventory)
        {
            GameManager.Instance.fridgeInventoryData = GameManager.Instance.SaveInventory(fridgeInventory);
        }

        if (fuelStoreInventory)
        {
            GameManager.Instance.fuelStoreInventoryData = GameManager.Instance.SaveInventory(fuelStoreInventory);
        }

        GameManager.Instance.hasGoneOutside = true;

        SceneManager.LoadScene("Outside");
    }

    private void ReturnToBase()
    {
        if (!GameManager.Instance)
        {
            Debug.LogWarning("[FrontDoor] Cannot return to base. GameManager missing.");
            return;
        }

        if (playerInventory)
        {
            GameManager.Instance.playerInventoryData = GameManager.Instance.SaveInventory(playerInventory);
        }
        else
        {
            Debug.LogWarning("[FrontDoor] Returning without saving player inventory because it is missing.");
        }

        ScavengeTimer timer = FindObjectOfType<ScavengeTimer>();

        if (timer)
        {
            timer.PlayerReturnedToBase();
        }

        GameManager.Instance.AdvanceStage();

        SceneManager.LoadScene("Inside");
    }

    private void PlayDoorAudio()
    {
        if (doorAudio && doorOpenClip)
        {
            doorAudio.PlayOneShot(doorOpenClip);
        }
    }
}