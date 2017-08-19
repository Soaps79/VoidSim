using System;
using Assets.Scripts;
using Assets.Station.Efficiency;
using Assets.WorldMaterials.Products;
using QGame;
using UnityEngine;

namespace Assets.WorldMaterials.Population
{
	public class MoodMonitor
	{
		public readonly EfficiencyModule MoodModule = new EfficiencyModule();
		private readonly EfficiencyAffector _foodAffector = new EfficiencyAffector("Food");
		private readonly EfficiencyAffector _waterAffector = new EfficiencyAffector("Water");
		[SerializeField] private float _foodConsumedPerPop;
		[SerializeField] private float _waterConsumedPerPop;
		[SerializeField] private float _moodMinimumAmount;

		private int _currentCount;

		private Inventory _inventory;

		public MoodMonitor(PopulationSO scriptable, Inventory inventory)
		{
			_inventory = inventory;
			if (scriptable != null && scriptable.MoodParams != null)
				SetFromScriptable(scriptable.MoodParams);

			InitializeNeedsConsumption();
		}

		private void SetFromScriptable(MoodParams param)
		{
			_foodConsumedPerPop = param.FoodPerPop;
			_waterConsumedPerPop = param.WaterPerPop;
			_moodMinimumAmount = param.MoodMinimum;
		}

		private void InitializeNeedsConsumption()
		{
			MoodModule.RegisterAffector(_foodAffector);
			MoodModule.RegisterAffector(_waterAffector);
			MoodModule.MinimumAmount = _moodMinimumAmount;
			KeyValueDisplay.Instance.Add("Pop Mood", () => MoodDisplayString);

			Locator.WorldClock.OnHourUp += HandleHourTick;
		}

		public object MoodDisplayString
		{
			get
			{
				var display = MoodModule.CurrentAmount.ToString("0.00") + "  ";
				display += _foodAffector.Name + " " + _foodAffector.Efficiency.ToString("0.00") + "  ";
				display += _waterAffector.Name + " " + _waterAffector.Efficiency.ToString("0.00");
				return display;
			}
		}

		private void HandleHourTick(object sender, EventArgs e)
		{
			var currentHour = Locator.WorldClock.CurrentTime.Hour;
			if (currentHour == 10 || currentHour == 18)
			{
				_currentCount = _inventory.GetProductCurrentAmount(ProductIdLookup.Population);
				HandleFoodConsumption();
				HandleWaterConsumption();
			}
		}

		private void HandleFoodConsumption()
		{
			var exact = _foodConsumedPerPop * _currentCount;
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
			var exact = _waterConsumedPerPop * _currentCount;
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
			MoodModule.MinimumAmount = value ? 1.0f : _moodMinimumAmount;
		}
	}
}