using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Serialization;
using Assets.WorldMaterials.Products;
using QGame;
using Zenject;

namespace Assets.WorldMaterials
{
	[Serializable]
	public class InventoryData
	{
		public List<InventoryProductEntryData> Products;
		public List<InventoryPlaceableData> Placeables;
		public int DefaultProductCapacity;
	}

	[Serializable]
	public class InventoryProductEntryData
	{
		public string ProductName;
		public int Amount;
		public int MaxAmount;
	}

	[Serializable]
	public class InventoryPlaceableData
	{
		public int Id;
		public string PlaceableName;
	}

	public class Inventory : QScript, ISerializeData<InventoryData>
    {
		[Serializable]
		public class InventoryProductEntry
        {
            public Product Product;
            public int Amount;
            public int MaxAmount;
        }

		[Serializable]
        public class InventoryPlaceableEntry
        {
            public string Name;
            public int Id;
        }

        public class Factory : Factory<Inventory> { }

        public Action OnInventoryChanged;
        public Action<int, int> OnProductsChanged;
        public Action<int, int> OnProductMaxAmountChanged;
        public Action<string, bool> OnPlaceablesChanged;

        public IProductLookup ProductLookup { get; private set; }
        public string Name;
        
        private readonly Dictionary<int, InventoryProductEntry> _productTable 
            = new Dictionary<int, InventoryProductEntry>();

        public List<InventoryPlaceableEntry> Placeables = new List<InventoryPlaceableEntry>();
        private int _lastPlaceableId;
        public int DefaultProductCapacity { get; private set; }

        // only use for UI
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

            if (OnInventoryChanged != null)
                OnInventoryChanged();

            // return any remainder
            return amount - amountConsumed;
        }

        private bool TryAddProductEntry(int productId)
        {
            if (!_productTable.ContainsKey(productId))
            {
                _productTable.Add(productId, new InventoryProductEntry()
                {
                    Product = ProductLookup.GetProduct(productId),
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
            if (GetProductCurrentAmount(productId) == 0 )
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

            if (OnInventoryChanged != null)
                OnInventoryChanged();

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
			return _productTable.ContainsKey(productId) ? _productTable[productId].MaxAmount - _productTable[productId].Amount : 0;
		}

        public void Initialize(InventoryScriptable inventoryScriptable, IProductLookup productLookup, bool addAllEntries = false)
        {
	        DefaultProductCapacity = inventoryScriptable.ProductMaxAmount;
			// if addAllEntries, create entries for all known products
			Initialize(productLookup, addAllEntries);
			BindToSerializable(inventoryScriptable);
        }

		public void Initialize(InventoryData data, IProductLookup productLookup, bool addAllEntries)
		{
			Initialize(productLookup, addAllEntries);
			LoadFromData(data);
		}

	    private void Initialize(IProductLookup lookup, bool addAllEntries)
	    {
		    _productTable.Clear();
			ProductLookup = lookup;

			if (addAllEntries)
		    {
			    foreach (var product in ProductLookup.GetProducts())
			    {
				    AddProductEntry(product);
			    }
		    }
	    }

	    private void LoadFromData(InventoryData data)
	    {
		    DefaultProductCapacity = data.DefaultProductCapacity;
			foreach (var entry in data.Products)
		    {
			    var product = ProductLookup.GetProduct(entry.ProductName);
				if(!_productTable.ContainsKey(product.ID))
					_productTable.Add(product.ID, new InventoryProductEntry { Product = product });

			    _productTable[product.ID].Amount = entry.Amount;
			    _productTable[product.ID].MaxAmount = entry.MaxAmount;
		    }

			data.Placeables.ForEach(i => AddPlaceable(i.PlaceableName));
	    }

		private void BindToSerializable(InventoryScriptable inventoryScriptable)
	    {
		    // populate amounts for any from scriptable
			foreach (var info in inventoryScriptable.Products)
		    {
			    TryAddProduct(ProductLookup.GetProduct(info.ProductName).ID, info.Amount);
		    }

		    foreach (var placeable in inventoryScriptable.Placeables)
		    {
			    AddPlaceable(placeable);
		    }
	    }

	    private void AddPlaceable(string placeableName)
	    {
			_lastPlaceableId++;
		    Placeables.Add(new InventoryPlaceableEntry { Name = placeableName, Id = _lastPlaceableId });
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

	    public InventoryData GetData()
	    {
			// convert products and placeables to their data types
		    var products = GetProductEntries().Select(i => new InventoryProductEntryData
		    {
			    ProductName = i.Product.Name,
			    Amount = i.Amount,
			    MaxAmount = i.MaxAmount
		    }).ToList();

		    var placeables = Placeables.Select(i => new InventoryPlaceableData
		    {
			    Id = i.Id,
			    PlaceableName = i.Name
		    }).ToList();

		    var data = new InventoryData
		    {
			    Products = products,
			    Placeables = placeables
		    };

		    return data;
		}
    }
}