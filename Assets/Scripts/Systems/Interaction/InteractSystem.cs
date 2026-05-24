// Author: Adam Kenny
// Student: Applied Computing (Game Development) 3rd Year (20102588)
// Date Created: 2025-07-16
// Description: Handles player interaction with objects in the scene using raycasting and an interaction icon.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Interface for interactable objects.
public interface IInteractable
{
    void Interact();
}

public class InteractSystem : MonoBehaviour
{
    public static InteractSystem Instance;
    public Transform rayOrigin;      
    public float interactRange = 5f;  // How far the ray can reach
    public Image interactIcon;

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

    public IInteractable currentInteractable;

    // Checks for interactable objects every frame.
    void Update()
    {
        Ray ray = new Ray(rayOrigin.position, rayOrigin.forward);
        RaycastHit hit;

        // Default state: hide icon
        interactIcon.enabled = false;
        currentInteractable = null;

        if (Physics.Raycast(ray, out hit, interactRange))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable != null)
            {
                // Show the icon and prompt
                interactIcon.enabled = true;
                currentInteractable = interactable;

                if (Input.GetKeyDown(KeyCode.E))
                {
                    interactable.Interact();
                }
            }
        }
    }
}
