using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;
using System.Linq;

namespace UIWidgets
{
	/// <summary>
	/// Base class for TileView's with items with different widths or heights.
	/// </summary>
	/// <typeparam name="TComponent">Component class.</typeparam>
	/// <typeparam name="TItem">Item class.</typeparam>
	public class TileViewCustomSize<TComponent,TItem> : ListViewCustom<TComponent,TItem>
		where TComponent : ListViewItem
	{
		/// <summary>
		/// Items sizes.
		/// </summary>
		protected readonly Dictionary<TItem,Vector2> ItemSizes = new Dictionary<TItem,Vector2>();

		/// <summary>
		/// Blocks sizes.
		/// </summary>
		protected readonly List<float> BlockSizes = new List<float>();

		/// <summary>
		/// Items per row.
		/// </summary>
		protected int ItemsPerRow;

		/// <summary>
		/// Items per column.
		/// </summary>
		protected int ItemsPerColumn;

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
		/// Calculates the max count of visible items.
		/// </summary>
		protected override void CalculateMaxVisibleItems()
		{
			CalculateItemsSizes(DataSource, false);

			var lowest_width = ItemSizes.Count > 0 ? ItemSizes.Values.Min(x => x.x) : 1;
			var lowest_height = ItemSizes.Count > 0 ? ItemSizes.Values.Min(x => x.y) : 1;

			var spacing_x = GetItemSpacingX();
			var spacing_y = GetItemSpacingY();

			var height = scrollHeight + spacing_y - LayoutBridge.GetFullMarginY();
			var width = scrollWidth + spacing_x - LayoutBridge.GetFullMarginX();

			if (IsHorizontal())
			{
				ItemsPerColumn = Mathf.FloorToInt(height / (lowest_height + spacing_y));
				ItemsPerColumn = Mathf.Max(1, ItemsPerColumn);
				ItemsPerColumn = LayoutBridge.ColumnsConstraint(ItemsPerColumn);

				CalculateBlockSizes(ItemsPerColumn);
				
				ItemsPerRow = RequiredBlocksCount(width) + 1;
				ItemsPerRow = Mathf.Max(2, ItemsPerRow);
			}
			else
			{
				ItemsPerRow = Mathf.FloorToInt(width / (lowest_width + spacing_x));
				ItemsPerRow = Mathf.Max(1, ItemsPerRow);
				ItemsPerRow = LayoutBridge.RowsConstraint(ItemsPerRow);

				CalculateBlockSizes(ItemsPerRow);

				ItemsPerColumn = RequiredBlocksCount(height) + 1;
				ItemsPerColumn = Mathf.Max(2, ItemsPerColumn);
			}

			maxVisibleItems = ItemsPerRow * ItemsPerColumn;
		}

		/// <summary>
		/// Get required blocks count.
		/// </summary>
		/// <param name="total">Total size.</param>
		/// <returns>Required blocks count.</returns>
		protected int RequiredBlocksCount(float total)
		{
			var spacing = LayoutBridge.GetSpacing();
			var count = BlockSizes.OrderBy(x => x).TakeWhile((size, index) => {
				total -= size;
				if (index > 0)
				{
					total -= spacing;
				}
				return total >= 0;
			}).Count() + 1;

			return count;
		}

		/// <summary>
		/// Calculate block sizes.
		/// </summary>
		/// <param name="perBlock">Per block.</param>
		protected void CalculateBlockSizes(int perBlock)
		{
			BlockSizes.Clear();
			var blocks = Mathf.CeilToInt((float)DataSource.Count / (float)perBlock);
			for (int i = 0; i < blocks; i++)
			{
				var size = GetItemSize(DataSource[i * perBlock]);
				for (int j = (i * perBlock) + 1; j < (i + 1) * perBlock; j++)
				{
					if (j==DataSource.Count)
					{
						break ;
					}
					size = Mathf.Max(size, GetItemSize(DataSource[j]));
				}
				BlockSizes.Add(size);
			}
		}

		/// <summary>
		/// Count of items the per block.
		/// </summary>
		/// <returns>The per block.</returns>
		protected int ItemsPerBlock()
		{
			return IsHorizontal() ? ItemsPerColumn : ItemsPerRow;
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
			var bottom = GetScrollValue() + scrollHeight;

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
			return GetBlocksSize(topHiddenItems + visibleItems, bottomHiddenItems);
		}

		/// <summary>
		/// Calculates the size of the top filler.
		/// </summary>
		/// <returns>The top filler size.</returns>
		protected override float CalculateTopFillerSize()
		{
			return GetBlocksSize(0, topHiddenItems);
		}

		/// <summary>
		/// Gets the blocks count.
		/// </summary>
		/// <returns>The blocks count.</returns>
		/// <param name="items">Items.</param>
		int GetBlocksCount(int items)
		{
			return Mathf.CeilToInt((float)items / (float)ItemsPerBlock());
		}

		float GetBlocksSize(int start, int count)
		{
			if (count==0)
			{
				return 0f;
			}

			var start_block = GetBlocksCount(start);
			var end_block = GetBlocksCount(start + count);
			var block_count = end_block - start_block;

			var size = BlockSizes.GetRange(start_block, block_count).SumFloat();
			size += (LayoutBridge.GetSpacing() * (block_count - 1));

			return Mathf.Max(0, size);
		}

		float GetItemSize(TItem item)
		{
			var size = ItemSizes[item];
			return IsHorizontal() ? size.x : size.y;
		}

		/// <summary>
		/// Get block index by item index.
		/// </summary>
		/// <param name="index">Item index.</param>
		/// <returns>Block index.</returns>
		protected override int GetBlockIndex(int index)
		{
			return Mathf.FloorToInt((float)index / (float)ItemsPerBlock());
		}

		/// <summary>
		/// Gets the item position.
		/// </summary>
		/// <returns>The item position.</returns>
		/// <param name="index">Index.</param>
		public override float GetItemPosition(int index)
		{
			var block = GetBlockIndex(index);

			return BlockSizes.GetRange(0, block).SumFloat() + (LayoutBridge.GetSpacing() * block);
		}

		/// <summary>
		/// Gets the item position bottom.
		/// </summary>
		/// <returns>The item position bottom.</returns>
		/// <param name="index">Index.</param>
		public override float GetItemPositionBottom(int index)
		{
			var block = Mathf.Min(GetBlockIndex(index) + 1, BlockSizes.Count);

			return BlockSizes.GetRange(0, block).SumFloat() + (LayoutBridge.GetSpacing() * (block - 1)) + LayoutBridge.GetMargin() - GetScrollSize();
		}

		/// <summary>
		/// Gets the item middle position by index.
		/// </summary>
		/// <returns>The item middle position.</returns>
		/// <param name="index">Index.</param>
		public override float GetItemPositionMiddle(int index)
		{
			var start = GetItemPosition(index);
			var end = GetItemPositionBottom(index);
			return start + (end - start) / 2;
		}

		/// <summary>
		/// Calculate and sets the height of the items.
		/// </summary>
		/// <param name="items">Items.</param>
		/// <param name="forceUpdate">If set to <c>true</c> force update.</param>
		void CalculateItemsSizes(ObservableList<TItem> items, bool forceUpdate = true)
		{
			// remove old items
			ItemSizes.Keys.Except(items).ToArray().ForEach(x => ItemSizes.Remove(x));
			
			if (defaultItemLayoutGroup==null)
			{
				DefaultItemCopy.gameObject.SetActive(true);

				defaultItemLayoutGroup = DefaultItemCopy.GetComponent<LayoutGroup>();
			}

			if (forceUpdate)
			{
				DefaultItemCopy.gameObject.SetActive(true);
				
				items.ForEach(x => {
					ItemSizes[x] = CalculateComponentSize(x);
				});
				
				DefaultItemCopy.gameObject.SetActive(false);
			}
			else
			{
				items.Except(ItemSizes.Keys).ForEach(x => {
					ItemSizes[x] = CalculateComponentSize(x);
				});
			}
		}

		/// <summary>
		/// Resize this instance.
		/// </summary>
		public override void Resize()
		{
			CalculateItemsSizes(DataSource, true);

			base.Resize();
		}

		/// <summary>
		/// Updates the items.
		/// </summary>
		/// <param name="newItems">New items.</param>
		/// <param name="updateView">Update view.</param>
		protected override void SetNewItems(ObservableList<TItem> newItems, bool updateView=true)
		{
			CalculateItemsSizes(newItems);
			//CalculateMaxVisibleItems();

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
					CalculateItemsSizes(DataSource);
					CalculateMaxVisibleItems();

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

		int GetIndexBySize(float total_size)
		{
			var spacing = LayoutBridge.GetSpacing();
			return BlockSizes.TakeWhile((size, index) => {
				total_size -= size;
				if (index > 0)
				{
					total_size -= spacing;
				}
				return total_size >= 0;
			}).Count() * ItemsPerBlock();
		}

		/// <summary>
		/// Gets the last index of the visible.
		/// </summary>
		/// <returns>The last visible index.</returns>
		/// <param name="strict">If set to <c>true</c> strict.</param>
		protected override int GetLastVisibleIndex(bool strict=false)
		{
			var last_visible_index = GetIndexBySize(GetScrollValue() + scrollHeight);

			return (strict) ? last_visible_index : last_visible_index + GetItemsPerBlock();
		}

		/// <summary>
		/// Gets the first index of the visible.
		/// </summary>
		/// <returns>The first visible index.</returns>
		/// <param name="strict">If set to <c>true</c> strict.</param>
		protected override int GetFirstVisibleIndex(bool strict=false)
		{
			var first_visible_index = GetIndexBySize(GetScrollValue());

			return first_visible_index;
		}

		LayoutGroup defaultItemLayoutGroup;

		Vector2 CalculateComponentSize(TItem item)
		{
			if (defaultItemLayoutGroup==null)
			{
				return Vector2.zero;
			}

			SetData(DefaultItemCopy, item);

			var lg = DefaultItemCopy.GetComponentsInChildren<LayoutGroup>();
			Array.Reverse(lg);
			lg.ForEach(LayoutUtilites.UpdateLayout);

			LayoutUtilites.UpdateLayout(defaultItemLayoutGroup);

			var size = new Vector2(
				LayoutUtility.GetPreferredWidth(DefaultItemCopyRect),
				LayoutUtility.GetPreferredHeight(DefaultItemCopyRect)
			);

			return size;
		}

		/// <summary>
		/// Raises the item move event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		/// <param name="item">Item.</param>
		protected override void OnItemMove(AxisEventData eventData, ListViewItem item)
		{
			if (!Navigation)
			{
				return ;
			}

			var block = item.Index % ItemsPerBlock();
			switch (eventData.moveDir)
			{
				case MoveDirection.Left:
					if (block > 0)
					{
						SelectComponentByIndex(item.Index - 1);
					}
					break;
				case MoveDirection.Right:
					if (block < (ItemsPerBlock() - 1))
					{
						SelectComponentByIndex(item.Index + 1);
					}
					break;
				case MoveDirection.Up:
					var index_up = item.Index - ItemsPerBlock();
					if (IsValid(index_up))
					{
						SelectComponentByIndex(index_up);
					}
					break;
				case MoveDirection.Down:
					var index_down = item.Index + ItemsPerBlock();
					if (IsValid(index_down))
					{
						SelectComponentByIndex(index_down);
					}
					break;
			}
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
			var item = DataSource[index];
			var current_size = ItemSizes[item];
			var is_changed = (IsHorizontal() && current_size.x != size.x) || (!IsHorizontal() && current_size.y != size.y);
			if (is_changed)
			{
				ItemSizes[item] = size;

				var old = maxVisibleItems;
				CalculateMaxVisibleItems();
				if (maxVisibleItems > old)
				{
					UpdateView();
				}
				else
				{
					ScrollUpdate();
				}
			}
		}

		/// <summary>
		/// Scrolls the update.
		/// </summary>
		protected override void ScrollUpdate()
		{
			var oldTopHiddenItems = topHiddenItems;
			
			topHiddenItems = GetFirstVisibleIndex();

			if (topHiddenItems > (DataSource.Count - (1 * GetItemsPerBlock())))
			{
				topHiddenItems = Mathf.Max(0, DataSource.Count - (2 * GetItemsPerBlock()));
			}

			if (oldTopHiddenItems==topHiddenItems)
			{
				return ;
			}

			if ((CanOptimize()) && (DataSource.Count > 0))
			{
				visibleItems = (maxVisibleItems < DataSource.Count) ? maxVisibleItems : DataSource.Count;
			}
			else
			{
				visibleItems = DataSource.Count;
			}

			if ((topHiddenItems + visibleItems) > DataSource.Count)
			{
				visibleItems = DataSource.Count - topHiddenItems;
				if (visibleItems < ItemsPerBlock())
				{
					visibleItems = Mathf.Min(DataSource.Count, visibleItems + ItemsPerBlock());
					topHiddenItems = DataSource.Count - visibleItems;
				}
			}

			RemoveCallbacks();

			UpdateComponentsCount();

			bottomHiddenItems = Mathf.Max(0, DataSource.Count - visibleItems - topHiddenItems);

			var new_visible_range = Enumerable.Range(topHiddenItems, visibleItems).ToList();
			var current_visible_range = components.Convert<TComponent,int>(GetComponentIndex);

			var new_indices_to_change = new_visible_range.Except(current_visible_range).ToList();
			var components_to_change = new Stack<TComponent>(components.Where(x => !new_visible_range.Contains(x.Index)));

			new_indices_to_change.ForEach(index => {
				var component = components_to_change.Pop();

				component.Index = index;
				SetData(component, DataSource[index]);
				UpdateComponentLayout(component);
				Coloring(component as ListViewItem);
			});

			components.Sort(ComponentsComparer);
			components.ForEach(SetComponentAsLastSibling);

			AddCallbacks();

			if (LayoutBridge!=null)
			{
				LayoutBridge.SetFiller(CalculateTopFillerSize(), CalculateBottomFillerSize());
				LayoutBridge.UpdateLayout();
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

			var pos_block = IsHorizontal() ? point.x : -point.y;
			var start = GetIndexBySize(pos_block);

			var pos_elem = IsHorizontal() ? -point.y : point.x;
			var spacing = LayoutBridge.GetSpacing();
			var end = Mathf.Min(DataSource.Count, start + GetItemsPerBlock());

			for (int index = start; index < end; index++)
			{
				pos_elem -= GetItemSize(DataSource[index]);
				if (index > 0)
				{
					pos_elem -= spacing;
				}
				if (pos_elem < 0)
				{
					return index - 1;
				}
			}

			return Mathf.Min(DataSource.Count, start);
		}

		#region ListViewPaginator support
		/// <summary>
		/// Gets the items per block count.
		/// </summary>
		/// <returns>The items per block.</returns>
		public override int GetItemsPerBlock()
		{
			return ItemsPerBlock();
		}

		/// <summary>
		/// Gets the index of the nearest item.
		/// </summary>
		/// <returns>The nearest item index.</returns>
		public override int GetNearestItemIndex()
		{
			return GetIndexBySize(GetScrollValue());
		}
		#endregion
	}
}