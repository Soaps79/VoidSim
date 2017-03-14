using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.WorldMaterials
{
    /// <summary>
    /// Encapsulates conversion between editor friendly and game friendly product objects
    /// </summary>
    public class ProductConverter
    {
        public List<Product> ConvertToProducts(ProductLookup.ProductTableArray deserialized)
        {
            var list = new List<Product>();
            foreach (var product in deserialized.Products)
            {
                list.Add(new Product
                {
                    Category = product.Product.Category,
                    Name = product.Product.Name
                });
            }
            return list;
        }

        public Dictionary<ProductName, List<Recipe>> ConvertToRecipes(ProductLookup.ProductTableArray deserialized)
        {
            var table = new Dictionary<ProductName, List<Recipe>>();
            foreach (var product in deserialized.Products.Where(i => i.Recipes.Any()))
            {
                if (!table.ContainsKey(product.Product.Name))
                    table.Add(product.Product.Name, new List<Recipe>());

                foreach (var recipe in product.Recipes)
                {
                    table[product.Product.Name].Add(ConvertToRecipe(product.Product.Name, recipe));
                }
            }
            return table;
        }

        private Recipe ConvertToRecipe(ProductName product, RecipeModel model)
        {
            return new Recipe
            {
                ResultProduct = product,
                Ingredients = model.Ingredients.Select(ConvertToIngredient).ToList()
            };
        }

        private Ingredient ConvertToIngredient(IngredientModel model)
        {
            return new Ingredient
            {
                ProductName = model.ProductName,
                Quantity = model.Quantity
            };
        }
    }
}