// Author: Adam Kenny
// Student: Applied Computing (Game Development) 3rd Year (20102588)
// Date Created: 2025-08-19
// Description: Manages the inventory UI for both player and container inventories, handles slot creation and item transfer logic. 
//              Also shows UI for player Upgrades

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance;

    [Header("References")]
    public GameObject slotPrefab;
    public Transform leftSlotParent;   // Player inventory
    public Transform rightSlotParent;  // Fridge or container
    public Transform upgradePanel;     // Upgrade icons panel
    public Transform closeButton;      // Close button for dual inventory
    public bool isPlayerInventoryOpen = false;

    [Header("Inventories")]
    public Inventory playerInventory;
    private Inventory otherInventory;

    private List<SlotUI> leftSlots = new List<SlotUI>();
    private List<SlotUI> rightSlots = new List<SlotUI>();

    [Header("Transfer Logic")]
    private Inventory originalInventory = null;
    private int originalSlotIndex = -1;

    [Header("Upgrade Icons")]
    public Button betterBootsIcon;
    public Button slowMetabolismIcon;
    public Button maintenanceProIcon;
    public Button adaptiveClothingIcon;

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
    
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            TogglePlayerInventory();
        }
    }

    // Opens or closes the player-only inventory UI.
    public void TogglePlayerInventory()
    {
        if (HUDManager.Instance.InventoryLocked)
        {
            return;
        }

        if (!isPlayerInventoryOpen)
        {
            HUDManager.Instance.OpenPlayerInventory();

            CreateSlots(leftSlotParent, leftSlots, playerInventory, 10);
            RefreshPlayerOnly();
            UpdateUpgradeButtons();
            isPlayerInventoryOpen = true;
        }
        else
        {
            HUDManager.Instance.CloseInventory();
            RefreshPlayerOnly();
            UpdateUpgradeButtons();
            isPlayerInventoryOpen = false;

        }
    }

    // Opens the dual inventory UI for interacting with containers.
    public void OpenDualInventory(Inventory containerInventory)
    {
        if (HUDManager.Instance.InventoryLocked)
        {
            return;
        }

        HUDManager.Instance.OpenDualInventory();

        otherInventory = containerInventory;
        CreateSlots(leftSlotParent, leftSlots, playerInventory, 10);
        CreateSlots(rightSlotParent, rightSlots, otherInventory, 10);

        Refresh();
        UpdateUpgradeButtons();
    }

    // Closes the dual inventory UI and clears selections.
    public void CloseDualInventory()
    {
        // Hide UI panels
        HUDManager.Instance.CloseInventory();

        // Clear slot lists and destroy slot objects
        foreach (Transform child in leftSlotParent)
        {
            Destroy(child.gameObject);
            leftSlots.Clear();
        }


        foreach (Transform child in rightSlotParent)
        {
            Destroy(child.gameObject);
            rightSlots.Clear();
        }
            

        // Clear any selected slot
        originalInventory = null;
        originalSlotIndex = -1;


        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Refreshes only the player inventory slots.
    private void RefreshPlayerOnly()
    {
        for (int i = 0; i < playerInventory.invSlots.Length; i++)
        {
            var slot = playerInventory.invSlots[i];
            leftSlots[i].Set(slot.item, slot.amount);
        }
    }

    // Refreshes both player and container inventory slots.
    public void Refresh()
    {
        for (int i = 0; i < playerInventory.invSlots.Length; i++)
        {
            var slot = playerInventory.invSlots[i];
            leftSlots[i].Set(slot.item, slot.amount);
        }

        for (int i = 0; i < otherInventory.invSlots.Length; i++)
        {
            var slot = otherInventory.invSlots[i];
            rightSlots[i].Set(slot.item, slot.amount);
        }
    }

    // Creates slot UI elements for an inventory.
    private void CreateSlots(Transform parent, List<SlotUI> slotList, Inventory inventory, int count)
    {
        foreach (Transform child in parent)
            Destroy(child.gameObject);

        slotList.Clear();

        for (int i = 0; i < count; i++)
        {
            GameObject obj = Instantiate(slotPrefab, parent);
            SlotUI slotUI = obj.GetComponent<SlotUI>();

            slotUI.slotIndex = i;
            slotUI.inventoryOwner = inventory;

            Button btn = obj.GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => slotUI.OnClick());

            slotList.Add(slotUI);
        }
    }

    // Handles selecting a slot and transferring items between inventories.
    public void SelectSlot(Inventory inventory, int slotIndex)
    {
        // Store the first click and not the second
        if (!originalInventory)
        {
            originalInventory = inventory;
            originalSlotIndex = slotIndex;
            return;
        }

        // if original is not the same as current
        if (originalInventory != inventory || originalSlotIndex != slotIndex)
        {
            Inventory targetInventory = inventory;

            InventorySlot aSlot = originalInventory.invSlots[originalSlotIndex];
            InventorySlot bSlot = targetInventory.invSlots[slotIndex];

            if (!aSlot.IsEmpty)
            {
                bool transferredItem = targetInventory.AddItem(aSlot.item, aSlot.amount);

                if (transferredItem)
                {
                    aSlot.ClearSlot();

                    // Optional: Save both inventories
                    GameManager.Instance.SaveInventory(originalInventory);
                    GameManager.Instance.SaveInventory(targetInventory);
                }
            }

            // Clear selection and refresh both sides
            originalInventory = null;
            originalSlotIndex = -1;
            Refresh();
        }
    }

    public void UpdateUpgradeButtons()
    {
        var upgrades = GameManager.Instance.unlockedUpgrades;

        betterBootsIcon.interactable = upgrades.Contains(GameManager.PlayerUpgrade.BetterBoots);
        slowMetabolismIcon.interactable = upgrades.Contains(GameManager.PlayerUpgrade.SlowMetabolism);
        maintenanceProIcon.interactable = upgrades.Contains(GameManager.PlayerUpgrade.MaintenancePro);
        adaptiveClothingIcon.interactable = upgrades.Contains(GameManager.PlayerUpgrade.AdaptiveClothing);
    }
}
