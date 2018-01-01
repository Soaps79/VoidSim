using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Placeables;
using Assets.Placeables.Nodes;
using Assets.Scripts;
using Assets.Scripts.Serialization;
using Assets.WorldMaterials;
using Assets.WorldMaterials.Products;
using Assets.WorldMaterials.UI;
using Messaging;
using ModestTree;
using QGame;
using UnityEngine;

namespace Assets.Station
{
	public class FactoryControlData
	{
		public List<ProductFactoryData> Factories;
	}

	/// <summary>
	/// The purpose of this object is to act as owner to all of the station's factories.
	/// This means initializing them when they are placed, handling the trade requests for the
	/// ones set to auto-buy, and eventually handling their removal
	/// </summary>
	public class FactoryControl : QScript, IMessageListener, ISerializeData<FactoryControlData>
	{
		private Inventory _inventory;
		private ProductLookup _productLookup;
		public List<ProductFactory> Factories = new List<ProductFactory>();

		public Action OnFactoryListUpdated;
		public Action<Recipe> OnCraftComplete;
		private InventoryReserve _reserve;
		//private int _lastFactoryId;

		// this is a list of all items that the factories currently need
		// the inventory reserve will be set to these values, 
		// and StationTrader will handle placing the actual trade requests
		private readonly Dictionary<int, int> _purchasing = new Dictionary<int, int>();

		private List<ProductFactoryData> _deserialized = new List<ProductFactoryData>();

		private readonly CollectionSerializer<FactoryControlData> _serializer
			= new CollectionSerializer<FactoryControlData>();

		private const string _collectionName = "FactoryControl";

		public void Initialize(Inventory inventory, ProductLookup lookup, InventoryReserve reserve)
		{
			_inventory = inventory;
			_productLookup = lookup;
			_reserve = reserve;
			Locator.MessageHub.AddListener(this, ProductFactory.MessageName);

			var go = (GameObject)Instantiate(Resources.Load("Views/player_crafting_array_viewmodel"));
			go.transform.SetParent(transform);
			go.name = "player_crafting_array_viewmodel";
			var viewModel = go.GetComponent<PlayerCraftingArrayViewModel>();
			viewModel.Bind(this);
			if(_serializer.HasDataFor(this, _collectionName))
				Load();
		}

		private void Load()
		{
			_deserialized = _serializer.DeserializeData().Factories;
		}

		public void HandleMessage(string type, MessageArgs args)
		{
			if (type == ProductFactory.MessageName && args != null)
				HandleFactoryAdd(args as ProductFactoryMessageArgs);
		}

		// initializes the factory and gives it data from deserailization
		private void HandleFactoryAdd(ProductFactoryMessageArgs args)
		{
			if(args.ProductFactory == null)
				throw new UnityException("Factory control recieved bad message data");

			var factory = args.ProductFactory;
			factory.Initialize(_inventory, _productLookup);
			factory.OnIsBuyingchanged += RefreshPurchasing;
			factory.OnCraftComplete += HandleCraftComplete;

			CheckForDeserialized(factory);

			factory.OnRemove += HandleFactoryRemove;
			Factories.Add(factory);
			CheckCallback();
		}

		private void HandleFactoryRemove(ProductFactory obj)
		{
			if (Factories.Remove(obj))
			{
				CheckCallback();
			}
		}

		private void HandleCraftComplete(Recipe recipe)
		{
			if (OnCraftComplete != null)
				OnCraftComplete(recipe);
		}

		private void CheckForDeserialized(ProductFactory factory)
		{
			if (_deserialized.Any())
			{
				var resume = _deserialized.FirstOrDefault(i => i.Name == factory.name);
				if (resume != null)
				{
					factory.Resume(resume);
					_deserialized.Remove(resume);
				}
			}
		}

		// first pass at forecasting, currently buying enough to keep factories going for a week
		private void RefreshPurchasing()
		{
			var oldKeys = _purchasing.Keys.ToList();
			_purchasing.Clear();

			var ingredients = new List<ProductAmount>();
			Factories.Where(i => i.IsBuying && i.IsCrafting).ForEach(i => ingredients.AddRange(i.GetDayForecast()));
			foreach (var ingredient in ingredients)
			{
				if(!_purchasing.ContainsKey(ingredient.ProductId))
					_purchasing.Add(ingredient.ProductId, 0);

				_purchasing[ingredient.ProductId] += ingredient.Amount * 7;
			}

			foreach (var key in oldKeys)
			{
				if(!_purchasing.ContainsKey(key))
					_reserve.SetConsume(key, false);
			}

			foreach (var pair in _purchasing)
			{
				_reserve.SetConsume(pair.Key, true);
				_reserve.SetAmount(pair.Key, pair.Value);
			}
		}

		private void CheckCallback()
		{
			if (OnFactoryListUpdated != null)
				OnFactoryListUpdated();
		}

		public string Name { get { return "FactoryControl"; } }
		public FactoryControlData GetData()
		{
			return new FactoryControlData { Factories = Factories.Select(i => i.GetData()).ToList() };
		}
	}
}