// Author: Adam Kenny
// Student: Applied Computing (Game Development) 3rd Year (20102588)
// Date Created: 2025-07-16
// Description: Handles game settings such as audio, saving/loading preferences, and applying them globally.

using UnityEngine;
using UnityEngine.Audio;
using System.IO;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;
    public SettingsData settings = new SettingsData();
    public AudioMixer audioMixer;

    private string path;

    // Initializes the singleton, loads settings, and applies them.
    void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        path = Application.persistentDataPath + "/settings.json";
        AudioListener.volume = 0f;
        LoadSettings();
        ApplySettings();
        AudioListener.volume = 1f;
    }

    // Saves current settings to a JSON file.
    public void SaveSettings()
    {
        string json = JsonUtility.ToJson(settings, true);
        File.WriteAllText(path, json);
    }

    // Loads settings from a JSON file or sets defaults on first launch.
    public void LoadSettings()
    {
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            settings = JsonUtility.FromJson<SettingsData>(json);
        }
        else
        {
            // First launch — set defaults
            settings = new SettingsData();

            SaveSettings(); // Save the defaults so we don't run this again
        }
    }

    // Applies the current settings (such as audio) to the game.
    public void ApplySettings()
    {
        SetMasterVolume(settings.masterVolume);
        SetMusicVolume(settings.musicVolume);
        SetSFXVolume(settings.sfxVolume);
        SetCameraSensitivity(settings.cameraSensitivity);
    }

    // Sets master volume and updates audio mixer.
    public void SetMasterVolume(float value)
    {
        settings.masterVolume = value;
        audioMixer.SetFloat("Master", ConvertToDecibels(value));
    }

    // Sets music volume and updates audio mixer.
    public void SetMusicVolume(float value)
    {
        settings.musicVolume = value;
        audioMixer.SetFloat("MusicVolume", ConvertToDecibels(value));
    }

    // Sets SFX volume and updates audio mixer.
    public void SetSFXVolume(float value)
    {
        settings.sfxVolume = value;
        audioMixer.SetFloat("SFXVolume", ConvertToDecibels(value));
    }

    // Converts a volume value (0–1) to decibels for the audio mixer.
    private float ConvertToDecibels(float value)
    {
        return Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
    }

    public void SetCameraSensitivity(float value)
    {
        settings.cameraSensitivity = Mathf.Clamp(value, 0f, 500f); // <-- write to settings!
        var cam = FindObjectOfType<FirstPersonCamera>(true);
        if (cam)
        {
            cam.mouseSensitivity = settings.cameraSensitivity;
        }

    }
    public void ResetToDefaults()
    {
        settings = new SettingsData();
        ApplySettings();
    }

    public void SetFuelUseMultiplier(float value)
    {
        settings.fuelUseMultiplier = Mathf.Clamp(value, 0.1f, 5f);
        SaveSettings();
    }

    public void SetFuelPenaltyMultiplier(float value)
    {
        settings.fuelPenaltyMultiplier = Mathf.Clamp(value, 0.1f, 5f);
        SaveSettings();
    }

    public void SetLootMultiplier(float value)
    {
        settings.lootMultiplier = Mathf.Clamp(value, 0.1f, 5f);
        SaveSettings();
    }
}
