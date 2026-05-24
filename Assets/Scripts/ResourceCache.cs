// Author: Adam Kenny
// Student: Applied Computing (Game Development) 3rd Year (20102588)
// Date Created: 2025-07-16
// Description: Acts as a resource cache the player can interact with to collect random items and updates UI feedback.

using UnityEngine;

public class ResourceCache : MonoBehaviour, IInteractable
{
    public int resourceAmount = 1;
    public Item resourceItem;

    [Header("Possible Resources")]
    public Item Food;
    public Item Fuel;
    public Item Material;

    [Header("References")]
    public Inventory playerInventory;
    public Animator animator;

    public bool resourcesDepleted = false;

    private void Start()
    {
        GameObject player = GameObject.Find("Player");

        if (player)
        {
            playerInventory = player.GetComponentInChildren<Inventory>();
        }

        animator = GetComponent<Animator>();

        int itemRoll = Random.Range(0, 3);

        switch (itemRoll)
        {
            case 0:
                resourceItem = Food;
                break;

            case 1:
                resourceItem = Fuel;
                break;

            case 2:
                resourceItem = Material;
                break;
        }
    }

    public void Interact()
    {
        if (resourcesDepleted)
        {
            return;
        }

        if (!resourceItem)
        {
            Debug.LogWarning($"{name} has no resource item assigned.");
            return;
        }

        if (!playerInventory)
        {
            Debug.LogWarning($"{name} could not find player inventory.");
            return;
        }

        int baseAmount = Mathf.Max(1, resourceAmount);
        float itemsGained = baseAmount * GameManager.Instance.lootMult;
        int itemsGot = Mathf.Max(1, Mathf.RoundToInt(itemsGained));

        bool added = playerInventory.AddItem(resourceItem, itemsGot);

        if (!added)
        {
            if (ItemPickUpUI.Instance)
            {
                ItemPickUpUI.Instance.ShowMessage("Inventory Full", resourceItem.icon);
            }

            return;
        }

        if (ItemPickUpUI.Instance)
        {
            ItemPickUpUI.Instance.ShowMessage($"+{itemsGot} {resourceItem.Name}", resourceItem.icon);
        }

        resourcesDepleted = true;

        if (animator)
        {
            animator.SetTrigger("ResourceCollected");
        }
    }

    public void SelfDestruct()
    {
        Destroy(gameObject);
    }
}