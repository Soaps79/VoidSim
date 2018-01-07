using UnityEngine;
using UnityEngine.UI;
using UIWidgets;

namespace UIWidgetsSamples
{
	/// <summary>
	/// Indetermitate ProgressBar sample.
	/// </summary>
	[RequireComponent(typeof(Button))]
	public class SampleProgressBar1 : MonoBehaviour
	{
		/// <summary>
		/// Progressbar.
		/// </summary>
		[SerializeField]
		public Progressbar bar;

		Button button;

		/// <summary>
		/// Add listener.
		/// </summary>
		protected virtual void Start()
		{
			button = GetComponent<Button>();
			if (button!=null)
			{
				button.onClick.AddListener(Click);
			}
		}

		/// <summary>
		/// Toggle progressbar animation.
		/// </summary>
		protected virtual void Click()
		{
			if (bar.IsAnimationRun)
			{
				bar.Stop();
			}
			else
			{
				bar.Animate();
			}
		}

		/// <summary>
		/// Remove listener.
		/// </summary>
		protected virtual void OnDestroy()
		{
			if (button!=null)
			{
				button.onClick.RemoveListener(Click);
			}
		}
	}
}