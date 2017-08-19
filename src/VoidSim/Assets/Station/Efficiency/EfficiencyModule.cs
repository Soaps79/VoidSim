using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;

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

		private float _minimumAmount;
		public float MinimumAmount
		{
			get { return _minimumAmount; }
			set
			{
				if(Math.Abs(value - _minimumAmount) < .01f)
					return;

				_minimumAmount = value;
				UpdateValue();
			}
		}

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
			float amount;

			// if any affector is less than 1, the efficiency is pulled down to the worst
			var lowest = _affectors.Min(i => i.Efficiency);
			if (lowest < 1.0)
				amount = lowest;
			// otherwise, bonuses (efficiency greater than 1) are added up and applied
			else
			{
				var bonus = 0.0f;
				_affectors.ForEach(i => bonus += i.Efficiency - 1.0f);
				amount = 1.0f + bonus;
			}

			if (Math.Abs(CurrentAmount - amount) < .01)
				return;

			CurrentAmount = amount >= MinimumAmount ? amount : MinimumAmount;
			if (OnValueChanged != null)
				OnValueChanged(this);
		}

		// keeping this because I don't want to lose this equation
		//private void UpdateValue()
		//{
		//	var overall = 0.0f;
		//	var weightSum = _affectors.Sum(i => i.Weight);
		//	var baseValue = weightSum / 1;

		//	var weightedValueSum = _affectors.Sum(i => i.Efficiency * i.Weight * baseValue);

		//	if (weightSum != 0)
		//		overall = weightedValueSum / weightSum;
		//	else
		//		throw new DivideByZeroException("Your message here");

		//	if (Math.Abs(CurrentAmount - overall) < .01)
		//		return;

		//	CurrentAmount = overall;
		//	if (OnValueChanged != null)
		//		OnValueChanged(this);
		//}

		public void Clear()
		{
			_affectors.Clear();
			CurrentAmount = 1.0f;
			OnValueChanged = null;
		}
	}
}