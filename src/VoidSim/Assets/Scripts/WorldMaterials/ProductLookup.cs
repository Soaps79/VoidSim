using System.Collections.Generic;
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
	/// Designed to expose a product management view to the editor, and then expose data to the game.
	/// The accompanying custom editor has a button to serialize, then this Lookup loads from file on Awake.
	/// One instance should be exposed globally; concerned parties should manage their own versions of this data.
	/// </summary>
	public interface IProductLookup
	{
		List<Product> GetProducts();
		List<Recipe> GetRecipes();
	}

	// TODO: Change from QScript to ScriptableObject, there's no need for this to Update()
	public class ProductLookup : QScript, IProductLookup
	{
		[SerializeField]
		private ProductEditorView[] _startingProducts;

		// TODO: once there is more usage of this, reassess these containers and the interface
		private List<Product> _products;
		private Dictionary<ProductName, List<Recipe>> _recipes;

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
			var deserialized = DeserializeData();
			if(deserialized == null)
				throw new UnityException("ProductLookup could not deserialize");

			var converter = new ProductConverter();
			_products = converter.ConvertToProducts(deserialized);
			_recipes = converter.ConvertToRecipes(deserialized);
		}

		string GenerateDisplayText()
		{
			var output = _products.Aggregate("Products:", (current, product) => current + "\n" + product.Name.ToString());
			if (_recipes.Any())
			{
				// Wowza.
				output = _recipes.Aggregate(output + "\n\nRecipes", 
					(current1, recipe) => current1 + recipe.Value.Aggregate(
						string.Format("\n{0}:", recipe.Key), (current, rec) => current + rec.Ingredients.Aggregate(
							" ", (c, r) => c + string.Format("{0} {1} ", r.Quantity, r.ProductName))));
			}


			return output;
		}

		#region Serialization
		public class ProductTableArray
		{
			public ProductEditorView[] Products;
		}

		private ProductTableArray DeserializeData()
		{
			var text = File.ReadAllText(@"Resources/product_table.json");
			var table = JsonConvert.DeserializeObject<ProductTableArray>(text);
			return table;
		}

		public void SerializeData()
		{
			using (FileStream fs = File.Open("Resources/product_table.json", FileMode.Create, FileAccess.Write))
			using (StreamWriter sw = new StreamWriter(fs))
			using (JsonWriter jw = new JsonTextWriter(sw))
			{
				jw.Formatting = Formatting.Indented;
				var table = new ProductTableArray { Products = _startingProducts };
				JsonSerializer serializer = new JsonSerializer();
				serializer.Serialize(jw, table);
			}
		}
		#endregion

		public List<Product> GetProducts()
		{
			return _products;
		}

		public List<Recipe> GetRecipes()
		{
			var list = new List<Recipe>();
			foreach (var recList in _recipes.Values)
			{
				list.AddRange(recList);
			}
			return list;
		}
	}
}
