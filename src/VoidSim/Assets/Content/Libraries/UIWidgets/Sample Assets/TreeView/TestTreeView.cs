using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using UIWidgets;
using UnityEngine.Serialization;

namespace UIWidgetsSamples
{
	/// <summary>
	/// Test TreeView.
	/// </summary>
	public class TestTreeView : MonoBehaviour
	{
		/// <summary>
		/// TreeView.
		/// </summary>
		[SerializeField]
		public TreeView Tree;

		/// <summary>
		/// Nodes.
		/// </summary>
		protected ObservableList<TreeNode<TreeViewItem>> Nodes;

		/// <summary>
		/// UINodes.
		/// </summary>
		protected ObservableList<TreeNode<TreeViewItem>> UINodes;

		/// <summary>
		/// Default nodes.
		/// </summary>
		protected Dictionary<string, ObservableList<TreeNode<TreeViewItem>>> DefaultNodes;

		/// <summary>
		/// Constructor.
		/// </summary>
		public TestTreeView()
		{
			var config = new List<int>() { 1, 4, 10, 10, };

			Nodes = GenerateTreeNodes(config, isExpanded: true);
		}

		/// <summary>
		/// Start this instance.
		/// </summary>
		protected virtual void Start()
		{
			Tree.Init();

			Tree.Nodes = Nodes;

			//Tree.OnSelect.AddListener(CheckSelectedNode);
			//SetComparison();
		}

		/// <summary>
		/// Handle selected node event.
		/// Deselect node if match some condition.
		/// </summary>
		/// <param name="index">Node index.</param>
		/// <param name="component">Component.</param>
		protected virtual void CheckSelectedNode(int index, ListViewItem component)
		{
			if (Tree.DataSource[index].Node.TotalNodesCount > 1)
			{
				Tree.Deselect(index);
			}
		}

		/// <summary>
		/// Log selected node.
		/// </summary>
		public void TestSelect()
		{
			Debug.Log(Tree.SelectedIndex);
			Debug.Log(Tree.SelectedItem.Node.Item.Name);
		}

		/// <summary>
		/// Toggle node IsExpanded and measure how much time it's takes.
		/// </summary>
		/// <param name="node">Node.</param>
		protected virtual void NodeToggle(TreeNode<TreeViewItem> node)
		{
			Utilites.CheckRunTime(() => node.IsExpanded = !node.IsExpanded, "nodes: " + node.TotalNodesCount);
		}

		/// <summary>
		/// Add listener.
		/// </summary>
		public void TestSelectAll()
		{
			Tree.NodeDeselected.AddListener(OnNodeDeselected);
		}

		/// <summary>
		/// Is now handle selected node?
		/// </summary>
		protected bool handleSelected = false;

		/// <summary>
		/// Handle node deselected event.
		/// </summary>
		/// <param name="node">Node.</param>
		protected void OnNodeDeselected(TreeNode<TreeViewItem> node)
		{
			if (handleSelected)
			{
				return ;
			}
			handleSelected = true;

			Tree.DeselectNodeWithSubnodes(node);
			//Utilites.CheckRunTime(() => Tree.DeselectNodeWithSubnodes(node), "nodes: " + node.TotalNodesCount);

			handleSelected = false;
		}

		/// <summary>
		/// Handle node selected event.
		/// </summary>
		/// <param name="node">Node.</param>
		protected void OnNodeSelected(TreeNode<TreeViewItem> node)
		{
			if (handleSelected)
			{
				return ;
			}

			handleSelected = true;

			Tree.SelectNodeWithSubnodes(node);
			//Utilites.CheckRunTime(() => Tree.SelectNodeWithSubnodes(node), "nodes: " + node.TotalNodesCount);

			handleSelected = false;
		}

		/// <summary>
		/// Select fist node with subnodes and measure how much time it's takes.
		/// </summary>
		public void SelectFirstNodeWithSubnodes()
		{
			Utilites.CheckRunTime(() => Tree.SelectNodeWithSubnodes(Nodes[0]));
		}

		/// <summary>
		/// Select first subnode from second node.
		/// </summary>
		public void SelectNode()
		{
			Tree.SelectNode(Nodes[1].Nodes[0]);
		}

		/// <summary>
		/// Select node with subnodes.
		/// </summary>
		public void SelectNodeWithSubnodes()
		{
			Tree.SelectNodeWithSubnodes(Nodes[1].Nodes[1]);
		}

		/// <summary>
		/// Select node and scroll to it.
		/// </summary>
		public void TestSelectAndScrollTo()
		{
			SelectAndScrollTo(Nodes[2].Nodes[1].Nodes[1]);
		}

		/// <summary>
		/// Select node and scroll to it.
		/// </summary>
		/// <param name="node">Node.</param>
		public void SelectAndScrollTo(TreeNode<TreeViewItem> node)
		{
			// expand parent nodes
			Tree.ExpandParentNodes(node);

			// select node
			Tree.SelectNode(node);

			// scroll to node immediately
			//Tree.ScrollTo(Tree.SelectedIndex);

			// scroll to node animated
			Tree.ScrollToAnimated(Tree.SelectedIndex);
		}

		/// <summary>
		/// Use duplicates.
		/// </summary>
		public void UseDuplicates()
		{
			Nodes.OnChange += UpdateUINodes;

			UpdateUINodes();
		}

		void UpdateUINodes()
		{
			UINodes = DuplicateNodes(Nodes);

			// some code to set UINodes comparison

			Tree.Nodes = UINodes;

			// imporant: TreeView events will return index in UINodes, not nodes
		}

		ObservableList<TreeNode<T>> DuplicateNodes<T>(ObservableList<TreeNode<T>> source)
		{
			var result = new ObservableList<TreeNode<T>>();

			foreach (var node in source)
			{
				result.Add(new TreeNode<T>(
					node.Item,
					(node.Nodes==null) ? null : DuplicateNodes(node.Nodes),
					node.IsExpanded,
					node.IsVisible
				));
			}

			return result;
		}

		/// <summary>
		/// Set node sort.
		/// </summary>
		public void SetComparison()
		{
			Nodes[0].Nodes.Comparison = comparisonDesc;
			Nodes.Comparison = comparisonDesc;
		}

		/// <summary>
		/// Add subnodes.
		/// </summary>
		public void AddSubNodes()
		{
			if (Nodes.Count==0)
			{
				return ;
			}
			// get parent node
			var node = Nodes[0];
			// or find parent node by name
			// var node = nodes.Find(x => x.Item.Name = "Node 2");

			if (node.Nodes==null)
			{
				node.Nodes = new ObservableList<TreeNode<TreeViewItem>>();
			}
			var new_item1 = new TreeViewItem("Subnode 1");
			var new_node1 = new TreeNode<TreeViewItem>(new_item1);

			var new_item2 = new TreeViewItem("Subnode 2");
			var new_node2 = new TreeNode<TreeViewItem>(new_item2);

			var new_item3 = new TreeViewItem("Subnode 3");
			var new_node3 = new TreeNode<TreeViewItem>(new_item3);

			node.Nodes.BeginUpdate();

			node.Nodes.Add(new_node1);
			node.Nodes.Add(new_node2);
			node.Nodes.Add(new_node3);

			node.Nodes.EndUpdate();
		}

		/// <summary>
		/// Add node and scroll to it.
		/// </summary>
		public void ScrollToNewNode()
		{
			var new_item = new TreeViewItem("New node");
			var new_node = new TreeNode<TreeViewItem>(new_item);
			//nodes[0].Nodes.Add(new_node);
			Nodes.Add(new_node);
			//nodes.Insert(0, new_node);

			//ScrollToNode(new_node);
			ScrollToNodeAnimated(new_node);
		}

		/// <summary>
		/// Scroll to specified node.
		/// </summary>
		/// <param name="node">Node.</param>
		public void ScrollToNode(TreeNode<TreeViewItem> node)
		{
			// find index of node, DataSource contains list of visible nodes
			var index = Tree.Node2Index(node);
			// if node exists and visible
			if (index!=-1)
			{
				//scroll to node
				Tree.ScrollTo(index);
			}
		}

		/// <summary>
		/// Scroll to node animated.
		/// </summary>
		/// <param name="node">Node.</param>
		public void ScrollToNodeAnimated(TreeNode<TreeViewItem> node)
		{
			// expand node, so DataSource contains list of visible nodes
			Tree.ExpandParentNodes(node);

			// get node index
			var index = Tree.Node2Index(node);
			
			// if node exists and visible
			if (index!=-1)
			{
				//scroll to node
				Tree.ScrollToAnimated(index);
			}
		}

		/// <summary>
		/// Log selected nodes.
		/// </summary>
		public void GetSelectedNodes()
		{
			Debug.Log(Tree.SelectedIndex);
			Debug.Log(string.Join(",", Tree.SelectedIndices.Select(x => x.ToString()).ToArray()));
			var selectedNodes = Tree.SelectedNodes;
			if (selectedNodes!=null)
			{
				selectedNodes.ForEach(node => Debug.Log(node.Item.Name));
			}

		}

		/// <summary>
		/// Get node path.
		/// </summary>
		public void GetNodePath()
		{
			var path = Nodes[0].Nodes[0].Nodes[0].Path;
			path.ForEach(node => Debug.Log(node.Item.Name));
		}

		/// <summary>
		/// Select nodes.
		/// </summary>
		public void SelectNodes()
		{
			if ((Nodes.Count==0) || (Nodes[0].Nodes.Count==0))
			{
				return ;
			}
			// replace on find node "Node 1 - 1"
			var parent_node = Nodes[0].Nodes[0];
			var children = new List<TreeNode<TreeViewItem>>();
			GetChildrenNodes(parent_node, children);

			// add children to selected nodes
			Tree.SelectedNodes = Tree.SelectedNodes.Union(children).ToList();

			// select only children
			//Tree.SelectedNodes = children;
		}

		/// <summary>
		/// Deselect nodes.
		/// </summary>
		public void DeselectNodes()
		{
			if ((Nodes.Count==0) || (Nodes[0].Nodes.Count==0))
			{
				return ;
			}
			// replace on find node "Node 1 - 1"
			var parent_node = Nodes[0].Nodes[0];
			var children = new List<TreeNode<TreeViewItem>>();
			GetChildrenNodes(parent_node, children);

			// remove children from selected nodes
			Tree.SelectedNodes = Tree.SelectedNodes.Except(children).ToList();

			// deselect all
			//Tree.SelectedNodes = new List<TreeNode<TreeViewItem>>();
		}

		void GetChildrenNodes(TreeNode<TreeViewItem> node, List<TreeNode<TreeViewItem>> children)
		{
			if (node.Nodes!=null)
			{
				children.AddRange(node.Nodes);
				node.Nodes.ForEach(x => GetChildrenNodes(x, children));
			}
		}

		/// <summary>
		/// Only one node can be selected at once.
		/// </summary>
		public void SetOnlyOnSelectable()
		{
			Tree.MultipleSelect = false;
		}

		/// <summary>
		/// Multiple nodes can be selected at once.
		/// </summary>
		public void SetMultipleSelectable()
		{
			Tree.MultipleSelect = true;
		}

		/// <summary>
		/// Compare nodes by Name in ascending order.
		/// </summary>
		protected Comparison<TreeNode<TreeViewItem>> comparisonAsc = (x, y) => {
			return (x.Item.LocalizedName ?? x.Item.Name).CompareTo(y.Item.LocalizedName ?? y.Item.Name);
		};

		/// <summary>
		/// Compare nodes by Name in descending order.
		/// </summary>
		protected Comparison<TreeNode<TreeViewItem>> comparisonDesc = (x, y) => {
			return -(x.Item.LocalizedName ?? x.Item.Name).CompareTo(y.Item.LocalizedName ?? y.Item.Name);
		};

		/// <summary>
		/// Sort in ascending order.
		/// </summary>
		public void SortAsc()
		{
			Nodes.BeginUpdate();
			ApplyNodesSort(Nodes, comparisonAsc);
			Nodes.EndUpdate();
		}

		/// <summary>
		/// Sort in descending order.
		/// </summary>
		public void SortDesc()
		{
			Nodes.BeginUpdate();
			ApplyNodesSort(Nodes, comparisonDesc);
			Nodes.EndUpdate();
		}

		/// <summary>
		/// Apply nodes sort.
		/// </summary>
		/// <typeparam name="T">Node type.</typeparam>
		/// <param name="nodes">Nodes.</param>
		/// <param name="comparison">Nodes comparison.</param>
		public static void ApplyNodesSort<T>(ObservableList<TreeNode<T>> nodes, Comparison<TreeNode<T>> comparison)
		{
			nodes.Sort(comparison);
			nodes.ForEach(node => {
				if (node.Nodes!=null)
				{
					ApplyNodesSort(node.Nodes, comparison);
				}
			});
		}

		/// <summary>
		/// Remove node by name.
		/// </summary>
		/// <param name="name">Name.</param>
		public void TestRemove(string name)
		{
			RemoveByName(Nodes, name);
		}

		/// <summary>
		/// Remove node by name.
		/// </summary>
		/// <param name="nodes">Nodes.</param>
		/// <param name="name">Name.</param>
		static public void RemoveByName(ObservableList<TreeNode<TreeViewItem>> nodes, string name)
		{
			Remove(nodes, x => x.Item.Name==name);
		}

		/// <summary>
		/// Remove node if it's match specified function.
		/// </summary>
		/// <typeparam name="T">Node type.</typeparam>
		/// <param name="nodes">Nodes.</param>
		/// <param name="match">Match function.</param>
		/// <returns>true if node removed; otherwise, false.</returns>
		static public bool Remove<T>(ObservableList<TreeNode<T>> nodes, Predicate<TreeNode<T>> match)
		{
			var findedNode = nodes.Find(match);
			if (findedNode!=null)
			{
				findedNode.Parent = null;
				//this.nodes.Add(findedNode as TreeNode<TreeViewItem>);
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
		/// Test node position.
		/// </summary>
		/// <param name="name">Name.</param>
		public void TestReorder(string name)
		{
			ChangePositionByName(Nodes, name, 0);
		}
		
		/// <summary>
		/// Change node position.
		/// </summary>
		/// <param name="nodes">Nodes.</param>
		/// <param name="name">Node name.</param>
		/// <param name="position">New position.</param>
		public static void ChangePositionByName(ObservableList<TreeNode<TreeViewItem>> nodes, string name, int position)
		{
			ChangePosition(nodes, x => x.Item.Name==name, position);
		}

		/// <summary>
		/// Change node position.
		/// </summary>
		/// <typeparam name="T">Node type.</typeparam>
		/// <param name="nodes">Nodes.</param>
		/// <param name="match">Node match.</param>
		/// <param name="position">New position.</param>
		/// <returns>true if position changed; otherwise, false.</returns>
		public static bool ChangePosition<T>(ObservableList<TreeNode<T>> nodes, Predicate<TreeNode<T>> match, int position)
		{
			var findedNode = nodes.Find(match);
			if (findedNode!=null)
			{
				nodes.BeginUpdate();
				nodes.Remove(findedNode);
				nodes.Insert(position, findedNode);
				nodes.EndUpdate();
				return true;
			}
			foreach (var node in nodes)
			{
				if (node.Nodes==null)
				{
					continue ;
				}
				if (ChangePosition(node.Nodes, match, position))
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Show tree with 10000 nodes.
		/// </summary>
		public void Test10K()
		{
			var config = new List<int>() {10, 10, 10, 10};
			Nodes = GenerateTreeNodes(config, isExpanded: true);

			Tree.Nodes = Nodes;
		}

		/// <summary>
		/// Icon for nodes.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("icon")]
		protected Sprite Icon;
		
		/// <summary>
		/// Show tree with long nodes names.
		/// </summary>
		public void LongNames()
		{
			var config = new List<int>() {2, 2, 2, 2, 2, 2, 2, 2, 2};
			Nodes = GenerateTreeNodes(config, isExpanded: true, icon: Icon);
			
			Tree.Nodes = Nodes;
		}

		/// <summary>
		/// Set nodes by key.
		/// </summary>
		/// <param name="k">Key.</param>
		public void PerformanceCheck(string k)
		{
			if (DefaultNodes==null)
			{
				DefaultNodes = new Dictionary<string, ObservableList<TreeNode<TreeViewItem>>>();

				var config1k = new List<int>() {10, 10, 10 };
				DefaultNodes.Add("1k", GenerateTreeNodes(config1k, isExpanded: true));
				
				var config5k = new List<int>() {5, 10, 10, 10 };
				DefaultNodes.Add("5k", GenerateTreeNodes(config5k, isExpanded: true));
				
				var config10k = new List<int>() {10, 10, 10, 10 };
				DefaultNodes.Add("10k", GenerateTreeNodes(config10k, isExpanded: true));
				
				var config50k = new List<int>() {5, 10, 10, 10, 10 };
				DefaultNodes.Add("50k", GenerateTreeNodes(config50k, isExpanded: true));
			}
			Nodes = DefaultNodes[k];
			Tree.Nodes = DefaultNodes[k];
		}

		/// <summary>
		/// Set tree nodes.
		/// </summary>
		public void SetTreeNodes()
		{
			Tree.Nodes = Nodes;

			Nodes.BeginUpdate();

			var test_item = new TreeViewItem("added");
			var test_node = new TreeNode<TreeViewItem>(test_item);
			Nodes.Add(test_node);
			Nodes[1].IsVisible = false;
			Nodes[2].Nodes[1].IsVisible = false;

			Nodes.EndUpdate();
		}

		/// <summary>
		/// Add node.
		/// </summary>
		public void AddNode()
		{
			var test_item = new TreeViewItem("New node");
			var test_node = new TreeNode<TreeViewItem>(test_item);
			Nodes.Add(test_node);
		}

		/// <summary>
		/// Toggle node.
		/// </summary>
		public void ToggleNode()
		{
			Nodes[0].Nodes[0].IsExpanded = !Nodes[0].Nodes[0].IsExpanded;
		}

		/// <summary>
		/// Change node names.
		/// </summary>
		public void ChangeNodesName()
		{
			Nodes[0].Item.Name = "Node renamed from code";
			Nodes[0].Nodes[1].Item.Name = "Another node renamed from code";
		}

		/// <summary>
		/// Reset filter.
		/// </summary>
		public void ResetFilter()
		{
			Nodes.BeginUpdate();

			Nodes.ForEach(SetVisible);

			Nodes.EndUpdate();
		}

		void SetVisible(TreeNode<TreeViewItem> node)
		{
			if (node.Nodes!=null)
			{
				node.Nodes.ForEach(SetVisible);
			}
			node.IsVisible = true;
		}

		/// <summary>
		/// Filter nodes by name.
		/// </summary>
		/// <param name="nameContains">Name.</param>
		public void Filter(string nameContains)
		{
			Nodes.BeginUpdate();

			SampleFilter(Nodes, x => x.Name.Contains(nameContains));

			Nodes.EndUpdate();
		}

		/// <summary>
		/// Clear nodes.
		/// </summary>
		public void Clear()
		{
			//nodes.Clear();
			Nodes = new ObservableList<TreeNode<TreeViewItem>>();
			Tree.Nodes = Nodes;
		}

		/// <summary>
		/// Sample filter.
		/// Set node.IsVisible if it's match condition or it's subnodes match condition.
		/// </summary>
		/// <param name="nodes">Nodes.</param>
		/// <param name="filterFunc">Match function.</param>
		/// <returns>true if any node match condition; otherwise, false.</returns>
		protected static bool SampleFilter(ObservableList<TreeNode<TreeViewItem>> nodes, Func<TreeViewItem,bool> filterFunc)
		{
			return nodes.Count(x => {
				var have_visible_children = (x.Nodes!=null) && SampleFilter(x.Nodes, filterFunc);
				x.IsVisible = have_visible_children || filterFunc(x.Item) ;
				return x.IsVisible;
			}) > 0;
		}

		/// <summary>
		/// Generate nodes.
		/// </summary>
		/// <param name="items">Depth list.</param>
		/// <param name="nameStartsWith">Start part of name.</param>
		/// <param name="isExpanded">Is nodes expanded?</param>
		/// <param name="icon">Icon</param>
		/// <returns>Nodes.</returns>
		static public ObservableList<TreeNode<TreeViewItem>> GenerateTreeNodes(List<int> items, string nameStartsWith = "Node ", bool isExpanded = true, Sprite icon = null)
		{
			return Enumerable.Range(1, items[0]).Select(x => {
				var item_name = nameStartsWith + x;
				var item = new TreeViewItem(item_name, icon);
				var nodes = items.Count > 1
					? GenerateTreeNodes(items.GetRange(1, items.Count - 1), item_name + " - ", isExpanded)
					: null;

				return new TreeNode<TreeViewItem>(item, nodes, isExpanded);
			}).ToObservableList();
		}

		/// <summary>
		/// Reload scene.
		/// </summary>
		public virtual void ReloadScene()
		{
			#if UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
			Application.LoadLevel(Application.loadedLevel);
			#else
			UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
			#endif
		}
	}
}