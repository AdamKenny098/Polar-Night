// Author: Adam Kenny
// Student: Applied Computing (Game Development) 3rd Year (20102588)
// Date Created: 2025-08-22
// Description: Manages the countdown timer for scavenge events and handles outcomes based on player actions.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScavengeTimer : MonoBehaviour
{
    public float scavengeTime; // seconds
    private float timer;
    private bool timerRunning = false;

    void Start()
    {
        StartScavengeTimer();
    }

    public void StartScavengeTimer()
    {
        if (GameManager.Instance.UpgradeUnlocked(GameManager.PlayerUpgrade.AdaptiveClothing))
        {
            scavengeTime = 450f; // 7.5 minutes
        }
        else
        {
            scavengeTime = 300f; // 5 minutes
        }

        timer = scavengeTime;
        timerRunning = true;

        DayStageUI.Instance.UpdateStageDisplay(GameManager.DayStage.MidDay);
        DayStageUI.Instance.UpdateCountdown(timer);
    }

    void Update()
    {
        if (!timerRunning) return;

        timer -= Time.deltaTime;
        DayStageUI.Instance.UpdateCountdown(timer);

        if (timer <= 0f)
        {
            timerRunning = false;
            TriggerFreezeDeath();
        }
    }

    public void PlayerReturnedToBase()
    {
        timerRunning = false;
    }

    void TriggerFreezeDeath()
    {
        GameManager.Instance.Freeze();
    }
}
