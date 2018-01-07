using UnityEngine;
using UnityEngine.UI;
using UIWidgets;

namespace UIWidgetsSamples
{
	/// <summary>
	/// Determinate ProgressBar sample.
	/// </summary>
	[RequireComponent(typeof(Button))]
	public class SampleProgressBar2 : MonoBehaviour
	{
		/// <summary>
		/// Progressbar.
		/// </summary>
		[SerializeField]
		public Progressbar bar;

		Button button;
		
		/// <summary>
		/// Add listeners.
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
		/// Toggle animation.
		/// </summary>
		protected virtual void Click()
		{
			if (bar.IsAnimationRun)
			{
				bar.Stop();
			}
			else
			{
				if (bar.Value==0)
				{
					bar.Animate(bar.Max);
				}
				else
				{
					bar.Animate(0);
				}
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