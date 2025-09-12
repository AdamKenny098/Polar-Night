// Author: Adam Kenny
// Student: Applied Computing (Game Development) 3rd Year (20102588)
// Date Created: 2025-07-16
// Description: Represents a fridge in the game world that acts as a container and can be interacted with by the player.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fridge : MonoBehaviour, IInteractable
{
    public Inventory inventory;
    public AudioSource audioSource;
    public AudioClip openSound;

    // Ensures the fridge has an inventory component.
    private void Awake()
    {
        if (!inventory)
        {
            inventory = gameObject.AddComponent<Inventory>();
            inventory.slots = 10;
        }
    }

    // Handles player interaction with the fridge.
    public void Interact()
    {
        InventoryUI.Instance.OpenDualInventory(inventory);
        audioSource.PlayOneShot(openSound);
    }
}
