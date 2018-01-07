using UnityEngine;

namespace UIWidgets
{
	/// <summary>
	/// TreeViewNode drag support.
	/// </summary>
	[AddComponentMenu("UI/UIWidgets/TreeViewNodeDragSupport")]
	[RequireComponent(typeof(TreeViewComponent))]
	public class TreeViewNodeDragSupport : TreeViewCustomNodeDragSupport<TreeViewComponent,ListViewIconsItemComponent,TreeViewItem>
	{
		
	}
}