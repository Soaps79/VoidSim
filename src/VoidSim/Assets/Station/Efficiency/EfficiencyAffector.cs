using System;

namespace Assets.Station.Efficiency
{
	/// <summary>
	/// Used by an efficiency provider, such as a power plant, to deliver a value to an EfficencyModule
	/// </summary>
	public class EfficiencyAffector
	{
		private float _currentEfficiency = 1.0f;

		public Action<EfficiencyAffector> OnValueChanged;
		private float _weight = 1.0f;

		public EfficiencyAffector() { }

		public EfficiencyAffector(float value, float weight = 1.0f)
		{
			_currentEfficiency = value;
			_weight = weight;
		}

		public float Efficiency
		{
			get { return _currentEfficiency; }
			set
			{
				if (Math.Abs(value - _currentEfficiency) < .01f)
					return;

				_currentEfficiency = value;
				CheckValueChanged();
			}
		}

		public float Weight
		{
			get { return _weight; }
			set
			{
				if (Math.Abs(value - _weight) < .01f)
					return;

				_weight = value;
				CheckValueChanged();
			}
		}

		private void CheckValueChanged()
		{
			if (OnValueChanged != null)
				OnValueChanged(this);
		}
	}
}