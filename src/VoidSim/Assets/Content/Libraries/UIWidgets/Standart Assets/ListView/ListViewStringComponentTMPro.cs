using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UIWidgets.TMProSupport {

	/// <summary>
	/// List view item component.
	/// </summary>
	[AddComponentMenu("UI/UIWidgets/ListViewStringComponentTMPro")]
	public class ListViewStringComponentTMPro : ListViewStringComponent
	{
		/// <summary>
		/// Foreground graphics for coloring.
		/// </summary>
		public override Graphic[] GraphicsForeground {
			get {
				return new Graphic[] {TextTMPro, };
			}
		}

		/// <summary>
		/// The Text component.
		/// </summary>
		public TextMeshProUGUI TextTMPro;

		/// <summary>
		/// Sets the data.
		/// </summary>
		/// <param name="item">Text.</param>
		public override void SetData(string item)
		{
			TextTMPro.text = item.Replace("\\n", "\n");
		}
	}
}