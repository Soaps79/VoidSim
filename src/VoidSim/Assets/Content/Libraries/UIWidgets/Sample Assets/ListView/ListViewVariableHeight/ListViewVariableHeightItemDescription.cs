using UnityEngine;
using UIWidgets;

namespace UIWidgetsSamples
{
	/// <summary>
	/// ListViewVariableHeight item description.
	/// </summary>
	[System.Serializable]
	public class ListViewVariableHeightItemDescription : IItemHeight
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
		float height;

		/// <summary>
		/// Item height.
		/// </summary>
		public float Height {
			get {
				return height;
			}
			set {
				height = value;
			}
		}
	}
}