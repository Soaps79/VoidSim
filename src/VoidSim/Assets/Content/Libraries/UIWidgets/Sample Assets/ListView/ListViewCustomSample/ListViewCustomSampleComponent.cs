﻿using UnityEngine;
using UnityEngine.UI;
using UIWidgets;

namespace UIWidgetsSamples
{
	/// <summary>
	/// ListViewCustom sample component.
	/// </summary>
	public class ListViewCustomSampleComponent : ListViewItem, IViewData<ListViewCustomSampleItemDescription>
	{
		/// <summary>
		/// Foreground graphics for coloring.
		/// </summary>
		public override Graphic[] GraphicsForeground {
			get {
				return new Graphic[] {Text, };
			}
		}

		/// <summary>
		/// Icon.
		/// </summary>
		[SerializeField]
		public Image Icon;

		/// <summary>
		/// Text.
		/// </summary>
		[SerializeField]
		public Text Text;

		/// <summary>
		/// Progressbar.
		/// </summary>
		[SerializeField]
		public Progressbar Progressbar;

		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="item">Item.</param>
		public void SetData(ListViewCustomSampleItemDescription item)
		{
			if (item==null)
			{
				Icon.sprite = null;
				Text.text = string.Empty;
				Progressbar.Value = 0;
			}
			else
			{
				Icon.sprite = item.Icon;
				Text.text = item.Name;
				Progressbar.Value = item.Progress;
			}

			Icon.SetNativeSize();
			//set transparent color if no icon
			Icon.color = (Icon.sprite==null) ? Color.clear : Color.white;
		}
	}
}