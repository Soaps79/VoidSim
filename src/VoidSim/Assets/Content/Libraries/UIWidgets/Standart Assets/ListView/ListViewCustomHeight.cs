using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Linq;

namespace UIWidgets
{
	/// <summary>
	/// Base class for custom ListView with dynamic items heights.
	/// </summary>
	public class ListViewCustomHeight<TComponent,TItem> : ListViewCustom<TComponent,TItem>
		where TComponent : ListViewItem
		where TItem: IItemHeight
	{
		/// <summary>
		/// Calculate height automaticly without using IListViewItemHeight.Height.
		/// </summary>
		[SerializeField]
		[Tooltip("Calculate height automaticly without using IListViewItemHeight.Height.")]
		bool ForceAutoHeightCalculation = true;

		TComponent defaultItemCopy;
		RectTransform defaultItemCopyRect;

		/// <summary>
		/// Gets the default item copy.
		/// </summary>
		/// <value>The default item copy.</value>
		protected TComponent DefaultItemCopy {
			get {
				if (defaultItemCopy==null)
				{
					defaultItemCopy = Instantiate(DefaultItem) as TComponent;
					defaultItemCopy.transform.SetParent(DefaultItem.transform.parent, false);
					defaultItemCopy.gameObject.name = "DefaultItemCopy";
					defaultItemCopy.gameObject.SetActive(false);

					Utilites.FixInstantiated(DefaultItem, defaultItemCopy);
				}
				return defaultItemCopy;
			}
		}

		/// <summary>
		/// Gets the RectTransform of DefaultItemCopy.
		/// </summary>
		/// <value>RectTransform.</value>
		protected RectTransform DefaultItemCopyRect {
			get {
				if (defaultItemCopyRect==null)
				{
					defaultItemCopyRect = defaultItemCopy.transform as RectTransform;
				}
				return defaultItemCopyRect;
			}
		}

		bool IsCanCalculateHeight;

		/// <summary>
		/// Constructor.
		/// </summary>
		public ListViewCustomHeight()
		{
			IsCanCalculateHeight = typeof(IListViewItemHeight).IsAssignableFrom(typeof(TComponent));
		}

		/// <summary>
		/// Sets the default item.
		/// </summary>
		/// <param name="newDefaultItem">New default item.</param>
		protected override void SetDefaultItem(TComponent newDefaultItem)
		{
			if (newDefaultItem!=null)
			{
				if (defaultItemCopy!=null)
				{
					Destroy(defaultItemCopy.gameObject);
					defaultItemCopy = null;
				}
				if (defaultItemCopyRect!=null)
				{
					Destroy(defaultItemCopyRect.gameObject);
					defaultItemCopyRect = null;
				}
			}
			base.SetDefaultItem(newDefaultItem);
		}

		/// <summary>
		/// Calculates the max count of visible items.
		/// </summary>
		protected override void CalculateMaxVisibleItems()
		{
			SetItemsHeight(DataSource, false);
			CalculateMaxVisibleItems(DataSource);
		}

		/// <summary>
		/// Calculates the max count of visible items for specified items.
		/// </summary>
		protected virtual void CalculateMaxVisibleItems(ObservableList<TItem> items)
		{
			var height = GetScrollSize();
			maxVisibleItems = items.OrderBy<TItem,float>(GetItemHeight).TakeWhile(x => {
				height -= x.Height;
				return height > 0;
			}).Count() + 2;
		}

		/// <summary>
		/// Calculates the size of the item.
		/// <param name="reset">Reset item size.</param>
		/// </summary>
		protected override void CalculateItemSize(bool reset=false)
		{
			var rect = DefaultItem.transform as RectTransform;
			#if UNITY_4_6 || UNITY_4_7
			var layout_elements = rect.GetComponents<Component>().OfType<ILayoutElement>();
			#else
			var layout_elements = rect.GetComponents<ILayoutElement>();
			#endif
			if ((itemHeight==0) || reset)
			{
				var preffered_height = layout_elements.Max(x => Mathf.Max(x.minHeight, x.preferredHeight));
				itemHeight = (preffered_height > 0) ? preffered_height : rect.rect.height;
			}
			if ((itemWidth==0) || reset)
			{
				var preffered_width = layout_elements.Max(x => Mathf.Max(x.minWidth, x.preferredWidth));
				itemWidth = (preffered_width > 0) ? preffered_width : rect.rect.width;
			}
		}
		
		/// <summary>
		/// Scrolls to item with specifid index.
		/// </summary>
		/// <param name="index">Index.</param>
		public override void ScrollTo(int index)
		{
			if (!CanOptimize())
			{
				return ;
			}

			var top = GetScrollValue();
			var bottom = GetScrollValue() + GetScrollSize();

			var item_starts = GetItemPosition(index);
			var item_ends = GetItemPositionBottom(index);

			if (item_starts < top)
			{
				SetScrollValue(item_starts);
			}
			else if (item_ends > bottom)
			{
				SetScrollValue(item_ends);
			}
		}

		/// <summary>
		/// Calculates the size of the bottom filler.
		/// </summary>
		/// <returns>The bottom filler size.</returns>
		protected override float CalculateBottomFillerSize()
		{
			return GetItemsHeight(topHiddenItems + visibleItems, bottomHiddenItems);
		}

		/// <summary>
		/// Calculates the size of the top filler.
		/// </summary>
		/// <returns>The top filler size.</returns>
		protected override float CalculateTopFillerSize()
		{
			return GetItemsHeight(0, topHiddenItems);
		}

		float GetItemsHeight(int start, int count)
		{
			if (count==0)
			{
				return 0f;
			}

			var height = DataSource.GetRange(start, Mathf.Min(count, DataSource.Count - start)).SumFloat(GetItemHeight);

			return Mathf.Max(0, height + (LayoutBridge.GetSpacing() * (count - 1)));
		}

		float GetItemHeight(TItem item)
		{
			return item.Height;
		}

		/// <summary>
		/// Gets the item position.
		/// </summary>
		/// <returns>The item position.</returns>
		/// <param name="index">Index.</param>
		public override float GetItemPosition(int index)
		{
			var height = DataSource.GetRange(0, Mathf.Min(index, DataSource.Count)).SumFloat(GetItemHeight);
			return height + (LayoutBridge.GetSpacing() * index);
		}

		/// <summary>
		/// Gets the item position bottom.
		/// </summary>
		/// <returns>The item position bottom.</returns>
		/// <param name="index">Index.</param>
		public override float GetItemPositionBottom(int index)
		{
			return GetItemPosition(index) + DataSource[index].Height - LayoutBridge.GetSpacing() + LayoutBridge.GetMargin() - GetScrollSize();
		}

		/// <summary>
		/// Gets the item middle position by index.
		/// </summary>
		/// <returns>The item middle position.</returns>
		/// <param name="index">Index.</param>
		public override float GetItemPositionMiddle(int index)
		{
			return GetItemPosition(index) - (GetScrollSize() / 2) + (DataSource[index].Height / 2);
		}

		/// <summary>
		/// Calculate and sets the height of the items.
		/// </summary>
		/// <param name="items">Items.</param>
		/// <param name="forceUpdate">If set to <c>true</c> force update.</param>
		void SetItemsHeight(ObservableList<TItem> items, bool forceUpdate = true)
		{
			if (defaultItemLayoutGroup==null)
			{
				DefaultItemCopy.gameObject.SetActive(true);

				defaultItemLayoutGroup = DefaultItemCopy.GetComponent<LayoutGroup>();
			}

			DefaultItemCopy.gameObject.SetActive(true);

			items.ForEach(x => {
				if ((x.Height==0) || forceUpdate)
				{
					x.Height = CalculateItemHeight(x);
				}
			});

			DefaultItemCopy.gameObject.SetActive(false);
		}

		/// <summary>
		/// Resize this instance.
		/// </summary>
		public override void Resize()
		{
			SetItemsHeight(DataSource, true);

			base.Resize();
		}

		/// <summary>
		/// Updates the items.
		/// </summary>
		/// <param name="newItems">New items.</param>
		/// <param name="updateView">Update view.</param>
		protected override void SetNewItems(ObservableList<TItem> newItems, bool updateView=true)
		{
			SetItemsHeight(newItems);
			//CalculateMaxVisibleItems(newItems);

			base.SetNewItems(newItems, updateView);
		}

		/// <summary>
		/// Update this instance.
		/// </summary>
		protected override void Update()
		{
			if (DataSourceSetted || DataSourceChanged)
			{
				var reset_scroll = DataSourceSetted;

				DataSourceSetted = false;
				DataSourceChanged = false;

				lock (DataSource)
				{
					SetItemsHeight(DataSource);
					CalculateMaxVisibleItems(DataSource);

					UpdateView();
				}
				if (reset_scroll)
				{
					SetScrollValue(0f);
				}
			}

			if (NeedResize)
			{
				Resize();
			}

			if (IsStopScrolling())
			{
				EndScrolling();
			}
		}

		/// <summary>
		/// Gets the height of the index by.
		/// </summary>
		/// <returns>The index by height.</returns>
		/// <param name="height">Height.</param>
		int GetIndexByHeight(float height)
		{
			var spacing = LayoutBridge.GetSpacing();
			return DataSource.TakeWhile((item, index) => {
				height -= item.Height;
				if (index > 0)
				{
					height -= spacing;
				}
				return height >= 0;
			}).Count();
		}

		/// <summary>
		/// Gets the last index of the visible.
		/// </summary>
		/// <returns>The last visible index.</returns>
		/// <param name="strict">If set to <c>true</c> strict.</param>
		protected override int GetLastVisibleIndex(bool strict=false)
		{
			var last_visible_index = GetIndexByHeight(GetScrollValue() + GetScrollSize());

			return (strict) ? last_visible_index : last_visible_index + 2;
		}

		/// <summary>
		/// Gets the first index of the visible.
		/// </summary>
		/// <returns>The first visible index.</returns>
		/// <param name="strict">If set to <c>true</c> strict.</param>
		protected override int GetFirstVisibleIndex(bool strict=false)
		{
			var first_visible_index = GetIndexByHeight(GetScrollValue());

			if (strict)
			{
				return first_visible_index;
			}
			return Mathf.Min(first_visible_index, Mathf.Max(0, DataSource.Count - visibleItems));
		}

		LayoutGroup defaultItemLayoutGroup;

		/// <summary>
		/// Gets the height of the item.
		/// </summary>
		/// <returns>The item height.</returns>
		/// <param name="item">Item.</param>
		float CalculateItemHeight(TItem item)
		{
			if (defaultItemLayoutGroup==null)
			{
				DefaultItemCopy.gameObject.SetActive(true);

				defaultItemLayoutGroup = DefaultItemCopy.GetComponent<LayoutGroup>();
			}

			float height = 0f;
			if (!IsCanCalculateHeight || ForceAutoHeightCalculation)
			{
				if (defaultItemLayoutGroup!=null)
				{
					SetData(DefaultItemCopy, item);

					var lg = DefaultItemCopy.GetComponentsInChildren<LayoutGroup>();
					Array.Reverse(lg);
					lg.ForEach(LayoutUtilites.UpdateLayout);

					LayoutUtilites.UpdateLayout(defaultItemLayoutGroup);

					height = LayoutUtility.GetPreferredHeight(DefaultItemCopyRect);
				}
			}
			else
			{
				SetData(DefaultItemCopy, item);

				height = (DefaultItemCopy as IListViewItemHeight).Height;
			}

			return height;
		}

		/// <summary>
		/// Adds the callback.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <param name="index">Index.</param>
		protected override void AddCallback(ListViewItem item, int index)
		{
			item.onResize.AddListener(OnItemSizeChanged);
			base.AddCallback(item, index);
		}

		/// <summary>
		/// Removes the callback.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <param name="index">Index.</param>
		protected override void RemoveCallback(ListViewItem item, int index)
		{
			item.onResize.RemoveListener(OnItemSizeChanged);
			base.RemoveCallback(item, index);
		}

		/// <summary>
		/// Handle component size changed event.
		/// </summary>
		/// <param name="index">Item index.</param>
		/// <param name="size">New size.</param>
		protected void OnItemSizeChanged(int index, Vector2 size)
		{
			UpdateItemSize(index, size);
		}

		/// <summary>
		/// Update saved size of item.
		/// </summary>
		/// <param name="index">Item index.</param>
		/// <param name="size">New size.</param>
		/// <returns>true if size different; otherwise, false.</returns>
		protected virtual bool UpdateItemSize(int index, Vector2 size)
		{
			if (DataSource[index].Height!=size.y)
			{
				DataSource[index].Height = size.y;

				var old = maxVisibleItems;
				CalculateMaxVisibleItems(DataSource);
				if (maxVisibleItems > old)
				{
					UpdateView();
				}
				else
				{
					ScrollUpdate();
				}
				return true;
			}
			return false;
		}

		/// <summary>
		/// Calls specified function with each component.
		/// </summary>
		/// <param name="func">Func.</param>
		public override void ForEachComponent(Action<ListViewItem> func)
		{
			base.ForEachComponent(func);
			func(DefaultItemCopy);
		}

		/// <summary>
		/// Gets the index of the nearest item.
		/// </summary>
		/// <returns>The nearest item index.</returns>
		/// <param name="point">Point.</param>
		public override int GetNearestIndex(Vector2 point)
		{
			if (IsSortEnabled())
			{
				return -1;
			}
			var index = GetIndexByHeight(-point.y);
			if (index!=(DataSource.Count - 1))
			{
				var height = GetItemsHeight(0, index);
				var top = -point.y - height;
				var bottom = -point.y - (height + DataSource[index+1].Height + LayoutBridge.GetSpacing());
				if (bottom < top)
				{
					index += 1;
				}
			}

			return Mathf.Min(index, DataSource.Count - 1);
		}

		#region ListViewPaginator support
		/// <summary>
		/// Gets the index of the nearest item.
		/// </summary>
		/// <returns>The nearest item index.</returns>
		public override int GetNearestItemIndex()
		{
			return GetIndexByHeight(GetScrollValue());
		}
		#endregion

		#if UNITY_EDITOR
		bool IsItemCanCalculateHeight()
		{
			return IsCanCalculateHeight;
		}
		#endif
	}
}