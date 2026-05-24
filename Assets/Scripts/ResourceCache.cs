// Author: Adam Kenny
// Student: Applied Computing (Game Development) 3rd Year (20102588)
// Date Created: 2025-07-16
// Description: Acts as a resource cache the player can interact with to collect random items and updates UI feedback.

using UnityEngine;

public class ResourceCache : MonoBehaviour, IInteractable
{
    [Header("Rolled Resource")]
    public int resourceAmount = 1;
    public Item resourceItem;

    [Header("Loot Table")]
    public ResourceLootTable lootTable;
    public bool rollOnStart = true;

    [Header("Fallback Possible Resources")]
    public Item Food;
    public Item Fuel;
    public Item Material;

    [Header("References")]
    public Inventory playerInventory;
    public Animator animator;

    public bool resourcesDepleted = false;

    private int rolledAmount = 0;
    private bool hasRolledLoot = false;

    private void Start()
    {
        FindPlayerInventory();

        animator = GetComponent<Animator>();

        if (rollOnStart)
        {
            RollResource();
        }
    }

    private void FindPlayerInventory()
    {
        GameObject player = GameObject.Find("Player");

        if (!player)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        if (player)
        {
            playerInventory = player.GetComponentInChildren<Inventory>();
        }
    }

    private void RollResource()
    {
        float lootMultiplier = GetLootMultiplier();

        if (lootTable && lootTable.TryGetRandomLoot(lootMultiplier, out Item rolledItem, out int amount))
        {
            resourceItem = rolledItem;
            rolledAmount = amount;
            hasRolledLoot = true;
            return;
        }

        RollFallbackResource(lootMultiplier);
    }

    private void RollFallbackResource(float lootMultiplier)
    {
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

        rolledAmount = CalculateAmount(resourceAmount, lootMultiplier);
        hasRolledLoot = true;
    }

    public void Interact()
    {
        if (resourcesDepleted)
        {
            return;
        }

        if (!playerInventory)
        {
            FindPlayerInventory();
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

        int itemsGot = hasRolledLoot
            ? Mathf.Max(1, rolledAmount)
            : CalculateAmount(resourceAmount, GetLootMultiplier());

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

    private int CalculateAmount(int baseAmount, float lootMultiplier)
    {
        int safeBaseAmount = Mathf.Max(1, baseAmount);
        int calculatedAmount = Mathf.RoundToInt(safeBaseAmount * lootMultiplier);

        return Mathf.Max(1, calculatedAmount);
    }

    private float GetLootMultiplier()
    {
        if (GameManager.Instance)
        {
            return GameManager.Instance.lootMult;
        }

        return 1f;
    }

    public void SelfDestruct()
    {
        Destroy(gameObject);
    }
}