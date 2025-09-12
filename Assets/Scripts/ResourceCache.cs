// Author: Adam Kenny
// Student: Applied Computing (Game Development) 3rd Year (20102588)
// Date Created: 2025-07-16
// Description: Acts as a resource cache the player can interact with to collect random items and updates UI feedback.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceCache : MonoBehaviour, IInteractable
{
    public int resourceAmount;
    public Item resourceItem;
    public Item Food;
    public Item Fuel;
    public Item Material;
    public Inventory playerInventory;
    public bool resourcesDepleted = false;

    public Animator animator;

    void Start()
    {

        playerInventory = GameObject.Find("Player").GetComponentInChildren<Inventory>();
        animator = gameObject.GetComponent<Animator>();
        int ItemRoll = Random.Range(0, 3);

        switch (ItemRoll)
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
            case 3:
                resourceItem = null;
                break;
        }
    }

    public void Interact()
    {
        float itemsGained = 1 * GameManager.Instance.lootMult;
        int itemsGot = Mathf.RoundToInt(itemsGained);

        playerInventory.AddItem(resourceItem, itemsGot);
        ItemPickUpUI.Instance.ShowMessage($"+{resourceAmount} {resourceItem.Name}", resourceItem.icon);
        resourcesDepleted = true;  
        if (resourcesDepleted)
        {
            animator.SetTrigger("ResourceCollected");
        }
    }

    public void SelfDestruct()
    {
        Destroy(gameObject);
    }

}
