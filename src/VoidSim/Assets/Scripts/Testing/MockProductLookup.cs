using System.Collections.Generic;
using System.Linq;
using Assets.WorldMaterials.Products;
using UnityEngine;

namespace Assets.Scripts.Testing
{
    public class MockProductLookup : MonoBehaviour, IProductLookup
    {
        private List<Product> _products = new List<Product>();
        private List<CraftingContainerInfo> _containers = new List<CraftingContainerInfo>();
        private List<Recipe> _recipes = new List<Recipe>();

        public List<Product> GetProducts()
        {
            return _products;
        }

        public void AddProduct(Product product)
        {
            _products.Add(product);
        }

        public List<CraftingContainerInfo> GetContainers()
        {
            return _containers;   
        }

        public List<Recipe> GetRecipes()
        {
            return _recipes;
        }

        public Product GetProduct(int productId)
        {
            return _products.FirstOrDefault(i => i.ID == productId);
        }

        public Product GetProduct(string productName)
        {
            return _products.FirstOrDefault(i => i.Name == productName);
        }
    }
}