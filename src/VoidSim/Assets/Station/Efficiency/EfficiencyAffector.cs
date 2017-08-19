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
		// mostly for debugging
		public string Driver;

		public EfficiencyAffector(string driverName) { Driver = driverName; }

		public EfficiencyAffector(string driverName, float value)
		{
			Driver = driverName;
			_currentEfficiency = value;
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

		private void CheckValueChanged()
		{
			if (OnValueChanged != null)
				OnValueChanged(this);
		}
	}
}