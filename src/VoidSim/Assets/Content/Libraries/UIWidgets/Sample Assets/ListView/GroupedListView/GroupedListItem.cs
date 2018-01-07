using UIWidgets;

namespace UIWidgetsSamples
{
	/// <summary>
	/// GroupedListItem interface.
	/// </summary>
	public interface IGroupedListItem : IItemHeight
	{
		/// <summary>
		/// Name.
		/// </summary>
		string Name {
			get;
			set;
		}
	}

	/// <summary>
	/// Group class for GroupedList.
	/// </summary>
	public class GroupedListGroup : IGroupedListItem
	{
		/// <summary>
		/// Name.
		/// </summary>
		public string Name {
			get;
			set;
		}

		/// <summary>
		/// Displayed item height.
		/// </summary>
		public float Height {
			get ;
			set ;
		}
	}

	/// <summary>
	/// Item class for GroupedList.
	/// </summary>
	public class GroupedListItem : IGroupedListItem
	{
		/// <summary>
		/// Name.
		/// </summary>
		public string Name {
			get;
			set;
		}

		/// <summary>
		/// Displayed item height.
		/// </summary>
		public float Height {
			get ;
			set ;
		}
	}
}