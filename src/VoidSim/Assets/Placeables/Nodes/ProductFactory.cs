using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using Assets.Scripts.WorldMaterials;
using Assets.WorldMaterials;
using Assets.WorldMaterials.Products;
using Messaging;
using UnityEngine;

// some Station owned object(Station) holds a reference to this
namespace Assets.Placeables.Nodes
{
    public class ProductFactoryMessageArgs : MessageArgs
    {
        public ProductFactory ProductFactory;
    }

    /// <summary>
    /// Gives placeable an automated crafting container
    /// </summary>
    public class ProductFactory : PlaceableNode
    {
        public const string MessageName = "ProductFactoryPlaced";

        // This code was refactored to use ID's instead of strings
        // Should probably get a cleanup pass
        
        // currently builds its initial item on repeat, needs work to switch between recipes
        public override void BroadcastPlacement()
        {
            MessageHub.Instance.QueueMessage(MessageName, new ProductFactoryMessageArgs { ProductFactory = this } );
        }

        [SerializeField] private string _containerType;
        private readonly List<Recipe> _recipes = new List<Recipe>();
        private int _currentCraftQueueId;
        private Inventory _inventory;

        public Recipe CurrentlyCrafting { get; private set; }

        public float CurrentCraftRemainingAsZeroToOne
        {
            get { return _container.CurrentCraftRemainingAsZeroToOne; }
        }

        // set in prefab
        public bool IsInPlayerArray;
        public string InitialRecipe;

        private ProductLookup _productLookup;
        
        public List<Recipe> Recipes { get { return _recipes; } }

        public bool IsCrafting { get; private set; }

        private CraftingContainer _container;

        // Factory binds to Inventory and initializes its container. Will begin crafting if InitialRecipe not null
        public void Initialize(Inventory inventory, ProductLookup productLookup)
        {
            _inventory = inventory;
            _productLookup = productLookup;
            if (_container == null)
            {
                _container = gameObject.GetOrAddComponent<CraftingContainer>();
                _container.Info = ProductLookup.Instance.GetContainer(_containerType);
                _container.OnCraftingComplete += StoreProductAndRestartCrafting;
            }

            LoadRecipes();
            if (!string.IsNullOrEmpty(InitialRecipe))
            {
                var recipe = _recipes.FirstOrDefault(i => i.ResultProductName == InitialRecipe);
                if (recipe == null)
                    throw new UnityException(string.Format("ProductFactory initialized with recipe it doesnt have: {0}", InitialRecipe));
                StartCrafting(recipe);
            }
        }

        // loads recipes for specified container type from product lookup
        private void LoadRecipes()
        {
            var recipes = _productLookup.GetRecipesForContainer(_containerType);
            if (!recipes.Any())
                throw new UnityException(string.Format("No recipes found for container {0}", _containerType));

            _recipes.Clear();
            _recipes.AddRange(recipes);
        }

        // public version, will switch recipes if it is already crafting one
        public void StartCrafting(int productId)
        {
            if(IsCrafting)
                StopCrafting();

            var recipe = _recipes.FirstOrDefault(i => i.ResultProductID == productId);
            if(recipe == null)
                throw new UnityException(string.Format("ProductFactory asked to make product that it has no recipe for, ID: ", productId));

            if(_currentCraftQueueId > 0)
                _container.CancelCrafting(_currentCraftQueueId);
            StartCrafting(recipe);
        }

        // stops crafting recipe and refunds ingredients
        public void StopCrafting()
        {
            if (!IsCrafting)
                return;

            RefundProducts();
            _container.CancelCrafting(_currentCraftQueueId);
            IsCrafting = false;
        }

        private void RefundProducts()
        {
            foreach (var ingredient in CurrentlyCrafting.Ingredients)
            {
                _inventory.TryAddProduct(ingredient.ProductId, ingredient.Quantity);
            }
        }

        // enables basic loop for continuous crafting
        private void StoreProductAndRestartCrafting(Recipe recipe)
        {
            StoreResult(recipe);
            StartCrafting(recipe);
        }

        // handles updating private data
        private void StartCrafting(Recipe recipe)
        {
            if (!TryWithdrawProducts(recipe)) return;
            _currentCraftQueueId = _container.QueueCrafting(recipe);
            CurrentlyCrafting = recipe;
            IsCrafting = true;
        }

        // if inventory has enough product for ingredients, remove them
        private bool TryWithdrawProducts(Recipe recipe)
        {
            foreach (var ingredient in recipe.Ingredients)
            {
                if (_inventory.HasProduct(ingredient.ProductId, ingredient.Quantity))
                    continue;

                Debug.Log(string.Format("Automated container ran out of Product {0}", ingredient.ProductId));
                return false;
            }

            foreach (var ingredient in recipe.Ingredients)
            {
                _inventory.TryRemoveProduct(ingredient.ProductId, ingredient.Quantity);
            }
            return true;
        }

        // add successful craft result to inventory
        private void StoreResult(Recipe recipe)
        {
            _inventory.TryAddProduct(recipe.ResultProductID, recipe.ResultAmount);
        }
    }
}
