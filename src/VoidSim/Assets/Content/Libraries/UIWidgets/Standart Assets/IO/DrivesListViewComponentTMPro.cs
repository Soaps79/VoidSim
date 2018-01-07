using TMPro;
using UnityEngine;

namespace UIWidgets.TMProSupport
{
	/// <summary>
	/// DrivesListViewComponent TMPro.
	/// Display drive.
	/// </summary>
	public class DrivesListViewComponentTMPro : DrivesListViewComponentBase
	{
		[SerializeField]
		TextMeshProUGUI Name;

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