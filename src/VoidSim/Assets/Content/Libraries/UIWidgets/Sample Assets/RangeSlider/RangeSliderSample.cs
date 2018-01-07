using UnityEngine;
using UnityEngine.UI;
using UIWidgets;

namespace UIWidgetsSamples
{
	/// <summary>
	/// How to use RangeSlider.
	/// </summary>
	[RequireComponent(typeof(RangeSlider))]
	public class RangeSliderSample : MonoBehaviour
	{
		/// <summary>
		/// Text component to display RangeSlider values.
		/// </summary>
		[SerializeField]
		protected Text Text;

		RangeSlider slider;

		/// <summary>
		/// Add listeners.
		/// </summary>
		protected virtual void Start()
		{
			slider = GetComponent<RangeSlider>();
			if (slider!=null)
			{
				slider.OnValuesChange.AddListener(SliderChanged);
				SliderChanged(slider.ValueMin, slider.ValueMax);
			}
		}

		/// <summary>
		/// Handle changed values.
		/// </summary>
		/// <param name="min">Min value.</param>
		/// <param name="max">Max value.</param>
		protected virtual void SliderChanged(int min, int max)
		{
			if (Text!=null)
			{
				if (slider.WholeNumberOfSteps)
				{
					Text.text = string.Format("Range: {0:000} - {1:000}; Step: {2}", min, max, slider.Step);
				}
				else
				{
					Text.text = string.Format("Range: {0:000} - {1:000}", min, max);
				}
			}
		}

		/// <summary>
		/// Remove listeners.
		/// </summary>
		protected virtual void OnDestroy()
		{
			if (slider!=null)
			{
				slider.OnValuesChange.RemoveListener(SliderChanged);
			}
		}
	}
}