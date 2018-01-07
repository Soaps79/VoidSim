﻿using UnityEngine;
using UnityEngine.EventSystems;

namespace UIWidgets
{
	/// <summary>
	/// DragSupport for ListViewCustom.
	/// </summary>
	/// <typeparam name="TListView">ListView type.</typeparam>
	/// <typeparam name="TComponent">Component type.</typeparam>
	/// <typeparam name="TItem">Item type.</typeparam>
	abstract public class ListViewCustomDragSupport<TListView,TComponent,TItem> : DragSupport<TItem>
		where TListView : ListViewCustom<TComponent,TItem>
		where TComponent : ListViewItem
	{
		/// <summary>
		/// ListViewIcons.
		/// </summary>
		[SerializeField]
		public TListView ListView;

		/// <summary>
		/// The drag info.
		/// </summary>
		[SerializeField]
		public TComponent DragInfo;

		/// <summary>
		/// DragInfo offset.
		/// </summary>
		[SerializeField]
		public Vector3 DragInfoOffset = new Vector3(-5, 5, 0);
		
		int index = 0;

		/// <summary>
		/// Delete item from ListView after drop.
		/// </summary>
		[SerializeField]
		public bool DeleteAfterDrop = true;

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
		/// Set Data, which will be passed to Drop component.
		/// </summary>
		/// <param name="eventData">Current event data.</param>
		protected override void InitDrag(PointerEventData eventData)
		{
			var component = GetComponent<TComponent>();
			Data = GetData(component);
			index = component.Index;

			ShowDragInfo();
		}

		/// <summary>
		/// Get data from specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		/// <returns>Data.</returns>
		abstract protected TItem GetData(TComponent component);

		/// <summary>
		/// Set data for DragInfo component.
		/// </summary>
		/// <param name="data">Data.</param>
		abstract protected void SetDragInfoData(TItem data);

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

			SetDragInfoData(Data);

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
			
			// remove used from current ListViewIcons.
			if (DeleteAfterDrop && success && (ListView!=null))
			{
				var first_index = ListView.DataSource.IndexOf(Data);
				var last_index = ListView.DataSource.LastIndexOf(Data);
				if (index==first_index)
				{
					ListView.DataSource.RemoveAt(index);
				}
				else if ((index+1)==last_index)
				{
					ListView.DataSource.RemoveAt(index+1);
				}
				else
				{
					ListView.DataSource.Remove(Data);
				}
			}

			base.Dropped(success);
		}
	}
}