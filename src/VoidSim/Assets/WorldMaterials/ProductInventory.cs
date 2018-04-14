using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Serialization;
using Assets.WorldMaterials.Products;

namespace Assets.WorldMaterials
{
    [Serializable]
    public class ProductInventoryData
    {
        public List<ProductInventoryEntryData> Products;
        public int DefaultProductCapacity;
    }

    [Serializable]
    public class ProductInventoryEntryData
    {
        public string ProductName;
        public int Amount;
        public int MaxAmount;
    }

    /// <summary>
    /// Container to manage Product stores for any game entity
    /// </summary>
    public class ProductInventory : ISerializeData<ProductInventoryData>
    {
        [Serializable]
        public class InventoryProductEntry
        {
            public Product Product;
            public int Amount;
            public int MaxAmount;
        }

        private readonly Dictionary<int, InventoryProductEntry> _productTable
            = new Dictionary<int, InventoryProductEntry>();

        public Action<int, int> OnProductsChanged;
        public Action<int, int> OnProductMaxAmountChanged;
        private IProductLookup _productLookup;

        public int DefaultProductCapacity { get; set; }

        // only use for UI and serialization
        public List<InventoryProductEntry> GetProductEntries()
        {
            return _productTable.Values.ToList();
        }

        /// <summary>
        /// TryAdd ProductAmount to inventory, returns amount that could not be added because max
        /// </summary>
        public int TryAddProduct(int productId, int amount)
        {
            // add a product entry if there is none, if it already exists, see if it has any room
            if (!TryAddProductEntry(productId)
                && _productTable[productId].Amount >= _productTable[productId].MaxAmount)
            {
                // return whole amount if we're full
                return amount;
            }

            var amountConsumed = amount;

            // if there is not room for amount, only consume what we can
            if (_productTable[productId].Amount + amount > _productTable[productId].MaxAmount)
                amountConsumed = _productTable[productId].MaxAmount - _productTable[productId].Amount;

            // add to inventory
            _productTable[productId].Amount += amountConsumed;

            if (OnProductsChanged != null)
                OnProductsChanged(productId, amountConsumed);

            // return any remainder
            return amount - amountConsumed;
        }

        private bool TryAddProductEntry(int productId)
        {
            if (!_productTable.ContainsKey(productId))
            {
                _productTable.Add(productId, new InventoryProductEntry()
                {
                    Product = _productLookup.GetProduct(productId),
                    Amount = 0,
                    MaxAmount = DefaultProductCapacity
                });
                return true;
            }
            return false;
        }

        private void AddProductEntry(Product product)
        {
            if (!_productTable.ContainsKey(product.ID))
            {
                _productTable.Add(product.ID, new InventoryProductEntry()
                {
                    Product = product,
                    Amount = 0,
                    MaxAmount = DefaultProductCapacity
                });
            }
        }

        /// <summary>
        /// If inventory has product, depletes the inventory by amount or however much it can.
        /// Returns amount depleted.
        /// </summary>
        public int TryRemoveProduct(int productId, int amount)
        {
            // also returns 0 when the product is unknown
            if (GetProductCurrentAmount(productId) == 0)
                return 0;

            // either deplete by whole amount, or as much as the inventory has
            var product = _productTable[productId];
            var amountConsumed = 0;
            if (amount > product.Amount)
            {
                amountConsumed = product.Amount;
                product.Amount = 0;
            }
            else
            {
                amountConsumed = amount;
                product.Amount -= amount;
            }

            // tell the world and return the difference
            if (OnProductsChanged != null)
                OnProductsChanged(product.Product.ID, -amountConsumed);

            return amountConsumed;
        }

        // Should GetProductCurrentAmount render this useless?
        public bool HasProduct(int productId, int amount)
        {
            return _productTable.ContainsKey(productId) && _productTable[productId].Amount >= amount;
        }

        public int GetProductMaxAmount(int id)
        {
            return _productTable.ContainsKey(id) ? _productTable[id].MaxAmount : 0;
        }

        // TODO: Enable unlimited amounts
        public void SetProductMaxAmount(int id, int max)
        {
            if (!_productTable.ContainsKey(id))
                TryAddProductEntry(id);

            _productTable[id].MaxAmount = max;
            if (OnProductMaxAmountChanged != null)
                OnProductMaxAmountChanged(id, max);
        }

        public int GetProductCurrentAmount(int id)
        {
            return _productTable.ContainsKey(id) ? _productTable[id].Amount : 0;
        }

        public int GetProductRemainingSpace(int productId)
        {
            if (!_productTable.ContainsKey(productId))
                return 0;

            // return the remaining space, if there is any
            return _productTable[productId].MaxAmount > _productTable[productId].Amount ?
                _productTable[productId].MaxAmount - _productTable[productId].Amount : 0;
        }

        public void Initialize(InventoryScriptable inventoryScriptable, IProductLookup productLookup, bool addAllEntries = false)
        {
            DefaultProductCapacity = inventoryScriptable.ProductMaxAmount;
            _productLookup = productLookup;
            // if addAllEntries, create entries for all known products
            Initialize(productLookup, addAllEntries);
            LoadFromScriptable(inventoryScriptable);
        }

        public void Initialize(ProductInventoryData data, IProductLookup productLookup, bool addAllEntries)
        {
            Initialize(productLookup, addAllEntries);
            LoadFromData(data);
        }

        private void Initialize(IProductLookup lookup, bool addAllEntries)
        {
            _productTable.Clear();
            _productLookup = lookup;

            if (addAllEntries)
            {
                foreach (var product in _productLookup.GetProducts())
                {
                    AddProductEntry(product);
                }
            }
        }

        private void LoadFromData(ProductInventoryData data)
        {
            DefaultProductCapacity = data.DefaultProductCapacity;
            foreach (var entry in data.Products)
            {
                var product = _productLookup.GetProduct(entry.ProductName);
                if (!_productTable.ContainsKey(product.ID))
                    _productTable.Add(product.ID, new InventoryProductEntry { Product = product });

                _productTable[product.ID].Amount = entry.Amount;
                _productTable[product.ID].MaxAmount = entry.MaxAmount;
            }
        }

        private void LoadFromScriptable(InventoryScriptable inventoryScriptable)
        {
            // populate amounts for any from scriptable
            foreach (var info in inventoryScriptable.Products)
            {
                TryAddProduct(_productLookup.GetProduct(info.ProductName).ID, info.Amount);
            }
        }

        public ProductInventoryData GetData()
        {
            return new ProductInventoryData()
            {
                DefaultProductCapacity = DefaultProductCapacity,
                Products = GetProductEntries().Select(i => new ProductInventoryEntryData
                {
                    ProductName = i.Product.Name,
                    Amount = i.Amount,
                    MaxAmount = i.MaxAmount
                }).ToList()
            };
        }
    }
}