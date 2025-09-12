// Author: Adam Kenny
// Student: Applied Computing (Game Development) 3rd Year (20102588)
// Date Created: 2025-07-16
// Description: Displays and updates the current day count on the UI.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DayCounterUI : MonoBehaviour
{
    public static DayCounterUI Instance;

    [SerializeField] private TextMeshProUGUI dayText;

    // Sets up the singleton instance for this UI.
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

    // Sets the initial day text at the start.
    private void Start()
    {
        UpdateDayText(GameManager.Instance.currentDay);
    }

    // Updates the displayed day count.
    public void UpdateDayText(int day)
    {
        dayText.text = $"Day {day}/14";
    }
}
