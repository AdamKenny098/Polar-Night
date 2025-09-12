// Author: Adam Kenny
// Student: Applied Computing (Game Development) 3rd Year (20102588)
// Date Created: 2025-07-16
// Description: Interactable radio static object that toggles audio mute state on interaction.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadioStatic : MonoBehaviour, IInteractable
{
    public AudioSource audioSource;

    // Called at the start of the script.
    void Start()
    {

    }

    // Called every frame.
    void Update()
    {

    }
    
    // Toggles audio mute when the radio is interacted with.
    public void Interact()
    {
        audioSource.mute = !audioSource.mute;
    }
}
