// Author: Adam Kenny
// Student: Applied Computing (Game Development) 3rd Year (20102588)
// Date Created: 2025-08-15
// Description: Manages the player's needs UI including hunger, temperature, and stamina indicators.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNeedsUI : MonoBehaviour
{
    public static PlayerNeedsUI Instance;

    [Header("UI References")]
    public Image hungerIcon;
    public Image tempIcon;
    public Image staminaIcon;

    [Header("Colors")]
    public Color green = Color.green;
    public Color yellow = Color.yellow;
    public Color orange = new Color(1f, 0.5f, 0f); 
    public Color red = Color.red;

    private Coroutine flashCoroutine;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        UpdateHungerUI(GameManager.Instance.currentHungerStage);
        UpdateTempUI(GameManager.Instance.amountOfFuel);
    }


    public void UpdateHungerUI(GameManager.HungerStage stage)
    {
        Debug.Log("🔍 Updating Hunger UI: " + stage);

        switch (stage)
        {
            case GameManager.HungerStage.Full:
                hungerIcon.color = Color.green;
                break;
            case GameManager.HungerStage.Hungry:
                hungerIcon.color = Color.yellow;
                break;
            case GameManager.HungerStage.Famished:
                hungerIcon.color = new Color(1f, 0.5f, 0f);
                break;
            case GameManager.HungerStage.Starving:
                hungerIcon.color = Color.red;
                break;
        }
    }


    public void UpdateTempUI(int filledFuelSlots)
    {
        if (filledFuelSlots >= 3)
            tempIcon.color = green;
        else if (filledFuelSlots >= 2)
            tempIcon.color = yellow;
        else if (filledFuelSlots >= 1)
            tempIcon.color = orange;
        else
            tempIcon.color = red;
    }


    // Call this every frame from PlayerMovement
    public void UpdateStaminaUI(bool isSprinting)
    {
        if (isSprinting)
        { 
            // Start flashing if not already
            if (flashCoroutine == null)
            {
                flashCoroutine = StartCoroutine(FlashStaminaIcon());
            }
        }
        else
        {
            // Stop flashing and set to static yellow
            if (flashCoroutine != null)
            {
                StopCoroutine(flashCoroutine);
                flashCoroutine = null;
            }

            staminaIcon.color = yellow;
        }
    }

    private IEnumerator FlashStaminaIcon()
    {
        while (true)
        {
            staminaIcon.color = yellow;
            yield return new WaitForSeconds(0.3f);
            staminaIcon.color = Color.clear; // Make it invisible to simulate flashing
            yield return new WaitForSeconds(0.3f);
        }
    }
}
