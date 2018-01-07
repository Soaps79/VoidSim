﻿using UnityEngine;
using UIWidgets;

namespace UIWidgets.TMProSupport {
	/// <summary>
	/// TabIconActiveButtonTMPro.
	/// </summary>
	public class TabIconActiveButtonTMPro : TabIconButtonTMPro {
		/// <summary>
		/// Sets the data.
		/// </summary>
		/// <param name="tab">Tab.</param>
		public override void SetData(TabIcons tab)
		{
			Name.text = tab.Name;
			if (Icon!=null)
			{
				Icon.sprite = tab.IconActive;

				if (SetNativeSize)
				{
					Icon.SetNativeSize();
				}

				Icon.color = (Icon.sprite==null) ? Color.clear : Color.white;
			}
		}
	}
}