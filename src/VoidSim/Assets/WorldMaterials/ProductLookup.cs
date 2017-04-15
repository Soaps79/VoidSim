using System.Collections.Generic;
using System.IO;
using System.Linq;

using Assets.Scripts.UIHelpers;
using QGame;
using UnityEngine;
using Newtonsoft.Json;
using UnityEditor;
using Formatting = Newtonsoft.Json.Formatting;

namespace Assets.Scripts.WorldMaterials
{
	/// <summary>
	/// Repository for Products, Containers and Recipes loaded in the game.
	/// One instance should be exposed globally; concerned parties should manage their own versions of this data.
	/// </summary>
	public interface IProductLookup
	{
		List<Product> GetProducts();
		List<CraftingContainerInfo> GetContainers();
		List<Recipe> GetRecipes();
	}

	// TODO: Change from QScript to ScriptableObject, there's no need for this to Update()
	public class ProductLookup : SingletonBehavior<ProductLookup>, IProductLookup
	{
		
		// TODO: once there is more usage of this, reassess these containers and the interface
		private List<Product> _products;
		private List<CraftingContainerInfo> _containers;
		private List<Recipe> _recipes;

	    public ProductLookupScriptable _LookupScriptable;

        void Awake()
		{
            PopulateProductData();
		}

		// loads the SO into a more usable form
		private void PopulateProductData()
		{
            if (_LookupScriptable == null)
                throw new UnityException("ProductLookup has no scriptable");

            var index = 0;
		    _products = _LookupScriptable.Products.Select(i =>
		            {
		                index++;
		                return new Product {Category = i.Category, Name = i.Name, ID = index};
		            }
		        ).ToList();

		    _containers =
		        _LookupScriptable.Containers.Select(
		            i => new CraftingContainerInfo {CraftingSpeed = i.CraftingSpeed, Name = i.Name}).ToList();

		    _recipes = _LookupScriptable.Recipes.Select(i => new Recipe
		    {
		        Container = _containers.FirstOrDefault(j => j.Name == i.ContainerName),
		        Ingredients =
		            i.Ingredients.Select(k => new Ingredient {ProductName = k.ProductName, Quantity = k.Quantity}).ToList(),
		        ResultProduct = i.ResultProduct,
                ResultAmount = i.ResultAmount,
		        TimeLength = i.TimeLength
		    }).ToList();
		}

		string GenerateDisplayText()
		{
			return "";
            // I can't bring myself to delete this, it was like 20 minutes jamming on one statement
			//var output = _products.Aggregate("Products:", (current, product) => current + "\n" + product.Name.ToString());
			//if (_recipes.Any())
			//{
			//	// Wowza.
			//	output = _recipes.Aggregate(output + "\n\nRecipes", 
			//		(current1, recipe) => current1 + recipe.Value.Aggregate(
			//			string.Format("\n{0}:", recipe.Key), (current, rec) => current + rec.Ingredients.Aggregate(
			//				" ", (c, r) => c + string.Format("{0} {1} ", r.Quantity, r.ProductName))));
			//}

			//return output;
		}

		public List<Product> GetProducts()
		{
			return _products;
		}

        // Plan is to form interface as needs require
        // would like to avoid usage of getting full list so impl can be optimized
	    public Product GetProduct(string productName)
	    {
            // throw errors here for not found, or let users handle it?
	        var product = _products.FirstOrDefault(i => i.Name == productName);
	        return product;
	    }

        public Product GetProduct(int productId)
        {
            // throw errors here for not found, or let users handle it?
            return _products.First(i => i.ID == productId);
        }

        public List<CraftingContainerInfo> GetContainers()
		{
			return _containers;
		}

		public List<Recipe> GetRecipes()
		{
			return _recipes;
		}

	    public CraftingContainerInfo GetContainer(string containerName)
	    {
	        return _containers.FirstOrDefault(i => i.Name == containerName);
	    }

        public List<Recipe> GetRecipesForContainer(string containerName)
        {
            return _recipes.Where(i => i.Container.Name == containerName).ToList();
        }
    }
}
