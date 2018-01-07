using UnityEngine;
using UnityEngine.UI;
using UIWidgets;
using TMPro;

namespace UIWidgets.TMProSupport {
	/// <summary>
	/// TreeView component TMPro.
	/// </summary>
	public class TreeViewComponentTMPro : TreeViewComponent {
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
			if ((Icon==null) || (TextTMPro==null))
			{
				return ;
			}
				
			if (Item==null)
			{
				Icon.sprite = null;
				TextTMPro.text = string.Empty;
			}
			else
			{
				Icon.sprite = Item.Icon;
				TextTMPro.text = Item.LocalizedName ?? Item.Name;
			}
			
			if (SetNativeSize)
			{
				Icon.SetNativeSize();
			}
			
			//set transparent color if no icon
			Icon.color = (Icon.sprite==null) ? Color.clear : Color.white;
		}
	}
}