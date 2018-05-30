using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.WorldMaterials;
using QGame;
using UnityEngine;

namespace Assets.WorldMaterials.Products
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

	    Product GetProduct(int productId);
	    Product GetProduct(string productName);
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
            if(_LookupScriptable != null)
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
		                return new Product
		                {
		                    Category = i.Category,
                            Name = i.Name,
                            ID = index,
                            Color = i.Color,
                            Icon = i.IconSprite ?? _LookupScriptable.DefaultSmallIcon
		                };
		            }
		        ).ToList();

		    _containers =
		        _LookupScriptable.Containers.Select(
		            i => new CraftingContainerInfo {CraftingSpeed = i.CraftingSpeed, Name = i.Name}).ToList();

            // real basic, should have better messaging on what is bad
            if(_LookupScriptable.Recipes.Any(i => i.Ingredients.Any(j => _LookupScriptable.Products.All(k => k.Name != j.ProductName))))
                throw new UnityException("ProductLookup: Recipe has unkown ingredient");

			var recipeLastId = 0;
		    _recipes = _LookupScriptable.Recipes.Select(i => new Recipe
		    {
				Id = ++recipeLastId,
				DisplayName = i.DisplayName,
		        Container = _containers.FirstOrDefault(j => j.Name == i.ContainerName),
		        Ingredients =
		            i.Ingredients.Select(ing => new Ingredient
		            {
		                ProductId = _products.First(j => j.Name == ing.ProductName).ID,
                        Quantity = ing.Quantity
		            }).ToList(),
			    Results = 
				    i.Results.Select(ing => new RecipeResult
				    {
					    ProductId = _products.First(j => j.Name == ing.ProductName).ID,
					    Quantity = ing.Quantity
				    }).ToList(),
		        TimeLength = i.TimeLength
		    }).ToList();
		}

		// I can't bring myself to delete this, it was like 20 minutes jamming on one statement
		//string GenerateDisplayText()
		//{
		//	var output = _products.Aggregate("Products:", (current, product) => current + "\n" + product.Name.ToString());
		//	if (_recipes.Any())
		//	{
		//		// Wowza.
		//		output = _recipes.Aggregate(output + "\n\nRecipes",
		//			(current1, recipe) => current1 + recipe.Value.Aggregate(
		//				string.Format("\n{0}:", recipe.Key), (current, rec) => current + rec.Ingredients.Aggregate(
		//					" ", (c, r) => c + string.Format("{0} {1} ", r.Quantity, r.ProductId))));
		//	}
		//	return output;
		//}

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

		public Recipe GetRecipe(int id)
		{
			return _recipes.FirstOrDefault(i => i.Id == id);
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
