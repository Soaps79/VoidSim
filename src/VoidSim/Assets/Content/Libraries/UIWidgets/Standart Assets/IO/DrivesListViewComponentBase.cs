namespace UIWidgets
{
	/// <summary>
	/// Base class for DrivesListViewComponent.
	/// </summary>
	abstract public class DrivesListViewComponentBase : ListViewItem, IViewData<FileSystemEntry>
	{
		/// <summary>
		/// Gets the current item.
		/// </summary>
		/// <value>Current item.</value>
		public FileSystemEntry Item {
			get;
			protected set;
		}

		/// <summary>
		/// Sets component data with specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		abstract public void SetData(FileSystemEntry item);
	}
}