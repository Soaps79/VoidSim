using UnityEngine;
using UIWidgets;

namespace UIWidgetsSamples
{
	/// <summary>
	/// TreeViewCheckboxes item.
	/// </summary>
	[System.Serializable]
	public class TreeViewCheckboxesItem : TreeViewItem
	{
		/// <summary>
		/// Is selected.
		/// </summary>
		[SerializeField]
		public bool Selected;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="itemName">Name.</param>
		/// <param name="itemIcon">Color.</param>
		/// <param name="itemSelected">Is selected.</param>
		public TreeViewCheckboxesItem(string itemName, Sprite itemIcon = null, bool itemSelected = false)
			: base(itemName, itemIcon)
		{
			Selected = itemSelected;
		}
	}
}