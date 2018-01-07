using UnityEngine;
using UnityEngine.UI;
using UIWidgets;
using TMPro;

namespace UIWidgets.TMProSupport {
	/// <summary>
	/// TabIconButtonTMPro.
	/// </summary>
	public class TabIconButtonTMPro : TabIconButtonBase {
		/// <summary>
		/// The name.
		/// </summary>
		[SerializeField]
		public TextMeshProUGUI Name;

		/// <summary>
		/// The icon.
		/// </summary>
		[SerializeField]
		public Image Icon;

		/// <summary>
		/// The size of the set native.
		/// </summary>
		[SerializeField]
		public bool SetNativeSize;

		/// <summary>
		/// Sets the data.
		/// </summary>
		/// <param name="tab">Tab.</param>
		public override void SetData(TabIcons tab)
		{
			Name.text = tab.Name;
		}
	}
}