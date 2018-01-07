﻿using UIWidgets;

namespace UIWidgetsSamples
{
	/// <summary>
	/// GroupedListView
	/// </summary>
	public class GroupedListView : ListViewCustomHeight<GroupedListViewComponent,IGroupedListItem>
	{
		/// <summary>
		/// Grouped data.
		/// </summary>
		public GroupedItems GroupedData = new GroupedItems();

		bool isGroupedListViewInited;

		/// <summary>
		/// Init this instance.
		/// </summary>
		public override void Init()
		{
			if (isGroupedListViewInited)
			{
				return ;
			}
			isGroupedListViewInited = true;

			base.Init();

			GroupedData.GroupComparison = (x, y) => x.Name.CompareTo(y.Name);
			GroupedData.Data = DataSource;
		}
	}
}