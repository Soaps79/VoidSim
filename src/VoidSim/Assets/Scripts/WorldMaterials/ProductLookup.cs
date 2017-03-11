using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.WorldMaterials
{
	/// <summary>
	/// Exposes resources to the editor, other systems should build game logic objects
	/// based on the data from here. Take the data and go, it's arrays and arrays here.
	/// </summary>
	public class ProductLookup : MonoBehaviour
	{
		[SerializeField]
		private ProductEditorView[] _startingProducts;

		private List<ProductEditorView> _products;

		[SerializeField]
		private TextBindingBehavior _outputText;

		void Start ()
		{
			_outputText.SetText(GenerateDisplayText());
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
	}
}
