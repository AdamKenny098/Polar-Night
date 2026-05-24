using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftingBenchUI : MonoBehaviour
{
    public static CraftingBenchUI Instance;

    [Header("Main UI")]
    public GameObject panelRoot;
    public TMP_Text titleText;
    public TMP_Text recipeCountText;
    public Button closeButton;

    [Header("Day 3B Recipe UI")]
    public Transform recipeListParent;
    public TMP_Text recipeDetailsText;

    private CraftingBench activeBench;
    private Inventory activeInventory;

    private readonly List<GameObject> recipeButtons = new List<GameObject>();

    private void Awake()
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

        if (closeButton)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(Close);
        }

        if (panelRoot)
        {
            panelRoot.SetActive(false);
        }
    }

    private void Update()
    {
        if (panelRoot && panelRoot.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            Close();
        }
    }

    public void Open(CraftingBench bench, Inventory inventory)
    {
        activeBench = bench;
        activeInventory = inventory;

        if (!panelRoot)
        {
            Debug.LogWarning("CraftingBenchUI is missing panelRoot.");
            return;
        }

        panelRoot.SetActive(true);

        UpdateHeader();
        BuildRecipeList();
        ClearRecipeDetails();

        LockGameplay();
    }

    public void Close()
    {
        ClearRecipeButtons();

        activeBench = null;
        activeInventory = null;

        if (panelRoot)
        {
            panelRoot.SetActive(false);
        }

        UnlockGameplay();
    }

    private void UpdateHeader()
    {
        int recipeCount = activeBench ? activeBench.GetRecipes().Count : 0;

        if (titleText)
        {
            titleText.text = activeBench ? activeBench.benchDisplayName : "Crafting Bench";
        }

        if (recipeCountText)
        {
            recipeCountText.text = $"Recipes Loaded: {recipeCount}";
        }
    }

    private void BuildRecipeList()
    {
        ClearRecipeButtons();

        if (!recipeListParent || !activeBench)
        {
            return;
        }

        List<CraftingRecipe> recipes = activeBench.GetRecipes();

        for (int i = 0; i < recipes.Count; i++)
        {
            CraftingRecipe recipe = recipes[i];

            if (!recipe)
            {
                continue;
            }

            GameObject buttonObject = CreateRecipeButton(recipe);
            recipeButtons.Add(buttonObject);
        }
    }

    private GameObject CreateRecipeButton(CraftingRecipe recipe)
    {
        GameObject buttonObject = new GameObject(recipe.recipeName);
        buttonObject.transform.SetParent(recipeListParent, false);

        Image image = buttonObject.AddComponent<Image>();
        image.color = GetRecipeButtonColor(recipe);

        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(() => ShowRecipeDetails(recipe));

        LayoutElement layout = buttonObject.AddComponent<LayoutElement>();
        layout.preferredHeight = 46;
        layout.minHeight = 46;

        GameObject labelObject = new GameObject("Label");
        labelObject.transform.SetParent(buttonObject.transform, false);

        TextMeshProUGUI label = labelObject.AddComponent<TextMeshProUGUI>();
        label.text = GetRecipeButtonText(recipe);
        label.fontSize = 22;
        label.color = Color.white;
        label.alignment = TextAlignmentOptions.MidlineLeft;
        label.enableWordWrapping = false;
        label.raycastTarget = false;

        RectTransform labelRect = labelObject.GetComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = new Vector2(12, 0);
        labelRect.offsetMax = new Vector2(-12, 0);

        return buttonObject;
    }

    private string GetRecipeButtonText(CraftingRecipe recipe)
    {
        bool canCraft = activeInventory && recipe.CanCraft(activeInventory);
        string status = canCraft ? "READY" : "MISSING";

        return $"{recipe.recipeName} [{status}]";
    }

    private Color GetRecipeButtonColor(CraftingRecipe recipe)
    {
        bool canCraft = activeInventory && recipe.CanCraft(activeInventory);

        if (canCraft)
        {
            return new Color(0.08f, 0.22f, 0.12f, 1f);
        }

        return new Color(0.24f, 0.08f, 0.08f, 1f);
    }

    private void ShowRecipeDetails(CraftingRecipe recipe)
    {
        if (!recipeDetailsText || !recipe)
        {
            return;
        }

        StringBuilder builder = new StringBuilder();

        builder.AppendLine(recipe.recipeName);
        builder.AppendLine();

        if (!string.IsNullOrWhiteSpace(recipe.description))
        {
            builder.AppendLine(recipe.description);
            builder.AppendLine();
        }

        string outputName = recipe.outputItem ? recipe.outputItem.Name : "None";
        builder.AppendLine($"Output: {outputName} x{recipe.outputAmount}");
        builder.AppendLine();

        bool canCraft = activeInventory && recipe.CanCraft(activeInventory);
        builder.AppendLine(canCraft ? "Status: Craftable" : "Status: Missing requirements");
        builder.AppendLine();

        builder.AppendLine("Ingredients:");

        for (int i = 0; i < recipe.ingredients.Count; i++)
        {
            CraftingIngredient ingredient = recipe.ingredients[i];

            if (ingredient == null || !ingredient.item)
            {
                continue;
            }

            int owned = activeInventory ? activeInventory.CountItem(ingredient.item) : 0;
            int required = Mathf.Max(1, ingredient.amount);
            int missing = Mathf.Max(0, required - owned);

            if (missing <= 0)
            {
                builder.AppendLine($"- {ingredient.item.Name}: {owned}/{required}");
            }
            else
            {
                builder.AppendLine($"- {ingredient.item.Name}: {owned}/{required}  Missing {missing}");
            }
        }

        recipeDetailsText.text = builder.ToString();
    }

    private void ClearRecipeDetails()
    {
        if (recipeDetailsText)
        {
            recipeDetailsText.text = "Select a recipe.";
        }
    }

    private void ClearRecipeButtons()
    {
        for (int i = 0; i < recipeButtons.Count; i++)
        {
            if (recipeButtons[i])
            {
                Destroy(recipeButtons[i]);
            }
        }

        recipeButtons.Clear();
    }

    private void LockGameplay()
    {
        if (HUDManager.Instance)
        {
            HUDManager.Instance.SetInventoryLocked(true);
            HUDManager.Instance.HideAllHUD();
        }

        if (InteractSystem.Instance)
        {
            InteractSystem.Instance.enabled = false;
        }

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void UnlockGameplay()
    {
        if (HUDManager.Instance)
        {
            HUDManager.Instance.ShowDefaultHUD();
            HUDManager.Instance.SetInventoryLocked(false);
        }

        if (InteractSystem.Instance)
        {
            InteractSystem.Instance.enabled = true;
        }

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}