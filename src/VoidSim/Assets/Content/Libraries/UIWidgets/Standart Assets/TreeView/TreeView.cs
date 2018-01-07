using UnityEngine;
using UnityEngine.Events;
using System;

namespace UIWidgets
{
	/// <summary>
	/// TreeViewNode event.
	/// </summary>
	[Serializable]
	public class TreeViewNodeEvent : UnityEvent<TreeNode<TreeViewItem>>
	{
	}

	/// <summary>
	/// TreeView.
	/// </summary>
	[AddComponentMenu("UI/UIWidgets/TreeView")]
	public class TreeView : TreeViewCustom<TreeViewComponent,TreeViewItem>
	{
		/// <summary>
		/// NodeToggleProxy event.
		/// </summary>
		public TreeViewNodeEvent NodeToggleProxy = new TreeViewNodeEvent();

		bool isInited = false;

		/// <summary>
		/// Init this instance.
		/// </summary>
		public override void Init()
		{
			if (isInited)
			{
				return ;
			}
			isInited = true;

			base.Init();

			NodeToggle.AddListener(NodeToggleProxy.Invoke);
		}
	}
}