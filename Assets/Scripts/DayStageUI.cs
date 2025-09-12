// Author: Adam Kenny
// Student: Applied Computing (Game Development) 3rd Year (20102588)
// Date Created: 2025-07-16
// Description: Updates the UI to display the current game stage, task prompt, and countdown timer.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DayStageUI : MonoBehaviour
{
    public static DayStageUI Instance;
    public TextMeshProUGUI stageLabel;
    public TextMeshProUGUI taskPrompt;
    public TextMeshProUGUI countdownText;

    // Sets up the singleton instance.
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

    // Updates the UI when the scene starts.
    private void Start()
    {
        UpdateStageDisplay(GameManager.Instance.currentStage); // Runs after scene setup
    }

    // Updates the UI with the current stage and related task.
    public void UpdateStageDisplay(GameManager.DayStage stage)
    {
        stageLabel.text = "Stage: " + stage.ToString();

        switch (stage)
        {
            case GameManager.DayStage.Morning:
                taskPrompt.text = "Task: Maintain the generators";
                countdownText.gameObject.SetActive(false);
                break;

            case GameManager.DayStage.MidDay:
                taskPrompt.text = "Task: Scavenge outside";
                countdownText.gameObject.SetActive(true); // Show timer
                break;

            case GameManager.DayStage.Evening:
                if (GameManager.Instance.hasMaintainedGenerators)
                {
                    taskPrompt.text = "Task: Sleep";
                }
                else
                {
                    taskPrompt.text = "Task: Check generators";
                }
                countdownText.gameObject.SetActive(false);
                break;
        }
    }

    // Updates the countdown timer on the UI.
    public void UpdateCountdown(float secondsLeft)
    {
        int minutes = Mathf.FloorToInt(secondsLeft / 60);
        int seconds = Mathf.FloorToInt(secondsLeft % 60);
        countdownText.text = $"Time left: {minutes:D2}:{seconds:D2}";
    }
}
