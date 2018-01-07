using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;
using System.ComponentModel;

namespace UIWidgets
{
	/// <summary>
	/// Tree node.
	/// </summary>
	[Serializable]
	public class TreeNode<TItem> : IObservable, INotifyPropertyChanged
	{
		/// <summary>
		/// Occurs when on change.
		/// </summary>
		public event OnChange OnChange = () => { };

		/// <summary>
		/// Occurs when a property value changes.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged = (x, y) => { };

		/// <summary>
		/// The pause observation.
		/// </summary>
		public bool PauseObservation;

		[SerializeField]
		bool isVisible = true;

		/// <summary>
		/// Is node visible?
		/// </summary>
		/// <value><c>true</c> if this node is visible; otherwise, <c>false</c>.</value>
		public bool IsVisible {
			get {
				return isVisible;
			}
			set {
				if (isVisible!=value)
				{
					isVisible = value;
					Changed("IsVisible");
				}
			}
		}

		[SerializeField]
		bool isExpanded;

		/// <summary>
		/// Is node expanded?
		/// </summary>
		/// <value><c>true</c> if this node is expanded; otherwise, <c>false</c>.</value>
		public bool IsExpanded {
			get {
				return isExpanded;
			}
			set {
				if (isExpanded!=value)
				{
					isExpanded = value;
					Changed("IsExpanded");
				}
			}
		}
		
		[SerializeField]
		TItem item;

		/// <summary>
		/// Gets or sets the item.
		/// </summary>
		/// <value>The item.</value>
		public TItem Item {
			get {
				return item;
			}
			set {
				item = value;
				Changed("Item");
			}
		}
		
		[SerializeField]
		ObservableList<TreeNode<TItem>> nodes;

		/// <summary>
		/// Gets or sets the nodes.
		/// </summary>
		/// <value>The nodes.</value>
		public ObservableList<TreeNode<TItem>> Nodes {
			get {
				return nodes;
			}
			set {
				if (nodes!=null)
				{
					nodes.OnChange -= Changed;
					nodes.OnCollectionChange -= CollectionChanged;
				}
				nodes = value;
				if (nodes!=null)
				{
					nodes.OnChange += Changed;
					nodes.OnCollectionChange += CollectionChanged;
					CollectionChanged();
				}
				Changed("Nodes");
			}
		}

		/// <summary>
		/// Gets the total nodes count.
		/// </summary>
		/// <value>The total nodes count.</value>
		public int TotalNodesCount {
			get {
				if (nodes==null)
				{
					return 1;
				}
				return nodes.Sum(x => x.TotalNodesCount) + 1;
			}
		}

		/// <summary>
		/// The used nodes count.
		/// </summary>
		public int UsedNodesCount;

		/// <summary>
		/// Gets all used nodes count.
		/// </summary>
		/// <value>All used nodes count.</value>
		public int AllUsedNodesCount {
			get {
				if (!isVisible)
				{
					return 0;
				}
				if (!isExpanded)
				{
					return 0 + UsedNodesCount;
				}
				if (nodes==null)
				{
					return 0 + UsedNodesCount;
				}
				return nodes.Sum(x => x.AllUsedNodesCount) + UsedNodesCount;
			}
		}

		WeakReference parent;

		/// <summary>
		/// Gets or sets the parent.
		/// </summary>
		/// <value>The parent.</value>
		public TreeNode<TItem> Parent {
			get {
				if ((parent!=null) && (parent.IsAlive))
				{
					return parent.Target as TreeNode<TItem>;
				}
				return null;
			}
			set {
				SetParentValue(value);
			}
		}

		/// <summary>
		/// Gets the all parent node, current parent node in first.
		/// </summary>
		/// <value>The path.</value>
		public List<TreeNode<TItem>> Path {
			get {
				var result = new List<TreeNode<TItem>>();
				var current_parent = Parent;

				while (current_parent!=null)
				{
					result.Add(current_parent);
					current_parent = current_parent.Parent;
				}
				var last = result.Count - 1;
				if ((last>=0) && (result[last].Item==null))
				{
					result.RemoveAt(last);
				}
				return result;
			}
		}

		/// <summary>
		/// Gets the root node. Use object.ReferenceEquals to check if nodes belong to same tree.
		/// </summary>
		/// <value>The root node.</value>
		public TreeNode<TItem> RootNode {
			get {
				var current_parent = Parent;

				while (current_parent!=null)
				{
					if (current_parent.Parent==null)
					{
						break;
					}
					current_parent = current_parent.Parent;
				}
				return current_parent;
			}
		}

		/// <summary>
		/// The index in TreeView, used for optimization purpose.
		/// </summary>
		public int Index = -1;

		/// <summary>
		/// Determines whether this instance is parent of node the specified node.
		/// </summary>
		/// <returns><c>true</c> if this instance is parent of node the specified node; otherwise, <c>false</c>.</returns>
		/// <param name="node">Node.</param>
		public bool IsParentOfNode(TreeNode<TItem> node)
		{
			var nodeParent = node.Parent;
			while (nodeParent != null)
			{
				if (nodeParent==this)
				{
					return true;
				}
				nodeParent = nodeParent.Parent;
			}

			return false;
		}

		/// <summary>
		/// Determines whether this instance can be child of the specified newParent.
		/// </summary>
		/// <returns><c>true</c> if this instance can be child of the specified newParent; otherwise, <c>false</c>.</returns>
		/// <param name="newParent">New parent.</param>
		public bool CanBeParent(TreeNode<TItem> newParent)
		{
			if (this==newParent)
			{
				return false;
			}
			return !IsParentOfNode(newParent);
		}

		void SetParentValue(TreeNode<TItem> newParent)
		{
			var oldParent = ((parent!=null) && (parent.IsAlive)) ? parent.Target as TreeNode<TItem> : null;

			if (oldParent==newParent)
			{
				return ;
			}

			if (newParent!=null)
			{
				if (newParent==this)
				{
					throw new ArgumentException("Node cannot be own parent.");
				}
				if (IsParentOfNode(newParent))
				{
					throw new ArgumentException("Own child node cannot be parent node.");
				}
			}

			if (oldParent!=null)
			{
				oldParent.nodes.OnCollectionChange -= oldParent.CollectionChanged;
				oldParent.nodes.Remove(this);
				oldParent.nodes.OnCollectionChange += oldParent.CollectionChanged;
			}

			parent = new WeakReference(newParent);

			if (newParent!=null)
			{
				if (newParent.nodes==null)
				{
					newParent.nodes = new ObservableList<TreeNode<TItem>>();

					newParent.nodes.OnChange += newParent.Changed;
					newParent.nodes.OnCollectionChange += newParent.CollectionChanged;
				}

				newParent.nodes.OnCollectionChange -= newParent.CollectionChanged;
				newParent.nodes.Add(this);
				newParent.nodes.OnCollectionChange += newParent.CollectionChanged;
			}
			//Changed();
		}

		/// <summary>
		/// Initializes a new instance of the class.
		/// </summary>
		/// <param name="nodeItem">Node item.</param>
		/// <param name="nodeNodes">Node nodes.</param>
		/// <param name="nodeIsExpanded">If set to <c>true</c> node is expanded.</param>
		/// <param name="nodeIsVisible">If set to <c>true</c> node is visible.</param>
		public TreeNode(TItem nodeItem,
			ObservableList<TreeNode<TItem>> nodeNodes = null,
			bool nodeIsExpanded = false,
			bool nodeIsVisible = true)
		{
			item = nodeItem;
			nodes = nodeNodes;

			isExpanded = nodeIsExpanded;
			isVisible = nodeIsVisible;

			if (nodes!=null)
			{
				nodes.OnChange += Changed;
				nodes.OnCollectionChange += CollectionChanged;
				CollectionChanged();
			}
		}

		/// <summary>
		/// Initializes a new instance of the class.
		/// </summary>
		/// <param name="node">Node.</param>
		/// <param name="serializedNodes">List of serialized nodes.</param>
		public TreeNode(TreeNodeSerialized<TItem> node, List<TreeNodeSerialized<TItem>> serializedNodes)
		{
			PauseObservation = node.PauseObservation;
			isVisible = node.IsVisible;
			isExpanded = node.IsExpanded;
			item = node.Item;

			nodes = new ObservableList<TreeNode<TItem>>();
			for (int i = 0; i < node.SubNodesCount; i++)
			{
				nodes.Add(new TreeNode<TItem>(serializedNodes[node.FirstSubNodeIndex + i], serializedNodes));
			}

			nodes.OnChange += Changed;
			nodes.OnCollectionChange += CollectionChanged;
		}

		void CollectionChanged()
		{
			if (nodes==null)
			{
				return ;
			}
			nodes.ForEach(SetParent);
		}

		void SetParent(TreeNode<TItem> node)
		{
			if ((node.Parent!=null) && (node.Parent!=this))
			{
				node.Parent.nodes.Remove(node);
			}
			node.parent = new WeakReference(this);
		}

		void Changed()
		{
			Changed("Nodes");
		}

		void Changed(string propertyName)
		{
			if (PauseObservation)
			{
				return ;
			}
			OnChange();
			PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		/// <summary>
		/// Remove this node from tree.
		/// </summary>
		public void RemoveFromTree()
		{
			Parent = null;
		}

		/// <summary>
		/// Determines whether the specified object is equal to the current object.
		/// </summary>
		/// <param name="obj">The object to compare with the current object.</param>
		/// <returns><c>true</c> if the specified object is equal to the current object; otherwise, <c>false</c>.</returns>
		public override bool Equals(System.Object obj)
		{
			var nodeObj = obj as TreeNode<TItem>;
			if (nodeObj==null)
			{
				return this==null;
			}
			if (this==null)
			{
				return false;
			}
			return item.Equals(nodeObj.item);
		}

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		/// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a hash table.</returns>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		/// <summary>
		/// Returns true if the nodes items are equal, false otherwise.
		/// </summary>
		/// <param name="a">The alpha component.</param>
		/// <param name="b">The blue component.</param>
		public static bool operator ==(TreeNode<TItem> a, TreeNode<TItem> b)
		{
			var a_null = object.ReferenceEquals(null, a);
			var b_null = object.ReferenceEquals(null, b);
			if (a_null && b_null)
			{
				return true;
			}
			if (a_null!=b_null)
			{
				return false;
			}

			var a_item_null = object.ReferenceEquals(null, a.item);
			var b_item_null = object.ReferenceEquals(null, b.item);
			if (a_item_null && b_item_null)
			{
				return true;
			}
			if (a_item_null!=b_item_null)
			{
				return false;
			}

			return a.item.Equals(b.item);
		}

		/// <summary>
		/// Returns true if the nodes items are not equal, false otherwise.
		/// </summary>
		/// <param name="a">The alpha component.</param>
		/// <param name="b">The blue component.</param>
		public static bool operator !=(TreeNode<TItem> a, TreeNode<TItem> b)
		{
			var a_null = object.ReferenceEquals(null, a);
			var b_null = object.ReferenceEquals(null, b);
			if (a_null && b_null)
			{
				return false;
			}
			if (a_null!=b_null)
			{
				return true;
			}

			var a_item_null = object.ReferenceEquals(null, a.item);
			var b_item_null = object.ReferenceEquals(null, b.item);
			if (a_item_null && b_item_null)
			{
				return false;
			}
			if (a_item_null!=b_item_null)
			{
				return true;
			}

			return !a.item.Equals(b.item);
		}

		/// <summary>
		/// Serialize the specified nodes.
		/// </summary>
		/// <param name="nodes">Nodes.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
		static public List<TreeNodeSerialized<TItem>> Serialize(ObservableList<TreeNode<TItem>> nodes)
		{
			if (nodes==null)
			{
				return null;
			}

			var serializedNodes = new List<TreeNodeSerialized<TItem>>();
			Serialize(nodes, serializedNodes, 0);

			return serializedNodes;
		}

		/// <summary>
		/// Serialize the specified nodes.
		/// </summary>
		/// <param name="nodes">Nodes.</param>
		/// <param name="serializedNodes">List of serialized nodes.</param>
		/// <param name="depth">Depth.</param>
		static protected List<TreeNodeSerialized<TItem>> Serialize(ObservableList<TreeNode<TItem>> nodes, List<TreeNodeSerialized<TItem>> serializedNodes, int depth)
		{
			foreach (var node in nodes)
			{
				serializedNodes.Add(new TreeNodeSerialized<TItem>(node, serializedNodes.Count + 1, depth));
				if (node.Nodes!=null)
				{
					Serialize(node.Nodes, serializedNodes, depth + 1);
				}
			}
			return serializedNodes;
		}

		/// <summary>
		/// Deserialize the specified nodes.
		/// </summary>
		/// <param name="serializedNodes">Serialized nodes.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
		static public ObservableList<TreeNode<TItem>> Deserialize(List<TreeNodeSerialized<TItem>> serializedNodes)
		{
			if (serializedNodes==null)
			{
				return null;
			}

			var nodes = new ObservableList<TreeNode<TItem>>();
			foreach (var serialized_node in serializedNodes)
			{
				if (serialized_node.Depth==0)
				{
					nodes.Add(new TreeNode<TItem>(serialized_node, serializedNodes));
				}
			}

			return nodes;
		}
	}

	/// <summary>
	/// Serialized TreeNode.
	/// </summary>
	[Serializable]
	public class TreeNodeSerialized<TItem>
	{
		/// <summary>
		/// The pause observation.
		/// </summary>
		[SerializeField]
		public bool PauseObservation;

		/// <summary>
		/// Is node visible?
		/// </summary>
		[SerializeField]
		public bool IsVisible;

		/// <summary>
		/// Is node expanded?
		/// </summary>
		[SerializeField]
		public bool IsExpanded;

		/// <summary>
		/// The item.
		/// </summary>
		[SerializeField]
		public TItem Item;

		/// <summary>
		/// The sub nodes count.
		/// </summary>
		public int SubNodesCount;

		/// <summary>
		/// The index of the first sub node.
		/// </summary>
		public int FirstSubNodeIndex;

		/// <summary>
		/// The depth.
		/// </summary>
		public int Depth;

		/// <summary>
		/// Initializes a new instance of the class.
		/// </summary>
		/// <param name="node">Node.</param>
		/// <param name="firstSubNodeIndex">Index of first subnode.</param>
		/// <param name="depth">Node depth.</param>
		public TreeNodeSerialized(TreeNode<TItem> node, int firstSubNodeIndex, int depth)
		{
			PauseObservation = node.PauseObservation;
			IsVisible = node.IsVisible;
			IsExpanded = node.IsExpanded;
			Item = node.Item;

			SubNodesCount = node.Nodes==null ? 0 : node.Nodes.Count;
			FirstSubNodeIndex = firstSubNodeIndex;
			Depth = depth;
		}
	}
}