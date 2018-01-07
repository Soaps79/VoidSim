using UnityEngine;
using UnityEngine.UI;
using UIWidgets;
using TMPro;

namespace UIWidgets.TMProSupport {
	/// <summary>
	/// DialogButtonComponent.
	/// Control how button name will be displayed.
	/// </summary>
	public class DialogButtonComponentTMPro : DialogButtonComponentBase {
		/// <summary>
		/// The name.
		/// </summary>
		[SerializeField]
		public TextMeshProUGUI Name;

		/// <summary>
		/// Sets the button name.
		/// </summary>
		/// <param name="name">Name.</param>
		public override void SetButtonName(string name)
		{
			Name.text = name;
		}
	}
}