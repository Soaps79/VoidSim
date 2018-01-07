using UnityEngine;
using UIWidgets;
using UnityEngine.Serialization;

namespace UIWidgetsSamples
{
	/// <summary>
	/// Test Progressbar.
	/// </summary>
	public class TestProgressbar : MonoBehaviour
	{
		/// <summary>
		/// Progressbar.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("bar")]
		protected Progressbar Progressbar;

		/// <summary>
		/// Spinner.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("spinner")]
		protected Spinner Spinner;

		/// <summary>
		/// Toggle progressbar.
		/// </summary>
		public void Toggle()
		{
			if (Progressbar.IsAnimationRun)
			{
				Progressbar.Stop();
			}
			else
			{
				if (Progressbar.Value==0)
				{
					Progressbar.Animate(Progressbar.Max);
				}
				else
				{
					Progressbar.Animate(0);
				}
			}
		}

		/// <summary>
		/// Set progressbar value.
		/// </summary>
		/// <param name="value">Value.</param>
		public void SetValue(int value)
		{
			Progressbar.Animate(value);
		}

		/// <summary>
		/// Set value from spinner.
		/// </summary>
		public void SetFromSpinner()
		{
			SetValue(Spinner.Value);
		}
	}
}