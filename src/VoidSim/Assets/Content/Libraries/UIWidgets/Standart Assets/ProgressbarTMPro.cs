using UnityEngine;
using UIWidgets;
using TMPro;

namespace UIWidgets.TMProSupport {
	/// <summary>
	/// Progressbar with TextMesh Pro support.
	/// </summary>
	public class ProgressbarTMPro : Progressbar {
		/// <summary>
		/// The empty bar text.
		/// </summary>
		[SerializeField]
		public TextMeshProUGUI EmptyBarTextTMPro;

		/// <summary>
		/// The full bar text.
		/// </summary>
		[SerializeField]
		public TextMeshProUGUI FullBarTextTMPro;

		/// <summary>
		/// Updates the text.
		/// </summary>
		protected override void UpdateText()
		{
			var text = TextFunc(this);
			if (FullBarTextTMPro!=null)
			{
				FullBarTextTMPro.text = text;
			}
			if (EmptyBarTextTMPro!=null)
			{
				EmptyBarTextTMPro.text = text;
			}
		}
	}
}