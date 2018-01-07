using UnityEngine;
using UnityEngine.EventSystems;

namespace UIWidgets
{
	/// <summary>
	/// DropSupport for ListViewCustom.
	/// </summary>
	/// <typeparam name="TListView">ListView type.</typeparam>
	/// <typeparam name="TComponent">Component type.</typeparam>
	/// <typeparam name="TItem">Item type.</typeparam>
	public class ListViewCustomDropSupport<TListView,TComponent,TItem> : MonoBehaviour, IDropSupport<TItem>
		where TListView : ListViewCustom<TComponent,TItem>
		where TComponent : ListViewItem
	{
		TListView listView;

		/// <summary>
		/// Current ListView.
		/// </summary>
		/// <value>ListView.</value>
		public TListView ListView {
			get {
				if (listView==null)
				{
					listView = GetComponent<TListView>();
				}
				return listView;
			}
		}

		/// <summary>
		/// The drop indicator.
		/// </summary>
		[SerializeField]
		protected ListViewDropIndicator DropIndicator;

		#region IDropSupport<TItem>
		/// <summary>
		/// Determines whether this instance can receive drop with the specified data and eventData.
		/// </summary>
		/// <returns><c>true</c> if this instance can receive drop with the specified data and eventData; otherwise, <c>false</c>.</returns>
		/// <param name="data">Data.</param>
		/// <param name="eventData">Event data.</param>
		public bool CanReceiveDrop(TItem data, PointerEventData eventData)
		{
			var index = ListView.GetNearestIndex(eventData);

			ShowDropIndicator(index);

			return true;
		}

		/// <summary>
		/// Handle dropped data.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="eventData">Event data.</param>
		public void Drop(TItem data, PointerEventData eventData)
		{
			var index = ListView.GetNearestIndex(eventData);

			AddItem(data, index);

			HideDropIndicator();
		}

		/// <summary>
		/// Handle canceled drop.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="eventData">Event data.</param>
		public void DropCanceled(TItem data, PointerEventData eventData)
		{
			HideDropIndicator();
		}
		#endregion

		/// <summary>
		/// Shows the drop indicator.
		/// </summary>
		/// <param name="index">Index.</param>
		protected virtual void ShowDropIndicator(int index)
		{
			if (DropIndicator!=null)
			{
				DropIndicator.Show(index, ListView);
			}
		}

		/// <summary>
		/// Hides the drop indicator.
		/// </summary>
		protected virtual void HideDropIndicator()
		{
			if (DropIndicator!=null)
			{
				DropIndicator.Hide();
			}
		}

		/// <summary>
		/// Add item to ListViewIcons.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <param name="index">Index.</param>
		protected virtual void AddItem(TItem item, int index)
		{
			if (index > ListView.DataSource.Count)
			{
				index = ListView.DataSource.Count;
			}
			if (index==-1)
			{
				ListView.DataSource.Add(item);
			}
			else
			{
				ListView.DataSource.Insert(index, item);
			}
		}
	}
}