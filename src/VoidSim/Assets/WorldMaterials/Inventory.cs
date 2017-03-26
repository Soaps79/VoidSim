using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.WorldMaterials;
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

        public class Factory : Factory<Inventory> { }

        public Action OnProductsChanged;

        [Inject]
        private ProductLookup _productLookup;
        public string Name;
        
        private readonly Dictionary<int, InventoryProductEntry> _productTable 
            = new Dictionary<int, InventoryProductEntry>();

        private InventoryScriptable _scriptable;

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
            Debug.Log(string.Format("Inventory update: {0} {1}", amount, product.Name));
            return true;
        }

        public bool TryAddProduct(string product, int amount)
        {
            return TryAddProduct(_productLookup.GetProduct(product), amount);
        }

        // early interface pass... update as usage requires
        public bool TryRemoveProduct(string productName, int amount)
        {
            if (!HasProduct(productName, amount))
                return false;

            _productTable.First(i => i.Value.Product.Name == productName).Value.Amount -= amount;
            if (OnProductsChanged != null)
                OnProductsChanged();

            return true;
        }

        public bool HasProduct(int productId, int amount)
        {
            return _productTable.ContainsKey(productId) && _productTable[productId].Amount >= amount;
        }

        public void BindToScriptable(InventoryScriptable inventoryScriptable, ProductLookup productLookup)
        {
            _scriptable = inventoryScriptable;
            _productLookup = productLookup;
            _productTable.Clear();

            foreach (var info in _scriptable.Products)
            {
                TryAddProduct(_productLookup.GetProduct(info.ProductName), info.Amount);
            }
        }

        public bool HasProduct(string productName, int quantity)
        {
            var entry = _productTable.FirstOrDefault(i => i.Value.Product.Name == productName).Value;
            return entry != null && entry.Amount >= quantity;

        }
    }
}
