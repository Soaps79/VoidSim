using System.Collections.Generic;
using System.Linq;
using QGame;
using UnityEngine;

namespace Assets.WorldMaterials.Products
{
    public class ProductValueLookup : SingletonBehavior<ProductValueLookup>
    {
        /// <summary>
        /// Placeholder - This object's purpose is to provide a currency value to products in order 
        /// to get trade/currency up and running. These values should later be determined by 
        /// other factors playing out in the galaxy.
        /// </summary>
        public ProductValueScriptable Scriptable;
        private Dictionary<int, int> _productValueTable;

        void Awake()
        {
            PopulateFromScriptable();
        }

        private void PopulateFromScriptable()
        {
            if(Scriptable == null)
                throw new UnityException("ProductValueLookup has no scriptable");

            _productValueTable = new Dictionary<int, int>();
            var products = ProductLookup.Instance.GetProducts();
            products.ForEach(i => _productValueTable.Add(i.ID, 0));

            foreach (var entry in Scriptable.Products)
            {
                var product = products.FirstOrDefault(i => i.Name == entry.ProductName);
                if (product == null)
                {
                    Debug.Log("ProductValueLookupScriptable has bad product " + entry.ProductName);
                    continue;
                }

                _productValueTable[product.ID] = entry.Value;
            }
        }

        public int GetValueOfProduct(int productId)
        {
            return _productValueTable.ContainsKey(productId) ? _productValueTable[productId] : 0;
        }
    }
}