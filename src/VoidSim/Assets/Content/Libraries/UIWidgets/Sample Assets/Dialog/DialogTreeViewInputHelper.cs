using UnityEngine;
using UIWidgets;
using System.Collections.Generic;

namespace UIWidgetsSamples
{
	/// <summary>
	/// DialogTreeViewInputHelper
	/// </summary>
	public class DialogTreeViewInputHelper : MonoBehaviour
	{
		/// <summary>
		/// TreeView.
		/// </summary>
		[SerializeField]
		public TreeView Folders;
		
		ObservableList<TreeNode<TreeViewItem>> nodes;
		bool _InitDone;

		/// <summary>
		/// Init.
		/// </summary>
		public void Refresh()
		{
			if (!_InitDone)
			{
				var config = new List<int>() { 5, 5, 2 };
				nodes = TestTreeView.GenerateTreeNodes(config, isExpanded: true);

				// Set nodes
				Folders.Init();
				Folders.Nodes = nodes;
				
				_InitDone = true;
			}
		}
	}
}