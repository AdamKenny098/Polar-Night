// Author: Adam Kenny
// Student: Applied Computing (Game Development) 3rd Year (20102588)
// Date Created: 2025-07-16
// Description: Interactable front door that manages entering/exiting the building, saving/loading inventories, and scene transitions.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FrontDoor : MonoBehaviour, IInteractable
{
    public Inventory playerInventory;
    public Inventory fridgeInventory;       // Optional, only in some scenes
    public Inventory fuelStoreInventory;    // Optional, only in some scenes

    public GameObject inventoryManager; // Reference to the InventoryManager GameObject

    public AudioSource doorAudio;
    public AudioClip doorOpenClip;

    private string currentSceneName;

    // Initializes inventory references and loads data at the start.
    void Start()
    {
        currentSceneName = SceneManager.GetActiveScene().name;
        inventoryManager = GameObject.Find("InventoryManager");
        playerInventory = GameManager.Instance.playerInventory;

        // Always try loading the player inventory
        if (playerInventory)
        {
            GameManager.Instance.LoadInventory(playerInventory, GameManager.Instance.playerInventoryData);
        }

        // Only try loading fridge/fuel if in the correct scene
        if (currentSceneName == "Inside")
        {
            if (fridgeInventory)
            {
                GameManager.Instance.LoadInventory(fridgeInventory, GameManager.Instance.fridgeInventoryData);
            }

            if (fuelStoreInventory && GameManager.Instance.fuelStoreInventoryData != null)
            {
                GameManager.Instance.LoadInventory(fuelStoreInventory, GameManager.Instance.fuelStoreInventoryData);
            }
        } 

        if (!playerInventory || !fridgeInventory || !fuelStoreInventory)
        {
            return;
        }

        // Load inventories from GameManager
        GameManager.Instance.LoadInventory(playerInventory, GameManager.Instance.playerInventoryData);
        GameManager.Instance.LoadInventory(fridgeInventory, GameManager.Instance.fridgeInventoryData);

        if (GameManager.Instance.fuelStoreInventoryData != null)
        {
            GameManager.Instance.LoadInventory(fuelStoreInventory, GameManager.Instance.fuelStoreInventoryData);
        }
        else
        {
            return;
        }
    }

    // Handles player interaction with the front door for entering/exiting.
    public void Interact()
    {
        if (currentSceneName == "Outside")
        {
            doorAudio.PlayOneShot(doorOpenClip);
            fuelStoreInventory = null;
            fridgeInventory = null;

            ReturnToBase();
        }
        else if (currentSceneName == "Inside")
        {
            if (GameManager.Instance.hasGoneOutside)
            {
                return;
            }
            else
            {
                doorAudio.PlayOneShot(doorOpenClip);
                GameManager.Instance.playerInventoryData = GameManager.Instance.SaveInventory(playerInventory);
                GameManager.Instance.fridgeInventoryData = GameManager.Instance.SaveInventory(fridgeInventory);
                GameManager.Instance.fuelStoreInventoryData = GameManager.Instance.SaveInventory(fuelStoreInventory);

                // Load the Outside scene and set the flag to true
                SceneManager.LoadScene("Outside");
                GameManager.Instance.hasGoneOutside = true; // Set the flag to true when going outside
            }
        }
    }

    // Handles logic for returning inside from outside.
    private void ReturnToBase()
    {
        // Save inventory before leaving
        GameManager.Instance.playerInventoryData = GameManager.Instance.SaveInventory(playerInventory);

        ScavengeTimer timer = FindObjectOfType<ScavengeTimer>();
        if (timer)
        {
            timer.PlayerReturnedToBase();
        }

        // Advance to evening phase
        GameManager.Instance.AdvanceStage();

        // Load back inside
        SceneManager.LoadScene("Inside");
    }
}
