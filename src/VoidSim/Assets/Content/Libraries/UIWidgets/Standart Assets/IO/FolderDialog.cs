using UnityEngine;
using UnityEngine.UI;

namespace UIWidgets
{
	/// <summary>
	/// DatePicker.
	/// </summary>
	[AddComponentMenu("UI/UIWidgets/FolderDialog")]
	public class FolderDialog : Picker<string,FolderDialog>
	{
		/// <summary>
		/// ListView.
		/// </summary>
		[SerializeField]
		public DirectoryTreeView DirectoryTreeView;

		/// <summary>
		/// OK button.
		/// </summary>
		[SerializeField]
		public Button OkButton;

		/// <summary>
		/// Prepare picker to open.
		/// </summary>
		/// <param name="defaultValue">Default value.</param>
		public override void BeforeOpen(string defaultValue)
		{
			DirectoryTreeView.SelectDirectory(defaultValue);

			DirectoryTreeView.NodeSelected.AddListener(NodeChanged);
			DirectoryTreeView.NodeDeselected.AddListener(NodeChanged);
			OkButton.onClick.AddListener(OkClick);
			NodeChanged(null);
		}

		/// <summary>
		/// Handle selected node event.
		/// </summary>
		/// <param name="node">Node.</param>
		protected virtual void NodeChanged(TreeNode<FileSystemEntry> node)
		{
			OkButton.interactable = DirectoryTreeView.SelectedNode!=null;
		}

		/// <summary>
		/// Handle OkButton click.
		/// </summary>
		public void OkClick()
		{
			var node = DirectoryTreeView.SelectedNode;
			if (node==null)
			{
				return ;
			}
			Selected(node.Item.FullName);
		}

		/// <summary>
		/// Prepare picker to close.
		/// </summary>
		public override void BeforeClose()
		{
			DirectoryTreeView.NodeSelected.RemoveListener(NodeChanged);
			OkButton.onClick.RemoveListener(OkClick);
		}
	}
}