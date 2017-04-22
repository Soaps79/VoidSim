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

    // This code was refactored to use ID's instead of strings
    // Should probably get a cleanup pass
    public class ProductFactory : PlaceableNode
    {
        public const string MessageName = "ProductFactoryPlaced";

        public override void BroadcastPlacement()
        {
            MessageHub.Instance.QueueMessage(MessageName, new ProductFactoryMessageArgs { ProductFactory = this } );
        }

        [SerializeField] private string _containerType;
        private readonly List<Recipe> _recipes = new List<Recipe>();
        private Inventory _inventory;
        public string CurrentlyCrafting;
        private ProductLookup _productLookup;

        // save for cancel
        private int _currentCraftId;

        private CraftingContainer _container;

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
            // tell container its type?
            LoadRecipes();
            if (!string.IsNullOrEmpty(CurrentlyCrafting))
            {
                _currentCraftId = _productLookup.GetProduct(CurrentlyCrafting).ID;
                BeginCrafting();
            }
                
        }

        private void LoadRecipes()
        {
            var recipes = _productLookup.GetRecipesForContainer(_containerType);
            if (!recipes.Any())
                throw new UnityException(string.Format("No recipes found for container {0}", _containerType));

            _recipes.Clear();
            _recipes.AddRange(recipes);
        }

        public void BeginCrafting()
        {
            _container.QueueCrafting(_recipes.FirstOrDefault(i => i.ResultProductID == _currentCraftId));
        }

        private void StoreProductAndRestartCrafting(Recipe recipe)
        {
            _inventory.TryAddProduct(recipe.ResultProductID, recipe.ResultAmount);
            foreach (var ingredient in recipe.Ingredients)
            {
                if (_inventory.HasProduct(ingredient.ProductId, ingredient.Quantity))
                    continue;

                Debug.Log(string.Format("Automated container ran out of Product {0}", ingredient.ProductId));
                return;
            }

            foreach (var ingredient in recipe.Ingredients)
            {
                _inventory.RemoveProduct(ingredient.ProductId, ingredient.Quantity);
            }

            _currentCraftId = _container.QueueCrafting(recipe);
        }
    }
}
