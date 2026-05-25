// Author: Adam Kenny
// Student: Applied Computing (Game Development) 3rd Year (20102588)
// Date Created: 2025-07-16
// Description: Interactable generator computer that manages stage progression when maintained.

using UnityEngine;

public class GeneratorComputer : MonoBehaviour, IInteractable
{
    [SerializeField] private bool hasBeenMaintainedThisStage = false;

    [Header("Preparation")]
    [SerializeField] private bool prepareBoardBeforeInteraction = true;
    [SerializeField] private float prepareCheckInterval = 1f;

    private float nextPrepareCheckTime;

    private void Start()
    {
        TryPrepareGeneratorRound();
    }

    private void Update()
    {
        if (!prepareBoardBeforeInteraction)
        {
            return;
        }

        if (Time.time < nextPrepareCheckTime)
        {
            return;
        }

        nextPrepareCheckTime = Time.time + prepareCheckInterval;
        TryPrepareGeneratorRound();
    }

    public void Interact()
    {
        if (!CanMaintainGenerator())
        {
            return;
        }

        if (GeneratorMinigame.Instance)
        {
            GeneratorMinigame.Instance.PrepareRound();
            GeneratorMinigame.Instance.StartMinigame();
        }
    }

    private void TryPrepareGeneratorRound()
    {
        if (!CanMaintainGenerator())
        {
            return;
        }

        if (!GeneratorMinigame.Instance)
        {
            return;
        }

        GeneratorMinigame.Instance.PrepareRound();
    }

    private bool CanMaintainGenerator()
    {
        if (hasBeenMaintainedThisStage)
        {
            return false;
        }

        if (!GameManager.Instance)
        {
            return false;
        }

        GameManager.DayStage stage = GameManager.Instance.currentStage;

        return stage == GameManager.DayStage.Morning ||
               stage == GameManager.DayStage.Evening;
    }
}