// Author: Adam Kenny
// Student: Applied Computing (Game Development) 3rd Year (20102588)
// Date Created: 2025-07-16
// Description: Handles the UI logic for a single inventory slot, including setting icons, quantities, and click interactions.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class SlotUI : MonoBehaviour, IPointerClickHandler
{
    public Image icon;
    public TextMeshProUGUI amountText;
    public int slotIndex; // Set when you create the slot
    public InventoryUI ownerUI; // Reference to who owns this slot
    public Inventory inventoryOwner; // Reference to the inventory this slot belongs to

    // Gets the InventoryUI owner at start.
    public void Start()
    {
        ownerUI = GetComponentInParent<InventoryUI>();
    }

    // Sets the icon and amount text for this slot.
    public void Set(Item item, int amount)
    {
        if (item)
        {
            icon.enabled = true;
            icon.sprite = item.icon != null ? item.icon : null;

            if (amount > 1)
            {
                amountText.text = amount.ToString();
            }
            else
            {
                amountText.text = "";
            }
        }
        else
        {
            icon.enabled = false;
            icon.sprite = null; // Clears out old icons
            amountText.text = "";
        }
    }

    // Handles click events on this slot.
    public void OnClick()
    {
        InventoryUI.Instance.SelectSlot(inventoryOwner, slotIndex);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        InventorySlot slot = inventoryOwner.invSlots[slotIndex];

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (!slot.IsEmpty && slot.item.Name == "Rationed Meal")
            {
                GameManager.Instance.hasEaten = true;
                if (GameManager.Instance.UpgradeUnlocked(GameManager.PlayerUpgrade.SlowMetabolism))
                {
                    GameManager.Instance.RestoreHunger(3);
                }
                else
                {
                    GameManager.Instance.RestoreHunger(2);
                }

                slot.amount--;

                if (slot.amount <= 0)
                    slot.ClearSlot();

                PlayerNeedsUI.Instance.UpdateHungerUI(GameManager.Instance.currentHungerStage);

                InventoryUI.Instance.Refresh();
            }
        }
        else if (eventData.button == PointerEventData.InputButton.Left)
        {
            InventoryUI.Instance.SelectSlot(inventoryOwner, slotIndex);
        }
    }

}
