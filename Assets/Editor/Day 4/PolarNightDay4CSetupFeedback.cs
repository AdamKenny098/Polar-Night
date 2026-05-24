using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class PolarNightDay4CSetupFeedback
{
    [MenuItem("Tools/Polar Night/Day 4/4C Setup Craft Feedback")]
    public static void SetupCraftFeedback()
    {
        CraftingBenchUI ui = Object.FindObjectOfType<CraftingBenchUI>();

        if (!ui || !ui.panelRoot)
        {
            Debug.LogError("CraftingBenchUI or panelRoot missing.");
            return;
        }

        TMP_Text feedbackText = FindOrCreateFeedbackText(ui.panelRoot.transform);

        ui.craftingFeedbackText = feedbackText;

        EditorUtility.SetDirty(ui);
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

        Debug.Log("Day 4C craft feedback setup complete.");
    }

    private static TMP_Text FindOrCreateFeedbackText(Transform panelRoot)
    {
        Transform existing = panelRoot.Find("CraftingFeedbackText");

        if (existing && existing.TryGetComponent(out TMP_Text existingText))
        {
            return existingText;
        }

        GameObject textObject = new GameObject("CraftingFeedbackText");
        textObject.transform.SetParent(panelRoot, false);

        TextMeshProUGUI text = textObject.AddComponent<TextMeshProUGUI>();
        text.text = "";
        text.fontSize = 24;
        text.color = Color.white;
        text.alignment = TextAlignmentOptions.MidlineLeft;
        text.enableWordWrapping = false;
        text.raycastTarget = false;

        RectTransform rect = textObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.48f, 0.18f);
        rect.anchorMax = new Vector2(0.92f, 0.26f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        return text;
    }
}