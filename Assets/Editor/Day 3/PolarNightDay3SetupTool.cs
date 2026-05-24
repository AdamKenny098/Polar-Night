using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class PolarNightDay3SetupTool
{
    private const string RecipeBookPath = "Assets/Crafting/DemoCraftingRecipeBook.asset";

    [MenuItem("Tools/Polar Night/Day 3/Setup Crafting Bench UI And Stand-In")]
    public static void SetupCraftingBenchUIAndStandIn()
    {
        Canvas canvas = FindOrCreateCanvas();
        EnsureEventSystemExists();

        CraftingBenchUI craftingBenchUI = FindOrCreateCraftingBenchUI();
        GameObject panel = FindOrCreatePanel(canvas.transform);

        TMP_Text titleText = FindOrCreateText(
            panel.transform,
            "TitleText",
            "Crafting Bench",
            34,
            new Vector2(0.06f, 0.78f),
            new Vector2(0.94f, 0.94f),
            TextAlignmentOptions.MidlineLeft
        );

        TMP_Text recipeCountText = FindOrCreateText(
            panel.transform,
            "RecipeCountText",
            "Recipes Loaded: 0",
            24,
            new Vector2(0.06f, 0.5f),
            new Vector2(0.94f, 0.7f),
            TextAlignmentOptions.MidlineLeft
        );

        Button closeButton = FindOrCreateCloseButton(panel.transform);

        craftingBenchUI.panelRoot = panel;
        craftingBenchUI.titleText = titleText;
        craftingBenchUI.recipeCountText = recipeCountText;
        craftingBenchUI.closeButton = closeButton;

        EditorUtility.SetDirty(craftingBenchUI);

        GameObject benchObject = FindOrCreateStandInBench();
        CraftingBench bench = benchObject.GetComponent<CraftingBench>();

        if (!bench)
        {
            bench = benchObject.AddComponent<CraftingBench>();
        }

        bench.benchDisplayName = "Crafting Bench";
        bench.recipeBook = AssetDatabase.LoadAssetAtPath<CraftingRecipeBook>(RecipeBookPath);

        if (!bench.recipeBook)
        {
            Debug.LogWarning($"No CraftingRecipeBook found at {RecipeBookPath}. Create or move DemoCraftingRecipeBook there, then assign it to the bench.");
        }

        EditorUtility.SetDirty(bench);

        panel.SetActive(false);

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

        Debug.Log("Day 3 crafting bench UI and stand-in bench setup complete.");
    }

    private static Canvas FindOrCreateCanvas()
    {
        Canvas existingCanvas = Object.FindObjectOfType<Canvas>();

        if (existingCanvas)
        {
            return existingCanvas;
        }

        GameObject canvasObject = new GameObject("Canvas");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 50;

        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        canvasObject.AddComponent<GraphicRaycaster>();

        return canvas;
    }

    private static void EnsureEventSystemExists()
    {
        EventSystem existingEventSystem = Object.FindObjectOfType<EventSystem>();

        if (existingEventSystem)
        {
            return;
        }

        GameObject eventSystemObject = new GameObject("EventSystem");
        eventSystemObject.AddComponent<EventSystem>();
        eventSystemObject.AddComponent<StandaloneInputModule>();
    }

    private static CraftingBenchUI FindOrCreateCraftingBenchUI()
    {
        CraftingBenchUI existingUI = Object.FindObjectOfType<CraftingBenchUI>();

        if (existingUI)
        {
            return existingUI;
        }

        GameObject uiObject = new GameObject("CraftingBenchUI");
        return uiObject.AddComponent<CraftingBenchUI>();
    }

    private static GameObject FindOrCreatePanel(Transform canvasTransform)
    {
        Transform existingPanel = canvasTransform.Find("CraftingBenchPanel");

        if (existingPanel)
        {
            return existingPanel.gameObject;
        }

        GameObject panel = new GameObject("CraftingBenchPanel");
        panel.transform.SetParent(canvasTransform, false);

        Image image = panel.AddComponent<Image>();
        image.color = new Color(0.025f, 0.03f, 0.035f, 0.96f);

        RectTransform rect = panel.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.25f, 0.25f);
        rect.anchorMax = new Vector2(0.75f, 0.75f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        return panel;
    }

    private static TMP_Text FindOrCreateText(
        Transform parent,
        string objectName,
        string defaultText,
        int fontSize,
        Vector2 anchorMin,
        Vector2 anchorMax,
        TextAlignmentOptions alignment
    )
    {
        Transform existingText = parent.Find(objectName);

        if (existingText)
        {
            TMP_Text existingTmp = existingText.GetComponent<TMP_Text>();

            if (existingTmp)
            {
                return existingTmp;
            }
        }

        GameObject textObject = new GameObject(objectName);
        textObject.transform.SetParent(parent, false);

        TextMeshProUGUI text = textObject.AddComponent<TextMeshProUGUI>();
        text.text = defaultText;
        text.fontSize = fontSize;
        text.alignment = alignment;
        text.color = Color.white;
        text.enableWordWrapping = true;
        text.raycastTarget = false;

        RectTransform rect = textObject.GetComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        return text;
    }

    private static Button FindOrCreateCloseButton(Transform panelTransform)
    {
        Transform existingButton = panelTransform.Find("CloseButton");

        if (existingButton)
        {
            Button existingButtonComponent = existingButton.GetComponent<Button>();

            if (existingButtonComponent)
            {
                return existingButtonComponent;
            }
        }

        GameObject buttonObject = new GameObject("CloseButton");
        buttonObject.transform.SetParent(panelTransform, false);

        Image image = buttonObject.AddComponent<Image>();
        image.color = new Color(0.35f, 0.08f, 0.08f, 1f);

        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = image;

        RectTransform rect = buttonObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.7f, 0.08f);
        rect.anchorMax = new Vector2(0.94f, 0.22f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        GameObject labelObject = new GameObject("Label");
        labelObject.transform.SetParent(buttonObject.transform, false);

        TextMeshProUGUI label = labelObject.AddComponent<TextMeshProUGUI>();
        label.text = "Close";
        label.fontSize = 22;
        label.alignment = TextAlignmentOptions.Center;
        label.color = Color.white;
        label.raycastTarget = false;

        RectTransform labelRect = labelObject.GetComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;

        return button;
    }

    private static GameObject FindOrCreateStandInBench()
    {
        GameObject existingBench = GameObject.Find("Crafting Bench - Stand In");

        if (existingBench)
        {
            return existingBench;
        }

        GameObject bench = GameObject.CreatePrimitive(PrimitiveType.Cube);
        bench.name = "Crafting Bench - Stand In";
        bench.transform.localScale = new Vector3(1.8f, 0.35f, 0.8f);

        Vector3 position = new Vector3(0f, 0.65f, 2.5f);

        GameObject player = GameObject.Find("Player");

        if (!player)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        if (player)
        {
            Vector3 forward = player.transform.forward;
            forward.y = 0f;
            forward.Normalize();

            if (forward.sqrMagnitude < 0.01f)
            {
                forward = Vector3.forward;
            }

            position = player.transform.position + forward * 2.5f;
            position.y = player.transform.position.y + 0.65f;
        }

        bench.transform.position = position;

        Renderer renderer = bench.GetComponent<Renderer>();

        if (renderer)
        {
            Material material = new Material(Shader.Find("Standard"));
            material.color = new Color(0.18f, 0.16f, 0.13f, 1f);
            renderer.sharedMaterial = material;
        }

        return bench;
    }
}