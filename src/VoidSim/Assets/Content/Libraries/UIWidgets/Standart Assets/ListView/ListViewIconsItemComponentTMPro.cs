using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UIWidgets.TMProSupport {
	/// <summary>
	/// ListViewIcons item component.
	/// </summary>
	[AddComponentMenu("UI/UIWidgets/ListViewIconsItemComponentTMPro")]
	public class ListViewIconsItemComponentTMPro : ListViewIconsItemComponent {
		/// <summary>
		/// Foreground graphics for coloring.
		/// </summary>
		public override Graphic[] GraphicsForeground {
			get {
				return new Graphic[] {TextTMPro, };
			}
		}

		/// <summary>
		/// The text.
		/// </summary>
		[SerializeField]
		public TextMeshProUGUI TextTMPro;

		/// <summary>
		/// Sets component data with specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		public override void SetData(ListViewIconsItemDescription item)
		{
			Item = item;
			name = item==null ? "DefaultItem (Clone)" : item.Name;
			if (Item==null)
			{
				if (Icon!=null)
				{
					Icon.sprite = null;
				}
				TextTMPro.text = string.Empty;
			}
			else
			{
				if (Icon!=null)
				{
					Icon.sprite = Item.Icon;
				}
				TextTMPro.text = Item.LocalizedName ?? Item.Name;
			}

			if ((SetNativeSize) && (Icon!=null))
			{
				Icon.SetNativeSize();
			}

			//set transparent color if no icon
			if (Icon!=null)
			{
				Icon.color = (Icon.sprite==null) ? Color.clear : Color.white;
			}
		}
	}
}