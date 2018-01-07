﻿using UnityEngine;
using UIWidgets;

namespace UIWidgetsSamples
{
	/// <summary>
	/// Test TreeViewCheckboxes.
	/// </summary>
	public class TestTreeViewCheckboxes : MonoBehaviour
	{
		/// <summary>
		/// TreeView.
		/// </summary>
		[SerializeField]
		public TreeViewCheckboxes Tree;

		/// <summary>
		/// Start this instance.
		/// </summary>
		protected virtual void Start()
		{
			Tree.Init();

			SetTreeNodes();

			Tree.OnNodeCheckboxChanged.AddListener(CheckboxChanged);
		}

		/// <summary>
		/// Set TreeView nodes.
		/// </summary>
		protected void SetTreeNodes()
		{
			var nodes = new ObservableList<TreeNode<TreeViewCheckboxesItem>>();
			nodes.Add(new TreeNode<TreeViewCheckboxesItem>(new TreeViewCheckboxesItem("Item 1")));
			nodes.Add(new TreeNode<TreeViewCheckboxesItem>(new TreeViewCheckboxesItem("Item 2")));
			nodes.Add(new TreeNode<TreeViewCheckboxesItem>(new TreeViewCheckboxesItem("Item 3")));

			nodes[0].Nodes = new ObservableList<TreeNode<TreeViewCheckboxesItem>>();
			nodes[0].Nodes.Add(new TreeNode<TreeViewCheckboxesItem>(new TreeViewCheckboxesItem("Item 1-1")));
			nodes[0].Nodes.Add(new TreeNode<TreeViewCheckboxesItem>(new TreeViewCheckboxesItem("Item 1-2")));

			Tree.Nodes = nodes;
		}

		/// <summary>
		/// Handle checkbox changed event.
		/// </summary>
		/// <param name="node">Node.</param>
		public void CheckboxChanged(TreeNode<TreeViewCheckboxesItem> node)
		{
			if (node.Item.Selected)
			{
				Debug.Log(node.Item.Name + " selected");
			}
			else
			{
				Debug.Log(node.Item.Name + " deselected");
			}
		}

		/// <summary>
		/// Remove listeners.
		/// </summary>
		protected virtual void OnDestroy()
		{
			if (Tree!=null)
			{
				Tree.OnNodeCheckboxChanged.RemoveListener(CheckboxChanged);
			}
		}
	}
}