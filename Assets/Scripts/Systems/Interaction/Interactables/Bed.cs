// Author: Adam Kenny
// Student: Applied Computing (Game Development) 3rd Year (20102588)
// Date Created: 2025-07-16
// Description: Interactable bed that allows the player to sleep and advance to the next morning during the evening stage.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bed : MonoBehaviour, IInteractable
{

    // Handles player interaction with the bed.
    public void Interact()
    {
        if (GameManager.Instance.currentStage != GameManager.DayStage.Evening)
        {
            return;
        }

        GameManager.Instance.EndDay();
    }
}
