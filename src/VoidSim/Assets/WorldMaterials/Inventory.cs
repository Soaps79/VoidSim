﻿using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.WorldMaterials;
using Assets.Station;
using QGame;
using Zenject;

namespace Assets.WorldMaterials
{
    public class Inventory : QScript
    {
        /// <summary>
        /// The idea here is that Products are one type of object that Inventory maintains.
        /// Another type will probably be some sort of Placeable.
        /// </summary>
        public class InventoryProductEntry
        {
            public Product Product;
            public int Amount;
            //public int MaxAmount;
        }

        public class InventoryPlaceableEntry
        {
            public string Name;
            public int Id;
        }

        public class Factory : Factory<Inventory> { }

        public Action OnInventoryChanged;
        public Action<string, int> OnProductsChanged;
        public Action<string, bool> OnPlaceablesChanged;

        [Inject] private ProductLookup _productLookup;
        public string Name;
        
        private readonly Dictionary<int, InventoryProductEntry> _productTable 
            = new Dictionary<int, InventoryProductEntry>();

        public List<InventoryPlaceableEntry> Placeables = new List<InventoryPlaceableEntry>();
        private InventoryScriptable _scriptable;
        private int _lastPlaceableId;

        // only use for UI
        public List<InventoryProductEntry> GetProductEntries()
        {
            return _productTable.Values.ToList();
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
                    Amount = 0
                });
            }

            // impl MaxAmount checks
            _productTable[product.ID].Amount += amount;

            if (OnProductsChanged != null)
                OnProductsChanged(product.Name, amount);

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
                OnProductsChanged(productName, -amount);

            if (OnInventoryChanged != null)
                OnInventoryChanged();

            return true;
        }

        public bool HasProduct(int productId, int amount)
        {
            return _productTable.ContainsKey(productId) && _productTable[productId].Amount >= amount;
        }

        public int GetProductCurrentAmount(string productName)
        {
            var entry = _productTable.FirstOrDefault(i => i.Value.Product.Name == productName).Value;
            return entry != null ? entry.Amount : 0;
        }

        public int GetProductCurrentAmount(int id)
        {
            return _productTable.ContainsKey(id) ? _productTable[id].Amount : 0;
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

            foreach (var placeable in _scriptable.Placeables)
            {
                _lastPlaceableId++;
                Placeables.Add(new InventoryPlaceableEntry { Name = placeable, Id = _lastPlaceableId });
            }
        }

        public bool HasProduct(string productName, int quantity)
        {
            var entry = _productTable.FirstOrDefault(i => i.Value.Product.Name == productName).Value;
            return entry != null && entry.Amount >= quantity;

        }

        public bool TryRemovePlaceable(int id)
        {
            var placeable = Placeables.FirstOrDefault(i => i.Id == id);
            if (placeable == null)
                return false;

            Placeables.Remove(placeable);
            if (OnPlaceablesChanged != null)
                OnPlaceablesChanged(placeable.Name, false);

            if (OnInventoryChanged != null)
                OnInventoryChanged();

            return true;
        }
    }
}