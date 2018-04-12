using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using Assets.Narrative.Missions;
using Assets.Scripts;
using Assets.WorldMaterials;
using Assets.WorldMaterials.Products;
using Messaging;
using QGame;
using UnityEngine;

namespace Assets.Station
{
	public class StationInventory : QScript, IMessageListener
	{
		private Inventory _inventory;
		private Dictionary<string, Product> _products;

		public void Initialize(Inventory inventory)
		{
			_inventory = inventory;
			Locator.MessageHub.AddListener(this, Mission.MessageName);
			_products = ProductLookup.Instance.GetProducts().ToDictionary(i => i.Name);
		}

		public void HandleMessage(string type, MessageArgs args)
		{
			if (type == Mission.MessageName && args != null)
				HandleMissionUpdate(args as MissionUpdateMessageArgs);
		}

		private void HandleMissionUpdate(MissionUpdateMessageArgs args)
		{
			if( args == null)
				throw new UnityException("StationInventory given bad mission reward args");

		    if (args.Status == MissionUpdateStatus.Complete && args.Mission == null)
		    {
		        foreach (var missionRewardAmount in args.Mission.Scriptable.Rewards)
		        {
		            if (_products.ContainsKey(missionRewardAmount.Name))
		                _inventory.Products.TryAddProduct(_products[missionRewardAmount.Name].ID, missionRewardAmount.Amount);
		        }
		    }
		}

		public string Name { get { return "StationInventory"; } }
	}
}