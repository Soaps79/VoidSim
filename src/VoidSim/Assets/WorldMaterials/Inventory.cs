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
        
        private readonly Dictionary<int, InventoryProductEntry> _productTable 
            = new Dictionary<int, InventoryProductEntry>();

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

        private string LoadDataFromFile(string fileName)
        {
            var text = File.ReadAllText(string.Format("Assets/Resources/{0}.json", fileName));
            return text;
        }

    }
}
