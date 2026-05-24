using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class PolarNightDay3ReadableUIPass
{
    [MenuItem("Tools/Polar Night/Day 3/Make Crafting UI Readable")]
    public static void MakeCraftingUIReadable()
    {
        CraftingBenchUI ui = Object.FindObjectOfType<CraftingBenchUI>();

        if (!ui)
        {
            Debug.LogError("No CraftingBenchUI found in the scene.");
            return;
        }

        if (!ui.panelRoot)
        {
            Debug.LogError("CraftingBenchUI has no panelRoot assigned.");
            return;
        }

        SetupPanel(ui.panelRoot);
        SetupTitle(ui);
        SetupRecipeCount(ui);
        SetupCloseButton(ui);

        EditorUtility.SetDirty(ui);
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

        Debug.Log("Crafting UI readability pass complete.");
    }

    private static void SetupPanel(GameObject panelRoot)
    {
        Image panelImage = panelRoot.GetComponent<Image>();

        if (!panelImage)
        {
            panelImage = panelRoot.AddComponent<Image>();
        }

        panelImage.color = new Color(0.02f, 0.025f, 0.03f, 0.98f);

        RectTransform panelRect = panelRoot.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.18f, 0.16f);
        panelRect.anchorMax = new Vector2(0.82f, 0.84f);
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;
    }

    private static void SetupTitle(CraftingBenchUI ui)
    {
        if (!ui.titleText)
        {
            Debug.LogWarning("CraftingBenchUI has no titleText assigned.");
            return;
        }

        ui.titleText.text = "Crafting Bench";
        ui.titleText.fontSize = 42;
        ui.titleText.color = Color.white;
        ui.titleText.alignment = TextAlignmentOptions.MidlineLeft;
        ui.titleText.enableWordWrapping = false;

        RectTransform rect = ui.titleText.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.08f, 0.78f);
        rect.anchorMax = new Vector2(0.92f, 0.94f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    private static void SetupRecipeCount(CraftingBenchUI ui)
    {
        if (!ui.recipeCountText)
        {
            Debug.LogWarning("CraftingBenchUI has no recipeCountText assigned.");
            return;
        }

        ui.recipeCountText.text = "Recipes Loaded: 4";
        ui.recipeCountText.fontSize = 30;
        ui.recipeCountText.color = new Color(0.82f, 0.9f, 1f, 1f);
        ui.recipeCountText.alignment = TextAlignmentOptions.MidlineLeft;
        ui.recipeCountText.enableWordWrapping = false;

        RectTransform rect = ui.recipeCountText.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.08f, 0.55f);
        rect.anchorMax = new Vector2(0.92f, 0.68f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    private static void SetupCloseButton(CraftingBenchUI ui)
    {
        if (!ui.closeButton)
        {
            Debug.LogWarning("CraftingBenchUI has no closeButton assigned.");
            return;
        }

        RectTransform buttonRect = ui.closeButton.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.68f, 0.10f);
        buttonRect.anchorMax = new Vector2(0.92f, 0.24f);
        buttonRect.offsetMin = Vector2.zero;
        buttonRect.offsetMax = Vector2.zero;

        Image buttonImage = ui.closeButton.GetComponent<Image>();

        if (buttonImage)
        {
            buttonImage.color = new Color(0.42f, 0.08f, 0.08f, 1f);
        }

        TMP_Text buttonText = ui.closeButton.GetComponentInChildren<TMP_Text>();

        if (buttonText)
        {
            buttonText.text = "Close";
            buttonText.fontSize = 26;
            buttonText.color = Color.white;
            buttonText.alignment = TextAlignmentOptions.Center;
            buttonText.enableWordWrapping = false;
        }
    }
}