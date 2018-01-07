
namespace UIWidgets
{
	/// <summary>
	/// ListViewInt base component.
	/// </summary>
	abstract public class ListViewIntComponentBase : ListViewItem, IViewData<int>
	{
		/// <summary>
		/// Sets the data.
		/// </summary>
		/// <param name="item">Item.</param>
		abstract public void SetData(int item);
	}
}