using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using QGame;
using UnityEngine;
using UnityEngine.VR;
using Zenject;

namespace Assets.Scripts.WorldMaterials
{
    public class Inventory : QScript
    {
        [Serializable]
        public class ProductEntryInfo
        {
            public string ProductName;
            public int Amount;
        }

        [Serializable]
        public class InventoryInfo
        {
            public string Name;
            public List<ProductEntryInfo> Products;
        }

        /// <summary>
        /// The idea here is that Products are one type of object that Inventory maintains.
        /// Another type will probably be some sort of Placeable.
        /// </summary>
        private class InventoryProductEntry
        {
            public Product Product;
            public int Amount;
            //public int MaxAmount;
        }

        [Inject]
        private ProductLookup _productLookup;
        public string Name;
        public bool LoadFromFile;

        private readonly Dictionary<int, InventoryProductEntry> _productTable 
            = new Dictionary<int, InventoryProductEntry>();

        protected override void OnStart()
        {
            if (LoadFromFile)
            {
                var json = LoadDataFromFile(Name);
                Deserialize(json);
            }
        }

        /// <summary>
        /// Add Products to an inventory
        /// TODO: Returning false if Product is full. May require method to consume to max and refund rest?
        /// </summary>
        public bool TryAddProduct(Product product, int amount)
        {
            if (!_productTable.ContainsKey(product.ID))
            {
                _productTable.Add(product.ID, new InventoryProductEntry()
                {
                    Product = product,
                    Amount = amount
                });
            }

            // impl MaxAmount checks
            _productTable[product.ID].Amount += amount;
            return true;
        }

        // early interface pass... update as usage requires
        public bool TryRemoveProduct(int productId, int amount)
        {
            if (!HasProduct(productId, amount))
                return false;

            _productTable[productId].Amount -= amount;
                return true;
        }

        public bool HasProduct(int productId, int amount)
        {
            return _productTable.ContainsKey(productId) && _productTable[productId].Amount >= amount;
        }

        public class Factory : Factory<Inventory>
        {
        }

        // need to extract serializtion
        public string Serialize()
        {
            var list = _productTable.Select(
                i => new ProductEntryInfo {ProductName = i.Value.Product.Name, Amount = i.Value.Amount});
            return JsonConvert.SerializeObject(list, Formatting.Indented);
        }

        public void Deserialize(string json)
        {
            var info = JsonConvert.DeserializeObject<InventoryInfo>(json);
            _productTable.Clear();
            foreach (var productEntryInfo in info.Products)
            {
                try
                {
                    var product = _productLookup.GetProduct(productEntryInfo.ProductName);
                    var entry = new InventoryProductEntry
                    {
                        Amount = productEntryInfo.Amount,
                        Product = product
                    };
                    _productTable.Add(product.ID, entry);
                }
                catch (Exception)
                {
                    throw new UnityException("Inventory deserialize: Tried to add same product twice.");
                }
            }
        }

        private string LoadDataFromFile(string fileName)
        {
            var text = File.ReadAllText(string.Format("Assets/Resources/{0}.json", fileName));
            return text;
        }

    }
}
