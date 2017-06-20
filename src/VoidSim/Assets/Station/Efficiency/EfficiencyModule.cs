using System;
using System.Collections.Generic;

namespace Assets.Station.Efficiency
{
	/// <summary>
	/// Owned by a consumer, ie: a goods producer, who has another object 
	/// effecting their efficiency, ie: energy provider or working employees
	/// 
	/// Consumers should hook into OnValueChanged
	/// </summary>
	public class EfficiencyModule
	{
		private readonly List<EfficiencyAffector> _affectors = new List<EfficiencyAffector>();
		public float CurrentAmount = 1.0f;

		public Action<EfficiencyModule> OnValueChanged;

		public void RegisterAffector(EfficiencyAffector affector)
		{
			_affectors.Add(affector);
			affector.OnValueChanged += OnAffectorChanged;
		}

		private void OnAffectorChanged(EfficiencyAffector efficiencyAffector)
		{
			var overall = 1.0f;
			_affectors.ForEach(i => overall *= i.Value);

			if(Math.Abs(CurrentAmount - overall) < .01)
				return;

			CurrentAmount = overall;
			if (OnValueChanged != null)
				OnValueChanged(this);
		}
	}
}