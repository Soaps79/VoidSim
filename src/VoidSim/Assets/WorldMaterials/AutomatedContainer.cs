using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using Assets.Scripts.WorldMaterials;
using QGame;
using UnityEngine;

namespace Assets.WorldMaterials
{
    /// <summary>
    /// This class is used to automate crafting of products. 
    /// It is sort of a decorator for CraftingContainer. 
    /// It can be driven by either the player through a UI or an AI actor.
    /// Users must first Initialize() and then can start repeated crafts with BeginCrafting()
    /// </summary>
    public class AutomatedContainer : QScript
    {
        private string _containerName;
        private readonly List<Recipe> _recipes = new List<Recipe>();
        [SerializeField]
        private Inventory _inventory;

        // save for cancel
        private int _currentCraftId;

        private CraftingContainer _container;

        public void Initialize(string containerName, Inventory inventory)
        {
            _inventory = inventory;
            _containerName = containerName;
            if (_container == null)
            {
                _container = gameObject.GetOrAddComponent<CraftingContainer>();
                _container.Info = ProductLookup.Instance.GetContainer(_containerName);
                _container.OnCraftingComplete += RestartCrafting;
            }
            // tell container its type?
            LoadRecipes();
        }

        private void LoadRecipes()
        {
            var recipes = ProductLookup.Instance.GetRecipesForContainer(_containerName);
            if (!recipes.Any())
                throw new UnityException(string.Format("No recipes found for container {0}", _containerName));

            _recipes.Clear();
            _recipes.AddRange(recipes);
        }

        public void BeginCrafting(string productName)
        {
            _container.QueueCrafting(_recipes.FirstOrDefault(i => i.ResultProduct == productName));
        }

        private void RestartCrafting(Recipe recipe)
        {
            _inventory.TryAddProduct(ProductLookup.Instance.GetProduct(recipe.ResultProduct), recipe.ResultAmount);
            foreach (var ingredient in recipe.Ingredients)
            {
                if (_inventory.HasProduct(ingredient.ProductName, ingredient.Quantity))
                    continue;

                Debug.Log(string.Format("Automated container ran out of Product {0}", ingredient.ProductName));
                return;
            }

            foreach (var ingredient in recipe.Ingredients)
            {
                _inventory.TryRemoveProduct(ingredient.ProductName, ingredient.Quantity);
            }

            _currentCraftId = _container.QueueCrafting(recipe);
        }
    }
}
