// Author: Adam Kenny
// Student: Applied Computing (Game Development) 3rd Year (20102588)
// Date Created: 2025-08-15
// Description: Represents a fuel storage unit that can be interacted with to manage fuel inventory and display fuel levels visually.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelStorage : MonoBehaviour, IInteractable
{
    public Inventory fuelInventory;

    public static FuelStorage Instance;
    public GameObject[] fuelIndicators; //Beyond 3 has no effect
    public Item gasolineItem; // Reference to the gasoline item


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
        UpdateFuelVisuals();
    }

    private void Update()
    {
        UpdateFuelVisuals();
    }

    public void UpdateFuelVisuals()
    {
        int fuelCount = CountFuelInInventory();

        for (int i = 0; i < fuelIndicators.Length; i++)
        {
            fuelIndicators[i].SetActive(i < fuelCount);
        }
    }

    public int CountFuelInInventory()
    {
        int count = 0;
        foreach (var slot in fuelInventory.invSlots)
        {
            if (!slot.IsEmpty && slot.item.Name == "Gasoline")
            {
                count += slot.amount;
            }
        }

        // Clamp to max visual indicator length
        return count; //Mathf.Clamp(count, 0, fuelIndicators.Length);
    }

    public void Interact()
    {
        InventoryUI.Instance.OpenDualInventory(fuelInventory);
    }

    public bool TryConsumeFuel(int amount)
    {
        int totalConsumed = 0;

        for (int i = 0; i < fuelInventory.invSlots.Length; i++)
        {
            InventorySlot slot = fuelInventory.invSlots[i];

            if (!slot.IsEmpty && slot.item.Name == "Gasoline")
            {
                int take = Mathf.Min(slot.amount, amount - totalConsumed);
                slot.amount -= take;
                totalConsumed += take;

                if (slot.amount <= 0)
                    slot.ClearSlot();

                if (totalConsumed >= amount)
                {
                    return true; // Successfully removed enough
                }
            }

            GameManager.Instance.amountOfFuel = CountFuelInInventory();
        }

        return false; // Not enough fuel available
    }

    public void AddStartingFuel(int amount)
    {
        if (gasolineItem == null)
        {
            return;
        }

        InventorySlot firstSlot = fuelInventory.invSlots[0];

        firstSlot.item = gasolineItem;
        firstSlot.amount = amount;

        UpdateFuelVisuals();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.fuelStoreInventoryData = 
                GameManager.Instance.SaveInventory(fuelInventory);
            SaveSystem.SaveGame();
        }
    }

}
