using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UIWidgets {
	/// <summary>
	/// ListViewInt component.
	/// </summary>
	public class ListViewIntComponentTMPro : ListViewIntComponentBase {
		/// <summary>
		/// Foreground graphics for coloring.
		/// </summary>
		public override Graphic[] GraphicsForeground {
			get {
				return new Graphic[] {Number, };
			}
		}

		/// <summary>
		/// The number.
		/// </summary>
		[SerializeField]
		public TextMeshProUGUI Number;

		/// <summary>
		/// Sets the data.
		/// </summary>
		/// <param name="item">Item.</param>
		public override void SetData(int item)
		{
			Number.text = item.ToString();
		}
	}
}