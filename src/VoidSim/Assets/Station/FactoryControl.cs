using System;
using System.Collections.Generic;
using Assets.Logistics;
using Assets.Placeables.Nodes;
using Assets.WorldMaterials;
using Assets.WorldMaterials.Products;
using Assets.WorldMaterials.UI;
using Messaging;
using QGame;
using UnityEngine;

namespace Assets.Station
{
	public class FactoryControl : QScript, IMessageListener
	{
		private Inventory _inventory;
		private ProductLookup _productLookup;
		public List<ProductFactory> Factories = new List<ProductFactory>();

		public Action OnFactoryListUpdated;

		public void Initialize(Inventory inventory, ProductLookup lookup)
		{
			_inventory = inventory;
			_productLookup = lookup;
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
			Factories.Add(args.ProductFactory);
			CheckCallback();
		}

		private void CheckCallback()
		{
			if (OnFactoryListUpdated != null)
				OnFactoryListUpdated();
		}

		public string Name { get { return "FactoryControl"; } }
	}
}