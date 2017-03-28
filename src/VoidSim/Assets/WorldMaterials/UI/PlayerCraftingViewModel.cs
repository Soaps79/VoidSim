﻿using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.WorldMaterials;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.WorldMaterials.UI
{
    /// <summary>
    /// Acts as a gate between the player's inventory and a list of available objects
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

        /// <summary>
        /// Typically called by the owner of the CraftingContainer
        /// </summary>
        public void Bind(List<Recipe> recipes, CraftingContainer crafter, Inventory inventory)
        {
            _crafter = crafter;
            _crafter.OnCraftingQueued += OnCraftingQueued;
            _crafter.OnCraftingCompleteUI += OnCraftingComplete;

            _recipes = recipes;

            _inventory = inventory;
            _inventory.OnInventoryChanged += SetCanAffordOnButtons;

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
            SetCanAffordOnButtons();
            var go = new GameObject();

            // set the canvas and position. make positioning dynamic eventually
            PositionOnCanvas(craftingPanel);
        }

        private static void PositionOnCanvas(Image craftingPanel)
        {
            craftingPanel.rectTransform.position = new Vector3(10, 180, 0);
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

        // iterate through all buttons, disabling those we cannot afford
        private void SetCanAffordOnButtons()
        {
            foreach (var button in _recipeButtons)
            {
                if (!button.Recipe.Ingredients.All(
                    ingredient => _inventory.HasProduct(ingredient.ProductName, ingredient.Quantity)))
                {
                    button.Button.interactable = false;
                }
                else
                {
                    button.Button.interactable = true;
                }
            }
        }

        // crafting complete, kill button
        private void OnCraftingComplete(Recipe recipe, int id)
        {
            var button = _queuedButtons.FirstOrDefault(i => i.QueueID == id);
            if(button == null)
                throw new UnityException(string.Format("Recipe {0} complete, has no button", recipe.ResultProduct));

            _inventory.TryAddProduct(recipe.ResultProduct, recipe.ResultAmount);
            _queuedButtons.Remove(button);
            button.Button.gameObject.SetActive(false);
            Destroy(button.Button);
        }

        // add button to queued list, hook in cancel action
        private void OnCraftingQueued(Recipe recipe, int id)
        {
            foreach (var ingredient in recipe.Ingredients)
            {
                if(!_inventory.TryRemoveProduct(ingredient.ProductName, ingredient.Quantity))
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

        private void QueueCrafting(Recipe recipe)
        {
            _crafter.QueueCrafting(recipe);
        }

        // tell crafter to cancel, kill button
        private void CancelCrafting(int buttonId)
        {
            _crafter.CancelCrafting(buttonId);

            var queued = _queuedButtons.FirstOrDefault(i => i.QueueID == buttonId);
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
            _recipeButtons.Add(new RecipeButton { Button = button, Recipe = recipe });
            return button;
        }

        // create button in queue list
        private Button CreateQueuedButton(Recipe recipe)
        {
            var button = GameObject.Instantiate(_queueButtonPrefab).GetComponent<Button>();
            var text = button.GetComponentInChildren<Text>();
            text.text = GenerateText(recipe);
            button.gameObject.transform.SetParent(_queueViewContext.transform);
            return button;
        }

        private string GenerateText(Recipe recipe)
        {
            return recipe.ResultProduct;
        }
    }
}