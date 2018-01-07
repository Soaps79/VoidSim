using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using UIWidgets;

namespace UIWidgetsSamples
{
	/// <summary>
	/// Test TreeGraph.
	/// </summary>
	public class TestTreeGraph : MonoBehaviour
	{
		/// <summary>
		/// Graph.
		/// </summary>
		[SerializeField]
		public TreeGraph Graph;

		/// <summary>
		/// Nodes.
		/// </summary>
		protected ObservableList<TreeNode<TreeViewItem>> Nodes;

		/// <summary>
		/// Start this instance.
		/// </summary>
		protected virtual void Start()
		{
			Graph.Init();

			Graph.Nodes = Nodes;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public TestTreeGraph()
		{
			var config = new List<int>() {1, 3, 2, };
			//var config = new List<int>() {2, 3, 3, };
			//var config = new List<int>() {2, 3, 1, };

			Nodes = GenerateTreeNodes(config, isExpanded: true);
		}

		/// <summary>
		/// Generate tree nodes.
		/// </summary>
		/// <param name="items">Depth list.</param>
		/// <param name="nameStartsWith">Start part of name.</param>
		/// <param name="isExpanded">Is nodes expanded?</param>
		/// <returns>Nodes.</returns>
		static public ObservableList<TreeNode<TreeViewItem>> GenerateTreeNodes(List<int> items, string nameStartsWith = "Node ", bool isExpanded = true)
		{
			return Enumerable.Range(1, items[0]).Select(x => {
				var item_name = nameStartsWith + x;
				var item = new TreeViewItem(item_name, null);
				var nodes = items.Count > 1
					? GenerateTreeNodes(items.GetRange(1, items.Count - 1), item_name + " - ", isExpanded)
					: null;

				return new TreeNode<TreeViewItem>(item, nodes, isExpanded);
			}).ToObservableList();
		}
	}
}