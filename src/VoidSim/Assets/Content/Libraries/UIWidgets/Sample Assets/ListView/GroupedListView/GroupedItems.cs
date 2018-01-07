using System;
using System.Linq;
using UIWidgets;

namespace UIWidgetsSamples
{
	/// <summary>
	/// GroupedItems sample.
	/// items grouped by keys.
	/// </summary>
	public class GroupedItems : GroupedList<IGroupedListItem>
	{
		/// <summary>
		/// Get group for specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <returns>Group for specified item.</returns>
		protected override IGroupedListItem GetGroup(IGroupedListItem item)
		{
			var name = item.Name.Length > 0 ? item.Name[0].ToString() : "";
			var key = GroupsWithItems.Keys.FirstOrDefault(x => x.Name==name);
			if (key==null)
			{
				key = new GroupedListGroup(){
					Name = name
				};
			}
			return key;
		}
	}
}