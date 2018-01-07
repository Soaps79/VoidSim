using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;
using System;
using System.Linq;

namespace UIWidgets
{
	/// <summary>
	/// TreeViewCustom.
	/// </summary>
	public class TreeViewCustom<TComponent,TItem> : ListViewCustom<TComponent,ListNode<TItem>> where TComponent : TreeViewComponentBase<TItem>
	{
		/// <summary>
		/// NodeEvent.
		/// </summary>
		[Serializable]
		public class NodeEvent : UnityEvent<TreeNode<TItem>>
		{

		}

		[SerializeField]
		ObservableList<TreeNode<TItem>> nodes = null;

		/// <summary>
		/// Gets or sets the nodes.
		/// </summary>
		/// <value>The nodes.</value>
		public virtual ObservableList<TreeNode<TItem>> Nodes {
			get {
				return nodes;
			}
			set {
				if (!isTreeViewCustomInited)
				{
					Init();
				}
				
				if (nodes!=null)
				{
					nodes.OnChange -= NodesChanged;

					var new_root = new TreeNode<TItem>(default(TItem));
					new_root.Nodes = nodes;
				}
				nodes = value;
				RootNode.Nodes = value;
				SetScrollValue(0f);
				Refresh();
				if (nodes!=null)
				{
					nodes.OnChange += NodesChanged;
				}
			}
		}

		/// <summary>
		/// Gets the selected node.
		/// </summary>
		/// <value>The selected node.</value>
		public TreeNode<TItem> SelectedNode {
			get {
				return selectedNodes.LastOrDefault();
			}
		}

		/// <summary>
		/// NodeToggle event.
		/// </summary>
		public NodeEvent NodeToggle = new NodeEvent();

		/// <summary>
		/// NodeSelected event.
		/// </summary>
		public NodeEvent NodeSelected = new NodeEvent();

		/// <summary>
		/// NodeDeselected event.
		/// </summary>
		public NodeEvent NodeDeselected = new NodeEvent();

		/// <summary>
		/// The selected nodes.
		/// </summary>
		protected HashSet<TreeNode<TItem>> selectedNodes = new HashSet<TreeNode<TItem>>();

		/// <summary>
		/// Gets or sets the selected nodes.
		/// </summary>
		/// <value>The selected nodes.</value>
		public List<TreeNode<TItem>> SelectedNodes {
			get {
				return selectedNodes.ToList();
			}
			set {
				selectedNodes.Clear();
				SelectedIndices = Nodes2Indices(value);
			}
		}

		[SerializeField]
		bool deselectCollapsedNodes = true;

		/// <summary>
		/// The deselect collapsed nodes.
		/// </summary>
		public bool DeselectCollapsedNodes {
			get {
				return deselectCollapsedNodes;
			}
			set {
				deselectCollapsedNodes = value;
				if (value)
				{
					selectedNodes.Clear();
					foreach (var item in SelectedItems)
					{
						selectedNodes.Add(item.Node);
					}
				}
			}
		}

		/// <summary>
		/// Opened nodes converted to list.
		/// </summary>
		protected ObservableList<ListNode<TItem>> NodesList = new ObservableList<ListNode<TItem>>();

		[System.NonSerialized]
		bool isTreeViewCustomInited = false;

		TreeNode<TItem> rootNode;

		/// <summary>
		/// Gets the root node.
		/// </summary>
		/// <value>The root node.</value>
		protected TreeNode<TItem> RootNode {
			get {
				return rootNode;
			}
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		public override void Init()
		{
			if (isTreeViewCustomInited)
			{
				return ;
			}
			isTreeViewCustomInited = true;

			rootNode = new TreeNode<TItem>(default(TItem));

			setContentSizeFitter = false;

			base.Init();

			Refresh();

			KeepSelection = true;

			DataSource = NodesList;
		}

		/// <summary>
		/// Determines whether this instance can optimize.
		/// </summary>
		/// <returns><c>true</c> if this instance can optimize; otherwise, <c>false</c>.</returns>
		protected override bool CanOptimize()
		{
			var scrollRectSpecified = scrollRect!=null;
			var containerSpecified = Container!=null;
			var currentLayout = containerSpecified ? ((layout!=null) ? layout : Container.GetComponent<LayoutGroup>()) : null;
			var validLayout = currentLayout is EasyLayout.EasyLayout;
			
			return scrollRectSpecified && validLayout;
		}

		/// <summary>
		/// Convert nodes tree to list.
		/// </summary>
		/// <returns>The list.</returns>
		/// <param name="sourceNodes">Source nodes.</param>
		/// <param name="depth">Depth.</param>
		/// <param name="list">List.</param>
		protected virtual int Nodes2List(IObservableList<TreeNode<TItem>> sourceNodes, int depth, ObservableList<ListNode<TItem>> list)
		{
			var added_nodes = 0;
			foreach (var node in sourceNodes)
			{
				if (!node.IsVisible)
				{
					node.Index = -1;
					ResetNodesIndex(node.Nodes);
					continue ;
				}
				list.Add(new ListNode<TItem>(node, depth));
				node.Index = list.Count - 1;

				if ((node.IsExpanded) && (node.Nodes!=null) && (node.Nodes.Count > 0))
				{
					var used = Nodes2List(node.Nodes, depth + 1, list);
					node.UsedNodesCount = used;
				}
				else
				{
					ResetNodesIndex(node.Nodes);
					node.UsedNodesCount = 0;
				}
				added_nodes += 1;
			}
			return added_nodes;
		}

		/// <summary>
		/// Reset nodes indexes.
		/// </summary>
		/// <param name="sourceNodes">Source nodes.</param>
		protected virtual void ResetNodesIndex(IObservableList<TreeNode<TItem>> sourceNodes)
		{
			if (sourceNodes==null)
			{
				return ;
			}
			foreach (var node in sourceNodes)
			{
				node.Index = -1;
				ResetNodesIndex(node.Nodes);
			}
		}

		/// <summary>
		/// Raises the toggle node event.
		/// </summary>
		/// <param name="index">Index.</param>
		protected void OnToggleNode(int index)
		{
			ToggleNode(index);
			NodeToggle.Invoke(NodesList[index].Node);
		}

		/// <summary>
		/// Expand the parent nodes.
		/// </summary>
		/// <param name="node">Node.</param>
		public virtual void ExpandParentNodes(TreeNode<TItem> node)
		{
			foreach (var parent in node.Path)
			{
				parent.IsExpanded = true;
			}
		}

		/// <summary>
		/// Collapse the parent nodes.
		/// </summary>
		/// <param name="node">Node.</param>
		public virtual void CollapseParentNodes(TreeNode<TItem> node)
		{
			foreach (var parent in node.Path)
			{
				parent.IsExpanded = false;
			}
		}
		
		/// <summary>
		/// Select the node.
		/// </summary>
		/// <param name="node">Node.</param>
		public void SelectNode(TreeNode<TItem> node)
		{
			if (!IsNodeInTree(node))
			{
				return ;
			}

			var index = Node2Index(node);
			if (IsValid(index))
			{
				Select(index);
			}
			else if (index==-1 && !DeselectCollapsedNodes)
			{
				selectedNodes.Add(node);
			}
		}

		/// <summary>
		/// Select the node with subnodes.
		/// </summary>
		/// <param name="node">Node.</param>
		public void SelectNodeWithSubnodes(TreeNode<TItem> node)
		{
			if (!IsNodeInTree(node))
			{
				return ;
			}

			var queue = new Queue<TreeNode<TItem>>();
			queue.Enqueue(node);

			while (queue.Count > 0)
			{
				var current_node = queue.Dequeue();

				var index = Node2Index(current_node);

				if (index!=-1)
				{
					Select(index);
					if (current_node.Nodes!=null)
					{
						current_node.Nodes.ForEach(queue.Enqueue);
					}
				}
				else if (!DeselectCollapsedNodes)
				{
					selectedNodes.Add(current_node);
					if (current_node.Nodes!=null)
					{
						current_node.Nodes.ForEach(queue.Enqueue);
					}
				}
			}
		}

		/// <summary>
		/// Deselect the node.
		/// </summary>
		/// <param name="node">Node.</param>
		public void DeselectNode(TreeNode<TItem> node)
		{
			if (!IsNodeInTree(node))
			{
				return ;
			}

			var index = Node2Index(node);
			if (IsValid(index))
			{
				Deselect(index);
			}
			else if (index==-1 && !DeselectCollapsedNodes)
			{
				selectedNodes.Remove(node);
			}
		}

		/// <summary>
		/// Deselect the node with subnodes.
		/// </summary>
		/// <param name="node">Node.</param>
		public void DeselectNodeWithSubnodes(TreeNode<TItem> node)
		{
			if (!IsNodeInTree(node))
			{
				return ;
			}

			var queue = new Queue<TreeNode<TItem>>();
			queue.Enqueue(node);

			while (queue.Count > 0)
			{
				var current_node = queue.Dequeue();

				var index = Node2Index(current_node);

				if (index!=-1)
				{
					Deselect(index);
					if (current_node.Nodes!=null)
					{
						current_node.Nodes.ForEach(queue.Enqueue);
					}
				}
				else if (!DeselectCollapsedNodes)
				{
					selectedNodes.Remove(current_node);
					if (current_node.Nodes!=null)
					{
						current_node.Nodes.ForEach(queue.Enqueue);
					}
				}
			}
		}

		/// <summary>
		/// Invokes the select event.
		/// </summary>
		/// <param name="index">Index.</param>
		protected override void InvokeSelect(int index)
		{
			if (!IsValid(index))
			{
				Debug.LogWarning("Incorrect index: " + index, this);
			}

			var node = NodesList[index].Node;

			selectedNodes.Add(node);

			base.InvokeSelect(index);

			NodeSelected.Invoke(node);
		}

		/// <summary>
		/// Invokes the deselect event.
		/// </summary>
		/// <param name="index">Index.</param>
		protected override void InvokeDeselect(int index)
		{
			if (!IsValid(index))
			{
				Debug.LogWarning("Incorrect index: " + index, this);
			}

			var node = NodesList[index].Node;

			if (DeselectCollapsedNodes || !IsNodeInTree(node) || Node2Index(node)!=-1)
			{
				selectedNodes.Remove(node);
			}

			base.InvokeDeselect(index);

			NodeDeselected.Invoke(node);
		}

		/// <summary>
		/// Recalculates the selected indices.
		/// </summary>
		/// <returns>The selected indices.</returns>
		/// <param name="newItems">New items.</param>
		protected override List<int> RecalculateSelectedIndices(ObservableList<ListNode<TItem>> newItems)
		{
			if (DeselectCollapsedNodes)
			{
				return base.RecalculateSelectedIndices(newItems);
			}
			else
			{
				return selectedNodes.Select<TreeNode<TItem>,int>(x => newItems.FindIndex(l => l.Node==x)).Where(x => x!=-1).ToList();
			}
		}

		/// <summary>
		/// Determines whether specified node in tree.
		/// </summary>
		/// <returns><c>true</c> if tree contains specified node; otherwise, <c>false</c>.</returns>
		/// <param name="node">Node.</param>
		public bool IsNodeInTree(TreeNode<TItem> node)
		{
			return object.ReferenceEquals(RootNode, node.RootNode);
		}

		/// <summary>
		/// Toggles the node.
		/// </summary>
		/// <param name="index">Index.</param>
		protected virtual void ToggleNode(int index)
		{
			var node = NodesList[index];

			node.Node.IsExpanded = !node.Node.IsExpanded;
		}

		/// <summary>
		/// Get indices of specified nodes.
		/// </summary>
		/// <returns>The indices.</returns>
		/// <param name="targetNodes">Target nodes.</param>
		protected List<int> Nodes2Indices(IEnumerable<TreeNode<TItem>> targetNodes)
		{
			return targetNodes.Where(IsNodeInTree).Select<TreeNode<TItem>,int>(Node2Index).Where(x => x!=-1).ToList();
		}

		/// <summary>
		/// Get node index.
		/// </summary>
		/// <returns>The node index.</returns>
		/// <param name="node">Node.</param>
		public int Node2Index(TreeNode<TItem> node)
		{
			return node.Index;
		}

		/// <summary>
		/// Updata view when node data changed.
		/// </summary>
		protected virtual void NodesChanged()
		{
			Refresh();
		}

		/// <summary>
		/// Refresh this instance.
		/// </summary>
		public virtual void Refresh()
		{
			if (nodes==null)
			{
				NodesList.Clear();

				return ;
			}

			NodesList.BeginUpdate();

			var selected_nodes = SelectedNodes;
			NodesList.Clear();

			Nodes2List(nodes, 0, NodesList);

			SilentDeselect(SelectedIndices);
			var indices = Nodes2Indices(selected_nodes);

			SilentSelect(indices);

			NodesList.EndUpdate();

			if (DeselectCollapsedNodes)
			{
				selectedNodes.Clear();
				foreach (var item in SelectedItems)
				{
					selectedNodes.Add(item.Node);
				}
			}
		}

		/// <summary>
		/// Clear items of this instance.
		/// </summary>
		public override void Clear()
		{
			nodes.Clear();
			SetScrollValue(0f);
		}

		/// <summary>
		/// [Not supported for TreeView] Add the specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <returns>Index of added item.</returns>
		[Obsolete("Not supported for TreeView", true)]
		public virtual int Add(TItem item)
		{
			return -1;
		}
		
		/// <summary>
		/// [Not supported for TreeView] Remove the specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <returns>Index of removed TItem.</returns>
		[Obsolete("Not supported for TreeView", true)]
		public virtual int Remove(TItem item)
		{
			return -1;
		}

		/// <summary>
		/// [Not supported for TreeView] Remove item by specifieitemsex.
		/// </summary>
		/// <returns>Index of removed item.</returns>
		/// <param name="index">Index.</param>
		public override void Remove(int index)
		{
			throw new NotSupportedException("Not supported for TreeView.");
		}

		/// <summary>
		/// [Not supported for TreeView] Set the specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <param name="allowDuplicate">If set to <c>true</c> allow duplicate.</param>
		/// <returns>Index of item.</returns>
		[Obsolete("Not supported for TreeView", true)]
		public virtual int Set(TItem item, bool allowDuplicate=true)
		{
			return -1;
		}

		/// <summary>
		/// Removes first elements that match the conditions defined by the specified predicate.
		/// </summary>
		/// <param name="match">Match.</param>
		/// <returns>true if item is successfully removed; otherwise, false.</returns>
		public bool Remove(Predicate<TreeNode<TItem>> match)
		{
			var index = nodes.FindIndex(match);
			if (index!=-1)
			{
				nodes.RemoveAt(index);
				return true;
			}
			foreach (var node in nodes)
			{
				if (node.Nodes==null)
				{
					continue ;
				}
				if (Remove(node.Nodes, match))
				{
					return true;
				}
			}
			return false;
		}
		
		static bool Remove(ObservableList<TreeNode<TItem>> nodes, Predicate<TreeNode<TItem>> match)
		{
			var index = nodes.FindIndex(match);
			if (index!=-1)
			{
				nodes.RemoveAt(index);
				return true;
			}
			foreach (var node in nodes)
			{
				if (node.Nodes==null)
				{
					continue ;
				}
				if (Remove(node.Nodes, match))
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Removes the callback.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <param name="index">Index.</param>
		protected override void RemoveCallback(ListViewItem item, int index)
		{
			if (item!=null)
			{
				(item as TreeViewComponentBase<TItem>).ToggleEvent.RemoveListener(OnToggleNode);
			}

			base.RemoveCallback(item, index);
		}

		/// <summary>
		/// Adds the callback.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <param name="index">Index.</param>
		protected override void AddCallback(ListViewItem item, int index)
		{
			base.AddCallback(item, index);

			(item as TreeViewComponentBase<TItem>).ToggleEvent.AddListener(OnToggleNode);
		}
	}
}