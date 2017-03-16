﻿using System.Collections.Generic;
using System.IO;
using System.Linq;

using Assets.Scripts.UIHelpers;
using QGame;
using UnityEngine;
using Newtonsoft.Json;
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
	    List<CraftingContainer> GetContainers();
        List<Recipe> GetRecipes();
	}

	// TODO: Change from QScript to ScriptableObject, there's no need for this to Update()
	public class ProductLookup : QScript, IProductLookup
	{
		
		// TODO: once there is more usage of this, reassess these containers and the interface
		private List<Product> _products;
	    private List<CraftingContainer> _containers;
		private List<Recipe> _recipes;

		void Awake()
		{
			// was having issues with publisher being available in Awake(), may be fixed?
			OnNextUpdate += (delta) =>
			{
				var publisher = GetComponent<TextBindingPublisher>();
				publisher.SetText(GenerateDisplayText());
			};

			PopulateProductData();
		}

		// loads from file, and remaps the editor types to a more usable form
		private void PopulateProductData()
		{
            // make this safer
            var json = LoadDataFromFile("products");
            _products = JsonConvert.DeserializeObject<Product[]>(json).ToList();

            json = LoadDataFromFile("containers");
            _containers = JsonConvert.DeserializeObject<List<CraftingContainer>>(json);

            json = LoadDataFromFile("recipes");
            _recipes = JsonConvert.DeserializeObject<List<Recipe>>(json);

		    int i = 0;
		}

		string GenerateDisplayText()
		{
		    return "";
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

        private string LoadDataFromFile(string fileName)
        {
            var text = File.ReadAllText(string.Format("Assets/Resources/{0}.json", fileName));
            return text;
        }

        public List<Product> GetProducts()
		{
			return _products;
		}

	    public List<CraftingContainer> GetContainers()
	    {
	        return _containers;
	    }

	    public List<Recipe> GetRecipes()
		{
			return _recipes;
		}
	}
}
