using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Placeables.Nodes;
using Assets.WorldMaterials;
using Assets.WorldMaterials.Products;
using Assets.WorldMaterials.UI;
using Messaging;
using ModestTree;
using QGame;
using UnityEngine;

namespace Assets.Station
{
	/// <summary>
	/// The purpose of this object is to act as owner to all of the station's factories.
	/// This means initializing them when they are placed, handling the trade requests for the
	/// ones set to auto-buy, and eventually handling their removal
	/// </summary>
	public class FactoryControl : QScript, IMessageListener
	{
		private Inventory _inventory;
		private ProductLookup _productLookup;
		public List<ProductFactory> Factories = new List<ProductFactory>();

		public Action OnFactoryListUpdated;
		private InventoryReserve _reserve;

		// this is a list of all items that the factories currently need
		// the inventory reserve will be set to these values, 
		// and StationTrader will handle placing the actual trade requests
		private readonly Dictionary<int, int> _purchasing = new Dictionary<int, int>();

		public void Initialize(Inventory inventory, ProductLookup lookup, InventoryReserve reserve)
		{
			_inventory = inventory;
			_productLookup = lookup;
			_reserve = reserve;
			MessageHub.Instance.AddListener(this, ProductFactory.MessageName);
			
			var go = (GameObject)Instantiate(Resources.Load("Views/player_crafting_array_viewmodel"));
			go.transform.SetParent(transform);
			go.name = "player_crafting_array_viewmodel";
			var viewModel = go.GetComponent<PlayerCraftingArrayViewModel>();
			viewModel.Bind(this);
		}

		public void HandleMessage(string type, MessageArgs args)
		{
			if (type == ProductFactory.MessageName && args != null)
				HandleFactoryAdd(args as ProductFactoryMessageArgs);
		}

		private void HandleFactoryAdd(ProductFactoryMessageArgs args)
		{
			if(args.ProductFactory == null)
				throw new UnityException("Factory control recieved bad message data");

			args.ProductFactory.Initialize(_inventory, _productLookup);
			args.ProductFactory.OnIsBuyingchanged += RefreshPurchasing;
			Factories.Add(args.ProductFactory);
			CheckCallback();
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
	}
}