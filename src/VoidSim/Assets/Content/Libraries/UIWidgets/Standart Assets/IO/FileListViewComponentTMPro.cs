using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UIWidgets.TMProSupport
{
	/// <summary>
	/// FileListView component TMPro.
	/// </summary>
	public class FileListViewComponentTMPro : FileListViewComponentBase
	{
		/// <summary>
		/// Name.
		/// </summary>
		[SerializeField]
		protected TextMeshProUGUI Name;

		/// <summary>
		/// Foreground graphics.
		/// </summary>
		public override Graphic[] GraphicsForeground {
			get {
				return new Graphic[] { Name };
			}
		}

		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="item">Item.</param>
		public override void SetData(FileSystemEntry item)
		{
			base.SetData(item);

			Name.text = item.DisplayName;
		}
	}
}