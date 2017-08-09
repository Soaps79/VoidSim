using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Station.Efficiency
{
	/// <summary>
	/// Owned by a consumer, ie: a goods producer, who has another object 
	/// effecting their efficiency, ie: energy provider or working employees
	/// 
	/// Consumers should hook into OnValueChanged
	/// </summary>
	[Serializable]
	public class EfficiencyModule
	{
		private readonly List<EfficiencyAffector> _affectors = new List<EfficiencyAffector>();
		public float CurrentAmount = 1.0f;

		public Action<EfficiencyModule> OnValueChanged;

		public void RegisterAffector(EfficiencyAffector affector)
		{
			_affectors.Add(affector);
			affector.OnValueChanged += OnAffectorChanged;
			UpdateValue();
		}

		private void OnAffectorChanged(EfficiencyAffector efficiencyAffector)
		{
			UpdateValue();
		}

		private void UpdateValue()
		{
			var overall = 0.0f;
			var weightSum = _affectors.Sum(i => i.Weight);
			var baseValue = weightSum / 1;

			var weightedValueSum = _affectors.Sum(i => i.Efficiency * i.Weight * baseValue);
			
			if (weightSum != 0)
				overall = weightedValueSum / weightSum;
			else
				throw new DivideByZeroException("Your message here");

			if (Math.Abs(CurrentAmount - overall) < .01)
				return;

			CurrentAmount = overall;
			if (OnValueChanged != null)
				OnValueChanged(this);
		}

		public void Clear()
		{
			_affectors.Clear();
			CurrentAmount = 1.0f;
			OnValueChanged = null;
		}
	}
}