using System.Collections.Generic;
using System.Linq;
using Assets.Controllers.GUI;
using Assets.Scripts;
using Assets.Scripts.WorldMaterials;
using Assets.WorldMaterials.Products;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.WorldMaterials.UI
{
    /// <summary>
    /// Acts as a gate between the player's inventory and a list of available crafts
    /// Facilitates binding the crafting system to the UI, which is complex because actions 
    /// come from both sides (buttons trigger craft/cancel, container informs when finished)
    /// Handles the game view of recipes and current queue.
    /// 
    /// Callback Flows
    /// Init: Container > ViewModel > creates recipe buttons
    /// Queue Build: RecipeButton > ViewModel > Container queues build > ViewModel > create queue button
    /// Complete Build: Container > ViewModel > removes button from list
    /// Cancel Build: QueueButton > ViewModel > Container cancels build > ViewModel > remove queue button
    /// 
    /// The flows with VM > Container > VM facilitate other actors being able to queue and cancel
    /// </summary>
    public class PlayerCraftingViewModel : MonoBehaviour
    {
        private class RecipeButton
        {
            public int QueueID;
            public Button Button;
            public Recipe Recipe;
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

        private Inventory _inventory;

        private readonly List<RecipeButton> _queuedButtons = new List<RecipeButton>();
        private readonly List<RecipeButton> _recipeButtons = new List<RecipeButton>();
        private RecipeButton _activeCraft;

        /// <summary>
        /// Typically called by the owner of the CraftingContainer
        /// </summary>
        public void Bind(List<Recipe> recipes, CraftingContainer crafter, Inventory inventory)
        {
            _crafter = crafter;
            _crafter.OnCraftingQueued += OnCraftingQueued;
            _crafter.OnCraftingCompleteUI += OnCraftingComplete;
            _crafter.OnCraftingBegin += StartProgressBar;

            _recipes = recipes;

            _inventory = inventory;
            _inventory.OnInventoryChanged += SetCanAffordOnButtons;

            BindToUI();
        }

        private void StartProgressBar(Recipe recipe, int id)
        {
            _activeCraft = _queuedButtons.FirstOrDefault(i => i.QueueID == id);
            if(_activeCraft == null)
                throw new UnityException("PlayerCraftingViewModel got bad things");

            var slider = _activeCraft.Button.transform.GetComponentInChildren<Slider>();
            var binding = slider.gameObject.AddComponent<SliderBinding>();
            binding.Initialize(() => _crafter.CurrentCraftRemainingAsZeroToOne);
        }

        private void BindToUI()
        {
            var canvas = Locator.CanvasManager.GetCanvas(CanvasType.LowUpdate);

            // Instantiate the UI prefab
            var craftingPanel = Instantiate(_craftingPanelPrefab, canvas.transform, false);

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
            SetCanAffordOnButtons();
            gameObject.RegisterSystemPanel(craftingPanel.gameObject);
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

        // iterate through all buttons, disabling those we cannot afford
        private void SetCanAffordOnButtons()
        {
            foreach (var button in _recipeButtons)
            {
                if (!button.Recipe.Ingredients.All(
                    ingredient => _inventory.HasProduct(ingredient.ProductId, ingredient.Quantity)))
                {
                    button.Button.interactable = false;
                }
                else
                {
                    button.Button.interactable = true;
                }
            }
        }

        // crafting complete, kill queue button
        private void OnCraftingComplete(Recipe recipe, int id)
        {
            var button = _queuedButtons.FirstOrDefault(i => i.QueueID == id);
            if(button == null)
                throw new UnityException(string.Format("Recipe {0} complete, has no button", recipe.DisplayName));

	        foreach (var result in recipe.Results)
	        {
		        _inventory.TryAddProduct(result.ProductId, result.Quantity);
			}
			_queuedButtons.Remove(button);
            Destroy(button.Button.gameObject);
        }

        // add button to queued list, hook in cancel action
        private void OnCraftingQueued(Recipe recipe, int id)
        {
            foreach (var ingredient in recipe.Ingredients)
            {
                if(_inventory.TryRemoveProduct(ingredient.ProductId, ingredient.Quantity) < ingredient.Quantity)
                    Debug.Log("Craft button requested good is could not afford");
            }
            var button = CreateQueuedButton(recipe);
            var queued = new RecipeButton
            {
                Button = button, Recipe = recipe, QueueID = id
            };
            _queuedButtons.Add(queued);
            button.onClick.AddListener(() =>
            {
                CancelCrafting(queued.QueueID);
            });
        }

        // hooked to recipe button, calls queue on the crafter, which triggers the above function
        // it's an odd sequence, but allows this view model to know of queue adds coming from elsewhere
        private void QueueCrafting(Recipe recipe)
        {
            _crafter.QueueCrafting(recipe);
        }

        // tell crafter to cancel, kill button
        private void CancelCrafting(int buttonId)
        {
            _crafter.CancelCrafting(buttonId);

            var queued = _queuedButtons.FirstOrDefault(i => i.QueueID == buttonId);
            Destroy(queued.Button.gameObject);
        }

        // creates a button in recipe list
        private Button CreateRecipeButton(Recipe recipe)
        {
            var button = Instantiate(_craftingButtonPrefab, _recipesViewContext.transform, false).GetComponent<Button>();
            var text = button.GetComponentInChildren<TextMeshProUGUI>();
            text.text = GenerateText(recipe);
            _recipeButtons.Add(new RecipeButton { Button = button, Recipe = recipe });
            return button;
        }

        // create button in queue list
        private Button CreateQueuedButton(Recipe recipe)
        {
            var button = Instantiate(_queueButtonPrefab, _queueViewContext.transform, false).GetComponent<Button>();
            var text = button.GetComponentInChildren<TextMeshProUGUI>();
            text.text = GenerateText(recipe);
            return button;
        }

        private string GenerateText(Recipe recipe)
        {
            return recipe.DisplayName;
        }
    }
}
