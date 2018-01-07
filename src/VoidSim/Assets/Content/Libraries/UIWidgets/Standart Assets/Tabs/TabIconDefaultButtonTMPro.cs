using UnityEngine;
using UIWidgets;

namespace UIWidgets.TMProSupport {
	/// <summary>
	/// TabIconDefaultButtonTMPro.
	/// </summary>
	public class TabIconDefaultButtonTMPro : TabIconButtonTMPro {
		/// <summary>
		/// Sets the data.
		/// </summary>
		/// <param name="tab">Tab.</param>
		public override void SetData(TabIcons tab)
		{
			Name.text = tab.Name;
			if (Icon!=null)
			{
				Icon.sprite = tab.IconDefault;

				if (SetNativeSize)
				{
					Icon.SetNativeSize();
				}

				Icon.color = (Icon.sprite==null) ? Color.clear : Color.white;
			}
		}
	}
}