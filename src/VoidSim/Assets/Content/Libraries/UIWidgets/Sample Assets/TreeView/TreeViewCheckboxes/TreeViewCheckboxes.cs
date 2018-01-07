using UIWidgets;

namespace UIWidgetsSamples
{
	/// <summary>
	/// TreeView with checkboxes.
	/// </summary>
	public class TreeViewCheckboxes : TreeViewCustom<TreeViewCheckboxesComponent,TreeViewCheckboxesItem>
	{
		/// <summary>
		/// NodeCheckboxChanged event.
		/// </summary>
		public NodeEvent OnNodeCheckboxChanged = new NodeEvent();

		void NodeCheckboxChanged(int index)
		{
			OnNodeCheckboxChanged.Invoke(DataSource[index].Node);
		}

		/// <summary>
		/// Add callback.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <param name="index">Index.</param>
		protected override void AddCallback(ListViewItem item, int index)
		{
			base.AddCallback(item, index);

			(item as TreeViewCheckboxesComponent).NodeCheckboxChanged.AddListener(NodeCheckboxChanged);
		}

		/// <summary>
		/// Remove callback.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <param name="index">Index.</param>
		protected override void RemoveCallback(ListViewItem item, int index)
		{
			if (item!=null)
			{
				(item as TreeViewCheckboxesComponent).NodeCheckboxChanged.RemoveListener(NodeCheckboxChanged);
				base.RemoveCallback(item, index);
			}
		}
	}
}