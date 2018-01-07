using UnityEngine;
using UIWidgets;

namespace UIWidgetsSamples
{
	/// <summary>
	/// Progressbar sample.
	/// </summary>
	[RequireComponent(typeof(Progressbar))]
	public class SampleProgressbar : MonoBehaviour
	{
		/// <summary>
		/// Set custom test progress display.
		/// </summary>
		protected virtual void Start()
		{
			var bar = GetComponent<Progressbar>();
			bar.TextFunc = x => string.Format("Exp to next level: {0} / {1}", x.Value, x.Max);
		}

	}
}