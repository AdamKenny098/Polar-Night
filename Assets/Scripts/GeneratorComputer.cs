// Author: Adam Kenny
// Student: Applied Computing (Game Development) 3rd Year (20102588)
// Date Created: 2025-07-16
// Description: Interactable generator computer that manages stage progression when maintained.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorComputer : MonoBehaviour, IInteractable
{
    [SerializeField] private bool hasBeenMaintainedThisStage = false;

    // Handles interaction with the generator computer.

    public void Interact()
    {
        if (hasBeenMaintainedThisStage)
        {
            return;
        }

        GameManager.DayStage stage = GameManager.Instance.currentStage;

        if (stage == GameManager.DayStage.MidDay)
        {
            return;
        }

        if (GameManager.Instance.currentStage == GameManager.DayStage.Morning ||
            GameManager.Instance.currentStage == GameManager.DayStage.Evening)
        {
            GeneratorMinigame.Instance.StartMinigame();
        }
    }
}
