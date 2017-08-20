using System;
using Assets.Placeables.Nodes;
using Assets.Scripts;
using Assets.Station.Efficiency;
using Assets.WorldMaterials.Products;
using Messaging;
using QGame;

namespace Assets.WorldMaterials.Population
{
	public class MoodManager : IMessageListener
	{
		public readonly EfficiencyModule EfficiencyModule = new EfficiencyModule();
		private readonly EfficiencyAffector _foodAffector = new EfficiencyAffector("Food");
		private readonly EfficiencyAffector _waterAffector = new EfficiencyAffector("Water");
		private readonly EfficiencyAffector _leisureAffector = new EfficiencyAffector("Leisure");
		private float _foodConsumedPerPop;
		private float _waterConsumedPerPop;
		private float _moodMinimumAmount;
		private int _currentLeisure;

		private int _currentPopCount;

		private Inventory _inventory;

		public MoodManager(MoodParams moodParams, Inventory inventory)
		{
			_inventory = inventory;
			_inventory.OnProductsChanged += HandleInventoryUpdate;

			if (moodParams != null)
				SetFromScriptable(moodParams);
			else
				UberDebug.LogChannel(LogChannels.Warning, "MoodManager given bad params");

			InitializeNeedsConsumption();
			Locator.MessageHub.AddListener(this, LeisureProvider.MessageName);
			UpdateLeisureAffector();
		}

		private void HandleInventoryUpdate(int productId, int amount)
		{
			if(productId == ProductIdLookup.Population)
				UpdateLeisureAffector();
		}

		private void SetFromScriptable(MoodParams param)
		{
			_foodConsumedPerPop = param.FoodPerPop;
			_waterConsumedPerPop = param.WaterPerPop;
			_moodMinimumAmount = param.MoodMinimum;
			_currentLeisure = param.BaseLeisure;
		}

		private void InitializeNeedsConsumption()
		{
			EfficiencyModule.RegisterAffector(_foodAffector);
			EfficiencyModule.RegisterAffector(_waterAffector);
			EfficiencyModule.RegisterAffector(_leisureAffector);
			EfficiencyModule.MinimumAmount = _moodMinimumAmount;

			Locator.WorldClock.OnHourUp += HandleHourTick;
		}

		private void HandleHourTick(object sender, EventArgs e)
		{
			var currentHour = Locator.WorldClock.CurrentTime.Hour;
			if (currentHour == 10 || currentHour == 18)
			{
				UpdatePopCount();
				HandleFoodConsumption();
				HandleWaterConsumption();
			}
		}

		private void UpdatePopCount()
		{
			_currentPopCount = _inventory.GetProductCurrentAmount(ProductIdLookup.Population);
		}

		private void HandleFoodConsumption()
		{
			var exact = _foodConsumedPerPop * _currentPopCount;
			var need = (int)Math.Ceiling(exact);
			var consumed = _inventory.TryRemoveProduct(ProductIdLookup.Food, need);
			if (consumed < need)
			{
				_foodAffector.Efficiency = consumed == 0 ? 0 : (float)consumed / need;
			}
			else
			{
				_foodAffector.Efficiency = 1.0f;
			}
		}

		private void HandleWaterConsumption()
		{
			var exact = _waterConsumedPerPop * _currentPopCount;
			var need = (int)Math.Ceiling(exact);
			var consumed = _inventory.TryRemoveProduct(ProductIdLookup.Water, need);
			if (consumed < need)
			{
				_waterAffector.Efficiency = consumed == 0 ? 0 : (float)consumed / need;
			}
			else
			{
				_waterAffector.Efficiency = 1.0f;
			}
		}

		public void SetIgnoreNeeds(bool value)
		{
			EfficiencyModule.MinimumAmount = value ? 1.0f : _moodMinimumAmount;
		}

		public void HandleMessage(string type, MessageArgs args)
		{
			if (type == LeisureProvider.MessageName && args != null)
				HandleLeisureNode(args as LeisureProviderMessageArgs);
		}

		private void HandleLeisureNode(LeisureProviderMessageArgs args)
		{
			if (args == null)
				return;

			_currentLeisure += args.LeisureProvider.AmountProvided;
			UpdateLeisureAffector();
		}

		private void UpdateLeisureAffector()
		{
			UpdatePopCount();
			if (_currentPopCount == 0) return;
			var fulfilled = (float)_currentLeisure / _currentPopCount;
			_leisureAffector.Efficiency = fulfilled;
		}

		public string Name { get { return "MoodManager"; } }
	}
}