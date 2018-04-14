using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Narrative.Missions;
using Assets.Scripts;
using Assets.Scripts.Serialization;
using Assets.WorldMaterials.Products;
using Messaging;
using QGame;
using UnityEngine;

namespace Assets.WorldMaterials
{
	[Serializable]
	public class InventoryData
	{
	    public ProductInventoryData Products;
		public List<InventoryPlaceableData> Placeables;
	}

    [Serializable]
	public class InventoryPlaceableData
	{
		public int Id;
		public string PlaceableName;
	}

	public class StationInventory : QScript, ISerializeData<InventoryData>, IMessageListener
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
        private Dictionary<string, Product> _allProducts;


        public List<InventoryPlaceableEntry> Placeables = new List<InventoryPlaceableEntry>();
        private int _lastPlaceableId;

        public void Initialize(InventoryScriptable inventoryScriptable, IProductLookup productLookup, bool addAllEntries = false)
        {
            InitializeCommon();
            Products.DefaultProductCapacity = inventoryScriptable.ProductMaxAmount;
            // if addAllEntries, create entries for all known products
            Products.Initialize(inventoryScriptable, productLookup, addAllEntries);
            foreach (var placeable in inventoryScriptable.Placeables)
            {
                AddPlaceable(placeable);
            }
        }

        private void InitializeCommon()
        {
            Products = new ProductInventory();
            Products.OnProductsChanged += CheckInventoryChanged;
            Locator.MessageHub.AddListener(this, Mission.MessageName);
        }

        private void CheckInventoryChanged(int arg1, int arg2)
        {
            if (OnInventoryChanged != null)
                OnInventoryChanged();
        }

        public void Initialize(InventoryData data, IProductLookup productLookup, bool addAllEntries)
        {
            InitializeCommon();
            Products.Initialize(data.Products, productLookup, addAllEntries);
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

        public void HandleMessage(string type, MessageArgs args)
        {
            if (type == Mission.MessageName && args != null)
                HandleMissionUpdate(args as MissionUpdateMessageArgs);
        }

        private void HandleMissionUpdate(MissionUpdateMessageArgs args)
        {
            if (args == null)
                throw new UnityException("StationInventory given bad mission reward args");

            if (args.Status == MissionUpdateStatus.Complete && args.Mission == null)
            {
                foreach (var missionRewardAmount in args.Mission.Scriptable.Rewards)
                {
                    if (_allProducts.ContainsKey(missionRewardAmount.Name))
                        Products.TryAddProduct(_allProducts[missionRewardAmount.Name].ID, missionRewardAmount.Amount);
                }
            }
        }

        public string Name { get { return "StationInventory"; } }

        public InventoryData GetData()
	    {
			// convert products and placeables to their data types
		    var data = new InventoryData
		    {
			    Products = Products.GetData(),
			    Placeables = Placeables.Select(i => new InventoryPlaceableData
			    {
			        Id = i.Id,
			        PlaceableName = i.Name
			    }).ToList()
            };

		    return data;
		}
    }
}