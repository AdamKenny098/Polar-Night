using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class PolarNightDay3BReadableSetup
{
    [MenuItem("Tools/Polar Night/Day 3/Setup Recipe List UI")]
    public static void SetupRecipeListUI()
    {
        CraftingBenchUI ui = Object.FindObjectOfType<CraftingBenchUI>();

        if (!ui || !ui.panelRoot)
        {
            Debug.LogError("CraftingBenchUI or panelRoot missing. Run the Day 3A setup first.");
            return;
        }

        GameObject panel = ui.panelRoot;

        CreateSectionLabel(panel.transform, "RecipeListHeader", "Recipes",
            new Vector2(0.08f, 0.68f),
            new Vector2(0.42f, 0.76f));

        Transform recipeList = CreatePanel(panel.transform, "RecipeListParent",
            new Vector2(0.08f, 0.28f),
            new Vector2(0.42f, 0.66f));

        CreateSectionLabel(panel.transform, "RecipeDetailsHeader", "Details",
            new Vector2(0.48f, 0.68f),
            new Vector2(0.92f, 0.76f));

        TMP_Text detailsText = CreateText(panel.transform, "RecipeDetailsText",
            "Select a recipe.",
            24,
            new Vector2(0.48f, 0.28f),
            new Vector2(0.92f, 0.66f));

        ui.recipeListParent = recipeList;
        ui.recipeDetailsText = detailsText;

        EditorUtility.SetDirty(ui);
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

        Debug.Log("Day 3B recipe list UI setup complete.");
    }

    private static Transform CreatePanel(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax)
    {
        Transform existing = parent.Find(name);

        if (existing)
        {
            return existing;
        }

        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent, false);

        Image image = panel.AddComponent<Image>();
        image.color = new Color(0.04f, 0.045f, 0.05f, 0.95f);

        RectTransform rect = panel.GetComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        VerticalLayoutGroup layout = panel.AddComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(10, 10, 10, 10);
        layout.spacing = 8;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;

        return panel.transform;
    }

    private static TMP_Text CreateSectionLabel(Transform parent, string name, string text, Vector2 anchorMin, Vector2 anchorMax)
    {
        return CreateText(parent, name, text, 28, anchorMin, anchorMax);
    }

    private static TMP_Text CreateText(
        Transform parent,
        string name,
        string text,
        int fontSize,
        Vector2 anchorMin,
        Vector2 anchorMax
    )
    {
        Transform existing = parent.Find(name);

        if (existing && existing.TryGetComponent(out TMP_Text existingText))
        {
            return existingText;
        }

        GameObject textObject = new GameObject(name);
        textObject.transform.SetParent(parent, false);

        TextMeshProUGUI tmp = textObject.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.TopLeft;
        tmp.enableWordWrapping = true;
        tmp.raycastTarget = false;

        RectTransform rect = textObject.GetComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        return tmp;
    }
}