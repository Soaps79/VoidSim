﻿using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace UIWidgets
{
	/// <summary>
	/// TreeViewCustomNode drag support.
	/// </summary>
	public class TreeViewCustomNodeDragSupport<TTreeViewComponent,TListViewComponent,TItem> : DragSupport<TreeNode<TItem>>
		where TTreeViewComponent : TreeViewComponentBase<TItem>
		where TListViewComponent : MonoBehaviour, IViewData<TItem>
	{
		/// <summary>
		/// Component to display draggable info.
		/// </summary>
		[SerializeField]
		public TListViewComponent DragInfo;

		/// <summary>
		/// DragInfo offset.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("LocalPosition")]
		public Vector3 DragInfoOffset = new Vector3(-5, 5, 0);

		/// <summary>
		/// Start this instance.
		/// </summary>
		protected virtual void Start()
		{
			if (DragInfo!=null)
			{
				DragInfo.gameObject.SetActive(false);
			}
		}

		/// <summary>
		/// Draggable node.
		/// </summary>
		protected virtual TreeNode<TItem> Node {
			get {
				return GetComponent<TTreeViewComponent>().Node;
			}
		}

		/// <summary>
		/// Set Data, which will be passed to Drop component.
		/// </summary>
		/// <param name="eventData">Current event data.</param>
		protected override void InitDrag(PointerEventData eventData)
		{
			Data = Node;

			ShowDragInfo();
		}

		/// <summary>
		/// Shows the drag info.
		/// </summary>
		protected virtual void ShowDragInfo()
		{
			if (DragInfo==null)
			{
				return ;
			}
			DragInfo.transform.SetParent(DragPoint, false);
			DragInfo.transform.localPosition = DragInfoOffset;

			DragInfo.SetData(Data.Item);

			DragInfo.gameObject.SetActive(true);
		}

		/// <summary>
		/// Hides the drag info.
		/// </summary>
		protected virtual void HideDragInfo()
		{
			if (DragInfo==null)
			{
				return ;
			}
			DragInfo.gameObject.SetActive(false);
		}

		/// <summary>
		/// Called when drop completed.
		/// </summary>
		/// <param name="success"><c>true</c> if Drop component received data; otherwise, <c>false</c>.</param>
		public override void Dropped(bool success)
		{
			HideDragInfo();
			
			base.Dropped(success);
		}
	}
}