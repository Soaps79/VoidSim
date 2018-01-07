using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.Linq;

namespace UIWidgets
{
	/// <summary>
	/// Base class for custom ListView with dynamic items widths.
	/// </summary>
	public class ListViewCustomWidth<TComponent,TItem> : ListViewCustom<TComponent,TItem>
		where TComponent : ListViewItem
		where TItem: IItemWidth
	{
		/// <summary>
		/// Calculate width automaticly without using IListViewItemWidth.Width.
		/// </summary>
		[SerializeField]
		[Tooltip("Calculate width automaticly without using IListViewItemWidth.Width.")]
		bool ForceAutoWidthCalculation = true;

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

		bool IsCanCalculateWidth;

		/// <summary>
		/// Constructor.
		/// </summary>
		public ListViewCustomWidth()
		{
			IsCanCalculateWidth = typeof(IListViewItemWidth).IsAssignableFrom(typeof(TComponent));
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
			SetItemsWidth(DataSource, false);
			CalculateMaxVisibleItems(DataSource);
		}

		/// <summary>
		/// Calculates the max count of visible items for specified items.
		/// </summary>
		/// <param name="items">Items.</param>
		protected virtual void CalculateMaxVisibleItems(ObservableList<TItem> items)
		{
			var width = GetScrollSize();
			maxVisibleItems = items.OrderBy<TItem,float>(GetItemWidth).TakeWhile(x => {
				width -= x.Width;
				return width > 0;
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
			return GetItemsWidth(topHiddenItems + visibleItems, bottomHiddenItems);
		}

		/// <summary>
		/// Calculates the size of the top filler.
		/// </summary>
		/// <returns>The top filler size.</returns>
		protected override float CalculateTopFillerSize()
		{
			return GetItemsWidth(0, topHiddenItems);
		}

		float GetItemsWidth(int start, int count)
		{
			if (count==0)
			{
				return 0f;
			}

			var width = DataSource.GetRange(start, Mathf.Min(count, DataSource.Count - start)).SumFloat(GetItemWidth);

			return Mathf.Max(0, width + (LayoutBridge.GetSpacing() * (count - 1)));
		}

		float GetItemWidth(TItem item)
		{
			return item.Width;
		}

		/// <summary>
		/// Gets the item position.
		/// </summary>
		/// <returns>The item position.</returns>
		/// <param name="index">Index.</param>
		public override float GetItemPosition(int index)
		{
			var height = DataSource.GetRange(0, Mathf.Min(index, DataSource.Count)).SumFloat(GetItemWidth);
			return height + (LayoutBridge.GetSpacing() * index);
		}

		/// <summary>
		/// Gets the item position bottom.
		/// </summary>
		/// <returns>The item position bottom.</returns>
		/// <param name="index">Index.</param>
		public override float GetItemPositionBottom(int index)
		{
			return GetItemPosition(index) + DataSource[index].Width - LayoutBridge.GetSpacing() + LayoutBridge.GetMargin() - GetScrollSize();
		}

		/// <summary>
		/// Gets the item middle position by index.
		/// </summary>
		/// <returns>The item middle position.</returns>
		/// <param name="index">Index.</param>
		public override float GetItemPositionMiddle(int index)
		{
			return GetItemPosition(index) - (GetScrollSize() / 2) + (DataSource[index].Width / 2);
		}

		/// <summary>
		/// Add the specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <returns>Index of added item.</returns>
		public override int Add(TItem item)
		{
			if (item.Width==0)
			{
				item.Width = CalculateItemWidth(item);
			}

			return base.Add(item);
		}

		/// <summary>
		/// Calculate and sets the width of the items.
		/// </summary>
		/// <param name="items">Items.</param>
		/// <param name="forceUpdate">If set to <c>true</c> force update.</param>
		void SetItemsWidth(ObservableList<TItem> items, bool forceUpdate = true)
		{
			items.ForEach(x => {
				if ((x.Width==0) || forceUpdate)
				{
					x.Width = CalculateItemWidth(x);
				}
			});
		}

		/// <summary>
		/// Resize this instance.
		/// </summary>
		public override void Resize()
		{
			SetItemsWidth(DataSource, true);

			base.Resize();
		}

		/// <summary>
		/// Updates the items.
		/// </summary>
		/// <param name="newItems">New items.</param>
		/// <param name="updateView">Update view.</param>
		protected override void SetNewItems(ObservableList<TItem> newItems, bool updateView=true)
		{
			//SetItemsWidth(newItems);
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
					SetItemsWidth(DataSource);
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
		/// Gets the width of the index by.
		/// </summary>
		/// <returns>The index by width.</returns>
		/// <param name="width">Width.</param>
		int GetIndexByWidth(float width)
		{
			var spacing = LayoutBridge.GetSpacing();
			return DataSource.TakeWhile((item, index) => {
				width -= item.Width;
				if (index > 0)
				{
					width -= spacing;
				}
				return width >= 0;
			}).Count();
		}

		/// <summary>
		/// Gets the last index of the visible.
		/// </summary>
		/// <returns>The last visible index.</returns>
		/// <param name="strict">If set to <c>true</c> strict.</param>
		protected override int GetLastVisibleIndex(bool strict=false)
		{
			var last_visible_index = GetIndexByWidth(GetScrollValue() + GetScrollSize());

			return (strict) ? last_visible_index : last_visible_index + 2;
		}

		/// <summary>
		/// Gets the first index of the visible.
		/// </summary>
		/// <returns>The first visible index.</returns>
		/// <param name="strict">If set to <c>true</c> strict.</param>
		protected override int GetFirstVisibleIndex(bool strict=false)
		{
			var first_visible_index = GetIndexByWidth(GetScrollValue());

			if (strict)
			{
				return first_visible_index;
			}
			return Mathf.Min(first_visible_index, Mathf.Max(0, DataSource.Count - visibleItems));
		}
		
		LayoutGroup defaultItemLayoutGroup;

		/// <summary>
		/// Gets the width of the item.
		/// </summary>
		/// <returns>The item width.</returns>
		/// <param name="item">Item.</param>
		float CalculateItemWidth(TItem item)
		{
			if (defaultItemLayoutGroup==null)
			{
				defaultItemLayoutGroup = DefaultItemCopy.GetComponent<LayoutGroup>();
			}

			float width = 0f;
			if (!IsCanCalculateWidth || ForceAutoWidthCalculation)
			{
				if (defaultItemLayoutGroup!=null)
				{
					DefaultItemCopy.gameObject.SetActive(true);

					SetData(DefaultItemCopy, item);

					var lg = DefaultItemCopy.GetComponentsInChildren<LayoutGroup>();
					Array.Reverse(lg);
					lg.ForEach(LayoutUtilites.UpdateLayout);

					LayoutUtilites.UpdateLayout(defaultItemLayoutGroup);

					width = LayoutUtility.GetPreferredWidth(DefaultItemCopyRect);

					DefaultItemCopy.gameObject.SetActive(false);
				}
			}
			else
			{
				SetData(DefaultItemCopy, item);

				width = (DefaultItemCopy as IListViewItemWidth).Width;
			}

			return width;
		}

		/// <summary>
		/// Adds the callback.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <param name="index">Index.</param>
		protected override void AddCallback(ListViewItem item, int index)
		{
			item.onResize.AddListener(SizeChanged);
			base.AddCallback(item, index);
		}

		/// <summary>
		/// Removes the callback.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <param name="index">Index.</param>
		protected override void RemoveCallback(ListViewItem item, int index)
		{
			item.onResize.RemoveListener(SizeChanged);
			base.RemoveCallback(item, index);
		}

		void SizeChanged(int index, Vector2 size)
		{
			if (DataSource[index].Width!=size.x)
			{
				DataSource[index].Width = size.x;

				var old = maxVisibleItems;
				CalculateMaxVisibleItems(DataSource);
				if (maxVisibleItems > old)
				{
					UpdateView();
				}
				else
				{
					//ScrollUpdate();
				}
			}
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
			var index = GetIndexByWidth(-point.x);
			if (index!=(DataSource.Count - 1))
			{
				var width = GetItemsWidth(0, index);
				var top = -point.x - width;
				var bottom = -point.x - (width + DataSource[index+1].Width + LayoutBridge.GetSpacing());
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
			return GetIndexByWidth(GetScrollValue());
		}
		#endregion

		#if UNITY_EDITOR
		bool IsItemCanCalculateWidth()
		{
			return IsCanCalculateWidth;
		}
		#endif
	}
}