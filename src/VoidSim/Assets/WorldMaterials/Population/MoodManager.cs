using System;
using Assets.Scripts;
using Assets.Station.Efficiency;
using Assets.WorldMaterials.Products;

namespace Assets.WorldMaterials.Population
{
	public class MoodManager
	{
		public readonly EfficiencyModule EfficiencyModule = new EfficiencyModule();
		private readonly EfficiencyAffector _foodAffector = new EfficiencyAffector("Food");
		private readonly EfficiencyAffector _waterAffector = new EfficiencyAffector("Water");
		private float _foodConsumedPerPop;
		private float _waterConsumedPerPop;
		private float _moodMinimumAmount;
		private int _currentLeisure;

		private int _currentPopCount;

		private Inventory _inventory;
		private float _maxLeisureBonus;
		private int _baseLeisure;
		private LeisureTracker _leisureTracker;

		public MoodManager(MoodParams moodParams, Inventory inventory)
		{
			_inventory = inventory;
			_inventory.Products.OnProductsChanged += HandleInventoryUpdate;

			if (moodParams != null)
				SetFromScriptable(moodParams);
			else
				UberDebug.LogChannel(LogChannels.Warning, "MoodManager given bad params");

			InitializeNeedsConsumption();
			UpdatePopCount();
			_leisureTracker = new LeisureTracker(moodParams, _currentPopCount);
			EfficiencyModule.RegisterAffector(_leisureTracker.Affector);
		}

		private void HandleInventoryUpdate(int productId, int amount)
		{
			if (productId != ProductIdLookup.Population)
				return;

			UpdatePopCount();
			_leisureTracker.UpdatePopCount(_currentPopCount);
		}

		private void SetFromScriptable(MoodParams param)
		{
			_foodConsumedPerPop = param.FoodPerPop;
			_waterConsumedPerPop = param.WaterPerPop;
			_moodMinimumAmount = param.MoodMinimum;
			_currentLeisure = param.BaseLeisure;
			_maxLeisureBonus = param.MaxLeisureBonus;
			_baseLeisure = param.BaseLeisure;
		}

		private void InitializeNeedsConsumption()
		{
			EfficiencyModule.RegisterAffector(_foodAffector);
			EfficiencyModule.RegisterAffector(_waterAffector);
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
			_currentPopCount = _inventory.Products.GetProductCurrentAmount(ProductIdLookup.Population);

		}

		private void HandleFoodConsumption()
		{
			var exact = _foodConsumedPerPop * _currentPopCount;
			var need = (int)Math.Ceiling(exact);
			var consumed = _inventory.Products.TryRemoveProduct(ProductIdLookup.Food, need);
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
			var consumed = _inventory.Products.TryRemoveProduct(ProductIdLookup.Water, need);
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
	}
}