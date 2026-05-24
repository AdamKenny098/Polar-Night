// Author: Adam Kenny
// Student: Applied Computing (Game Development) 3rd Year (20102588)
// Date Created: 2025-07-16
// Description: Manages the UI for displaying floating text and icons when an item is picked up.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemPickUpUI : MonoBehaviour
{
    public static ItemPickUpUI Instance;

    [Header("Spawner Settings")]
    public GameObject floatingTextPrefab;
    public Transform textParent;

    // Sets up the singleton instance for this UI.
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        else
        {
           Destroy(gameObject); 
        } 
    }

    // Shows a floating message with an icon when an item is picked up.
    public void ShowMessage(string message, Sprite icon)
    {
        GameObject obj = Instantiate(floatingTextPrefab, textParent);

        TextMeshProUGUI tmpText = obj.GetComponentInChildren<TextMeshProUGUI>();
        Image image = obj.GetComponentInChildren<Image>();
        
        if (tmpText)
        {
            tmpText.text = message;
        }

        if (image)
        {
            image.sprite = icon;
        }
    }
}
