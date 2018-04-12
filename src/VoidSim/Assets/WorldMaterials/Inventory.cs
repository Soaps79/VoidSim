using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Serialization;
using Assets.WorldMaterials.Products;
using QGame;

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
        public class InventoryPlaceableEntry
        {
            public string Name;
            public int Id;
        }

        public ProductInventory Products { get; private set; }

        public Action OnInventoryChanged;
        public Action<string, bool> OnPlaceablesChanged;

        public IProductLookup ProductLookup { get; private set; }
        public string Name;
        
        public List<InventoryPlaceableEntry> Placeables = new List<InventoryPlaceableEntry>();
        private int _lastPlaceableId;

        public void Initialize(InventoryScriptable inventoryScriptable, IProductLookup productLookup, bool addAllEntries = false)
        {
            InitializeProductInventory();
            Products.DefaultProductCapacity = inventoryScriptable.ProductMaxAmount;
            // if addAllEntries, create entries for all known products
            Products.Initialize(inventoryScriptable, productLookup, addAllEntries);
            foreach (var placeable in inventoryScriptable.Placeables)
            {
                AddPlaceable(placeable);
            }
        }

        private void InitializeProductInventory()
        {
            Products = new ProductInventory();
            Products.OnProductsChanged += CheckInventoryChanged;
        }

        private void CheckInventoryChanged(int arg1, int arg2)
        {
            if (OnInventoryChanged != null)
                OnInventoryChanged();
        }

        public void Initialize(InventoryData data, IProductLookup productLookup, bool addAllEntries)
        {
            Products.Initialize(data, productLookup, addAllEntries);
            data.Placeables.ForEach(i => AddPlaceable(i.PlaceableName));
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
		    var products = Products.GetProductEntries().Select(i => new InventoryProductEntryData
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
			    Placeables = placeables,
                DefaultProductCapacity = Products.DefaultProductCapacity
		    };

		    return data;
		}
    }
}