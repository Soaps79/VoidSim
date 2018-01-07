using UnityEngine;
using UnityEngine.UI;
using UIWidgets;
using System.Collections.Generic;

namespace UIWidgetsSamples
{
	/// <summary>
	/// KeyValueListView item.
	/// </summary>
	public class KeyValueListViewItem : ListViewItem, IViewData<KeyValuePair<string,string>>
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
		/// Text.
		/// </summary>
		[SerializeField]
		public Text Text;

		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="item">Item.</param>
		public void SetData(KeyValuePair<string,string> item)
		{
			Text.text = item.Value;
		}
	}
}