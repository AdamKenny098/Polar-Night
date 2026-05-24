using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class PolarNightDay4ASetupCraftButton
{
    [MenuItem("Tools/Polar Night/Day 4/4A Setup Craft Button")]
    public static void SetupCraftButton()
    {
        CraftingBenchUI ui = Object.FindObjectOfType<CraftingBenchUI>();

        if (!ui || !ui.panelRoot)
        {
            Debug.LogError("CraftingBenchUI or panelRoot missing. Finish Day 3 setup first.");
            return;
        }

        Button craftButton = FindOrCreateCraftButton(ui.panelRoot.transform);
        TMP_Text label = craftButton.GetComponentInChildren<TMP_Text>();

        ui.craftButton = craftButton;
        ui.craftButtonLabel = label;

        EditorUtility.SetDirty(ui);
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

        Debug.Log("Day 4A craft button setup complete.");
    }

    private static Button FindOrCreateCraftButton(Transform panelRoot)
    {
        Transform existing = panelRoot.Find("CraftButton");

        if (existing && existing.TryGetComponent(out Button existingButton))
        {
            return existingButton;
        }

        GameObject buttonObject = new GameObject("CraftButton");
        buttonObject.transform.SetParent(panelRoot, false);

        Image image = buttonObject.AddComponent<Image>();
        image.color = new Color(0.08f, 0.28f, 0.12f, 1f);

        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = image;

        RectTransform rect = buttonObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.68f, 0.10f);
        rect.anchorMax = new Vector2(0.92f, 0.22f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        GameObject labelObject = new GameObject("Label");
        labelObject.transform.SetParent(buttonObject.transform, false);

        TextMeshProUGUI label = labelObject.AddComponent<TextMeshProUGUI>();
        label.text = "Craft";
        label.fontSize = 26;
        label.color = Color.white;
        label.alignment = TextAlignmentOptions.Center;
        label.enableWordWrapping = false;
        label.raycastTarget = false;

        RectTransform labelRect = labelObject.GetComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;

        return button;
    }
}