using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Xml;
using System.Xml.Serialization;
using Assets.Scripts.UIHelpers;
using QGame;
using UnityEngine;
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;

namespace Assets.Scripts.WorldMaterials
{
	/// <summary>
	/// Exposes resources to the editor, other systems should build game logic objects
	/// based on the data from here. Take the data and go, it's arrays and arrays here.
	/// </summary>
	public class ProductLookup : QScript
	{
		[SerializeField]
		private ProductEditorView[] _startingProducts;

		private List<ProductEditorView> _products;

		void Awake()
		{
			OnNextUpdate += (delta) =>
			{
				var publisher = GetComponent<TextBindingPublisher>();
				publisher.SetText(GenerateDisplayText());
			};
		}

		string GenerateDisplayText()
		{
			var output = string.Empty;
			foreach (var product in _startingProducts)
			{
				output += string.Format("{0} : {1}", product.Product.Name, product.Product.Category);
				if (product.Recipes.Any())
				{
					output += string.Format(" Recipes: ");
					foreach (var recipe in product.Recipes)
					{
						output += "[ ";
						output = recipe.Ingredients.Aggregate(output, (current, ingredient) 
							=> current + string.Format("{0} {1} ", ingredient.Quantity, ingredient.ProductName));
						output += "] ";
					}
				}
				output += "\n";
			}

			return output.Remove(output.Length - 2);
		}

		public class ProductTable
		{
			public ProductEditorView[] Products;
		}

		public void SerializeData()
		{
			using (FileStream fs = File.Open("Resources/product_table.json", FileMode.Create, FileAccess.Write))
			using (StreamWriter sw = new StreamWriter(fs))
			using (JsonWriter jw = new JsonTextWriter(sw))
			{
				jw.Formatting = Formatting.Indented;
				var table = new ProductTable { Products = _startingProducts };
				JsonSerializer serializer = new JsonSerializer();
				serializer.Serialize(jw, table);
			}
		}
	}
}
