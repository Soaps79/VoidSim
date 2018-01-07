using UnityEngine.UI;
using UnityEngine;

namespace UIWidgets
{
	/// <summary>
	/// DrivesListViewComponent.
	/// Display drive.
	/// </summary>
	public class DrivesListViewComponent : DrivesListViewComponentBase
	{
		/// <summary>
		/// Text component to display drive name.
		/// </summary>
		[SerializeField]
		protected Text Name;

		/// <summary>
		/// Sets component data with specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		public override void SetData(FileSystemEntry item)
		{
			Item = item;

			Name.text = Item.DisplayName;
		}
	}
}