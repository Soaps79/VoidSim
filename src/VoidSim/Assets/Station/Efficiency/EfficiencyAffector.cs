using System;

namespace Assets.Station.Efficiency
{
	/// <summary>
	/// Used by an efficiency provider, such as a power plant, to deliver a value to an EfficencyModule
	/// </summary>
	public class EfficiencyAffector
	{
		private float _currentEfficiency;

		public Action<EfficiencyAffector> OnValueChanged;

		public float Value
		{
			get { return _currentEfficiency; }
			set
			{
				if (Math.Abs(value - _currentEfficiency) < .01f)
					return;

				_currentEfficiency = value;
				if (OnValueChanged != null)
					OnValueChanged(this);
			}
		}
	}
}