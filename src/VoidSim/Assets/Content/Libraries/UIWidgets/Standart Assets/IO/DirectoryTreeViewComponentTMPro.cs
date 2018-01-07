using UnityEngine.UI;
using UIWidgets;
using TMPro;

namespace UIWidgets.TMProSupport {
	/// <summary>
	/// DirectoryTreeView component TMPro.
	/// </summary>
	public class DirectoryTreeViewComponentTMPro : DirectoryTreeViewComponent {
		/// <summary>
		/// Foreground graphics for coloring.
		/// </summary>
		public override Graphic[] GraphicsForeground {
			get {
				return new Graphic[] {TextTMPro, };
			}
		}

		/// <summary>
		/// Text.
		/// </summary>
		public TextMeshProUGUI TextTMPro;

		/// <summary>
		/// Updates the view.
		/// </summary>
		protected override void UpdateView()
		{
			TextTMPro.text = Node.Item.DisplayName;
		}
	}
}