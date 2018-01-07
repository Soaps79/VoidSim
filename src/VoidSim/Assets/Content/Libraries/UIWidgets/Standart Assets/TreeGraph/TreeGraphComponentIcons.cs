using UnityEngine;
using UnityEngine.UI;

namespace UIWidgets
{
	/// <summary>
	/// TreeGraph component with icons.
	/// </summary>
	public class TreeGraphComponentIcons : TreeGraphComponent<TreeViewItem>
	{
		/// <summary>
		/// Name.
		/// </summary>
		[SerializeField]
		public Text Name;

		/// <summary>
		/// Node.
		/// </summary>
		protected TreeNode<TreeViewItem> Node;

		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="node">Node.</param>
		public override void SetData(TreeNode<TreeViewItem> node)
		{
			Node = node;

			Name.text = node.Item.LocalizedName ?? node.Item.Name;

			name = node.Item.Name;
		}

		/// <summary>
		/// Toggle node visibility.
		/// </summary>
		public void ToggleVisibility()
		{
			Node.IsVisible = !Node.IsVisible;
		}

		/// <summary>
		/// Toggle expanded.
		/// </summary>
		public void ToggleExpanded()
		{
			Node.IsExpanded = !Node.IsExpanded;
		}
	}
}