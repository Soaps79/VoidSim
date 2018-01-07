using UnityEngine;
using UIWidgets;
using TMPro;

namespace UIWidgets.TMProSupport {
	/// <summary>
	/// Tab component.
	/// </summary>
	public class TabButtonComponentTMPro : TabButtonComponentBase {
		/// <summary>
		/// The name.
		/// </summary>
		[SerializeField]
		public TextMeshProUGUI Name;

		/// <summary>
		/// Sets the data.
		/// </summary>
		/// <param name="tab">Tab.</param>
		public override void SetButtonData(Tab tab)
		{
			Name.text = tab.Name;
		}
	}
}