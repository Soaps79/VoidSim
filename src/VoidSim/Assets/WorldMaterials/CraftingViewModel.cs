using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.WorldMaterials;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Facilitates binding the crafting system to the UI, which is complex because actions 
/// come from both sides (buttons trigger craft/cancel, container informs when finished)
/// Handles the game view of recipes and current queue.
/// 
/// Callback Flows
/// Init: Container > ViewModel > creates recipe buttons
/// Queue: RecipeButton > ViewModel > Container queues build > ViewModel > create queue button
/// Complete: Container > ViewModel > removes button from list
/// Cancel: QueueButton > ViewModel > Container cancels build > ViewModel > remove queue button
/// 
/// The flows with VM > Container > VM facilitate other actors being able to queue and cancel
/// </summary>
public class CraftingViewModel : MonoBehaviour
{
    private class QueuedButton
    {
        public int QueueID;
        public Button Button;
        public CraftingContainer.QueuedRecipe Recipe;
    }

    // set in CraftingContainer's prefab
    [SerializeField]
    private Button _craftingButtonPrefab;
    [SerializeField]
    private Button _queueButtonPrefab;
    [SerializeField]
    private Image _craftingPanelPrefab;

    // referenced when bound to UI
    private Image _recipesViewContext;
    private Image _queueViewContext;
    
    private List<Recipe> _recipes;
    private CraftingContainer _crafter;

    private readonly List<QueuedButton> _queuedButtons = new List<QueuedButton>();

    /// <summary>
    /// Typically called by the owner of the CraftingContainer
    /// </summary>
    public void Bind(List<Recipe> recipes, CraftingContainer crafter)
    {
        _crafter = crafter;
        _crafter.OnCraftingQueued += OnCraftingQueued;
        _crafter.OnCraftingCompleteUI += OnCraftingComplete;

        _recipes = recipes;
        BindToUI();
    }

    private void BindToUI()
    {
        // Instantiate the UI prefab
        var craftingPanel = GameObject.Instantiate(_craftingPanelPrefab);

        // get refs to recipes and queue views
        var scrollview = craftingPanel.transform.Find("content_holder/recipes_scroll_view");
        _recipesViewContext = scrollview.transform.Find("view_port/content").GetComponent<Image>();

        scrollview = craftingPanel.transform.Find("content_holder/queue_scroll_view");
        _queueViewContext = scrollview.transform.Find("view_port/content").GetComponent<Image>();

        if (_craftingButtonPrefab == null || _craftingPanelPrefab == null 
            || _recipesViewContext == null || _queueViewContext == null)
            throw new UnityException("Crafting UI ref missing");

        // create buttons for each recipe
        BindRecipes();

        // set the canvas and position. make positioning dynamic eventually
        PositionOnCanvas(craftingPanel);
    }

    private static void PositionOnCanvas(Image craftingPanel)
    {
        craftingPanel.rectTransform.position = new Vector3(45, 420, 0);
        var canvas = GameObject.Find("InfoCanvas");
        craftingPanel.transform.SetParent(canvas.transform);
    }

    // binds all recipes to the list of recipe buttons
    private void BindRecipes()
    {
        foreach (var recipe in _recipes)
        {
            var button = CreateRecipeButton(recipe);
            button.onClick.AddListener(() => { QueueCrafting(recipe); });
        }
    }

    // crafting complete, kill button
    private void OnCraftingComplete(CraftingContainer.QueuedRecipe queued)
    {
        var button = _queuedButtons.FirstOrDefault(i => i.QueueID == queued.ID);
        if(button == null)
            throw new UnityException(string.Format("Recipe {0} complete, has no button", queued.Recipe.ResultProduct));

        _queuedButtons.Remove(button);
        button.Button.gameObject.SetActive(false);
        Destroy(button.Button);
    }

    // add button to queued list, hook in cancel action
    private void OnCraftingQueued(CraftingContainer.QueuedRecipe queuedRecipe)
    {
        var button = CreateQueuedButton(queuedRecipe);
        var queued = new QueuedButton
        {
            Button = button, Recipe = queuedRecipe, QueueID = queuedRecipe.ID
        };
        _queuedButtons.Add(queued);
        button.onClick.AddListener(() =>
        {
            CancelCrafting(queued.QueueID);
        });
    }

    // tell crafter queuing was requested from button
    private void QueueCrafting(Recipe recipe)
    {
        _crafter.QueueCrafting(recipe);
    }

    // tell crafter cancel was requested from button
    private void CancelCrafting(int buttonId)
    {
        var queued = _queuedButtons.FirstOrDefault(i => i.QueueID == buttonId);
        _crafter.CancelCrafting(queued.Recipe);
        queued.Button.gameObject.SetActive(false);
        Destroy(queued.Button);
    }

    // creates a button in recipe list
    private Button CreateRecipeButton(Recipe recipe)
    {
        var button = GameObject.Instantiate(_craftingButtonPrefab).GetComponent<Button>();
        var text = button.GetComponentInChildren<Text>();
        text.text = GenerateText(recipe);
        button.gameObject.transform.SetParent(_recipesViewContext.transform);
        return button;
    }

    // create button in queue list
    private Button CreateQueuedButton(CraftingContainer.QueuedRecipe recipe)
    {
        var button = GameObject.Instantiate(_queueButtonPrefab).GetComponent<Button>();
        var text = button.GetComponentInChildren<Text>();
        text.text = GenerateText(recipe.Recipe);
        button.gameObject.transform.SetParent(_queueViewContext.transform);
        return button;
    }

    private string GenerateText(Recipe recipe)
    {
        return recipe.ResultProduct;
    }
}
