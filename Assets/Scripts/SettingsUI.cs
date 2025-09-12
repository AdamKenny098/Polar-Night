// Author: Adam Kenny
// Student: Applied Computing (Game Development) 3rd Year (20102588)
// Date Created: 2025-08-22
// Description: Manages the settings UI, including sliders for audio and sensitivity, and updates settings accordingly.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsUI : MonoBehaviour
{
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;
    public Slider sensitivitySlider;
    public Slider fuelUseMultiplierSlider;
    public Slider fuelPenaltyMultiplierSlider;
    public Slider lootMultiplierSlider;
    public Slider customFuelUseMultiplierSlider;
    public Slider customFuelPenaltyMultiplierSlider;
    public Slider customLootMultiplierSlider;

    public TMP_Text volumeTextValue;
    public TMP_Text musicVolumeTextValue;
    public TMP_Text SFXVolumeTextValue;
    public TMP_Text sensitivityTextValue;
    public TMP_Text fuelUseMultiplierTextValue;
    public TMP_Text fuelPenaltyMultiplierTextValue;
    public TMP_Text lootMultiplierTextValue;
    public TMP_Text customFuelUseMultiplierTextValue;
    public TMP_Text customFuelPenaltyMultiplierTextValue;
    public TMP_Text customLootMultiplierTextValue;




    void Start()
    {
        // Load initial slider values from saved settings
        masterSlider.value = SettingsManager.Instance.settings.masterVolume;
        musicSlider.value = SettingsManager.Instance.settings.musicVolume;
        sfxSlider.value = SettingsManager.Instance.settings.sfxVolume;

        sensitivitySlider.value = SettingsManager.Instance.settings.cameraSensitivity;

        fuelUseMultiplierSlider.value = GameManager.Instance.fuelUseMult;
        fuelPenaltyMultiplierSlider.value = GameManager.Instance.fuelPenaltyMult;
        lootMultiplierSlider.value = GameManager.Instance.lootMult;

        // Assign listeners dynamically
        masterSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);

        sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);

        fuelUseMultiplierSlider.onValueChanged.AddListener(OnFuelUseMultiplierChanged);
        fuelPenaltyMultiplierSlider.onValueChanged.AddListener(OnFuelPenaltyMultiplierChanged);
        lootMultiplierSlider.onValueChanged.AddListener(OnLootMultiplierChanged);

        volumeTextValue.text = masterSlider.value.ToString("0.0");
        musicVolumeTextValue.text = musicSlider.value.ToString("0.0");
        SFXVolumeTextValue.text = sfxSlider.value.ToString("0.0");

        sensitivityTextValue.text = sensitivitySlider.value.ToString("0");

        fuelUseMultiplierTextValue.text = fuelUseMultiplierSlider.value.ToString("0.0");
        fuelPenaltyMultiplierTextValue.text = fuelPenaltyMultiplierSlider.value.ToString("0.0");
        lootMultiplierTextValue.text = lootMultiplierSlider.value.ToString("0.0");
    }

    public void OnMasterVolumeChanged(float value)
    {
        volumeTextValue.text = value.ToString("0.0");
        SettingsManager.Instance.SetMasterVolume(value);
        SettingsManager.Instance.SaveSettings();
    }

    public void OnMusicVolumeChanged(float value)
    {
        musicVolumeTextValue.text = value.ToString("0.0");
        SettingsManager.Instance.SetMusicVolume(value);
        SettingsManager.Instance.SaveSettings();
    }

    public void OnSFXVolumeChanged(float value)
    {
        SFXVolumeTextValue.text = value.ToString("0.0");
        SettingsManager.Instance.SetSFXVolume(value);
        SettingsManager.Instance.SaveSettings();
    }

    public void OnSensitivityChanged(float value)
    {
        sensitivityTextValue.text = value.ToString("0");
        SettingsManager.Instance.SetCameraSensitivity(value);
        SettingsManager.Instance.SaveSettings();
    }

    public void OnFuelUseMultiplierChanged(float value)
    {
        fuelUseMultiplierTextValue.text = value.ToString("0.0");
        if (GameManager.Instance.currentDifficulty == GameManager.Difficulty.Custom)
        {
            GameManager.Instance.fuelUseMult = value;
            SettingsManager.Instance.SetFuelUseMultiplier(value);
        }
        SettingsManager.Instance.SaveSettings();
    }

    public void OnFuelPenaltyMultiplierChanged(float value)
    {
        fuelPenaltyMultiplierTextValue.text = value.ToString("0.0");

        if (GameManager.Instance.currentDifficulty == GameManager.Difficulty.Custom)
        {
            GameManager.Instance.fuelPenaltyMult = value;
            SettingsManager.Instance.SetFuelPenaltyMultiplier(value);
        }
    }

    public void OnLootMultiplierChanged(float value)
    {
        lootMultiplierTextValue.text = value.ToString("0.0");

        if (GameManager.Instance.currentDifficulty == GameManager.Difficulty.Custom)
        {
            GameManager.Instance.lootMult = value;
            SettingsManager.Instance.SetLootMultiplier(value);
        }
    }
}
