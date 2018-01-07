using UnityEngine;
using System.Linq;
using System;
using System.Collections.Generic;

namespace UIWidgets
{
	/// <summary>
	/// ListViewIcons.
	/// </summary>
	[AddComponentMenu("UI/UIWidgets/ListViewIcons")]
	public class ListViewIcons : ListViewCustom<ListViewIconsItemComponent,ListViewIconsItemDescription>
	{
		[System.NonSerialized]
		bool isListViewIconsInited = false;

		/// <summary>
		/// Items comparison.
		/// </summary>
		protected Comparison<ListViewIconsItemDescription> ItemsComparison =
			(x, y) => (x.LocalizedName ?? x.Name).CompareTo(y.LocalizedName ?? y.Name);

		/// <summary>
		/// Init this instance.
		/// </summary>
		public override void Init()
		{
			if (isListViewIconsInited)
			{
				return ;
			}
			isListViewIconsInited = true;

			base.Init();

			SortFunc = list => list.OrderBy(item => item.LocalizedName ?? item.Name);
			//DataSource.Comparison = ItemsComparison;
		}
	}
}