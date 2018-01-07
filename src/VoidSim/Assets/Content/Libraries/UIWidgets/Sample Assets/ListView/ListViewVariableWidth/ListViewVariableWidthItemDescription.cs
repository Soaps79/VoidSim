using UnityEngine;
using UIWidgets;

namespace UIWidgetsSamples
{
	/// <summary>
	/// ListViewVariableWidth item description.
	/// </summary>
	[System.Serializable]
	public class ListViewVariableWidthItemDescription : IItemWidth
	{
		/// <summary>
		/// Name.
		/// </summary>
		[SerializeField]
		public string Name;

		/// <summary>
		/// Text.
		/// </summary>
		[SerializeField]
		public string Text;

		[SerializeField]
		float width;

		/// <summary>
		/// Width.
		/// </summary>
		public float Width {
			get {
				return width;
			}
			set {
				width = value;
			}
		}
	}
}