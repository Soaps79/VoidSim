using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;
using System.Linq;

namespace UIWidgets
{
	/// <summary>
	/// Base class for TileView's.
	/// </summary>
	/// <typeparam name="TComponent">Component class.</typeparam>
	/// <typeparam name="TItem">Item class.</typeparam>
	public class TileViewCustom<TComponent,TItem> : ListViewCustom<TComponent,TItem> where TComponent : ListViewItem
	{
		/// <summary>
		/// Items per row.
		/// </summary>
		protected int ItemsPerRow;

		/// <summary>
		/// Items per column.
		/// </summary>
		protected int ItemsPerColumn;

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
		/// Get block index by item index.
		/// </summary>
		/// <param name="index">Item index.</param>
		/// <returns>Block index.</returns>
		protected override int GetBlockIndex(int index)
		{
			return Mathf.FloorToInt((float)index / (float)ItemsPerBlock());
		}

		/// <summary>
		/// Calculates the max count of visible items.
		/// </summary>
		protected override void CalculateMaxVisibleItems()
		{
			var spacing_x = GetItemSpacingX();
			var spacing_y = GetItemSpacingY();

			var height = scrollHeight + spacing_y - LayoutBridge.GetFullMarginY();
			var width = scrollWidth + spacing_x - LayoutBridge.GetFullMarginX();

			if (IsHorizontal())
			{
				ItemsPerRow = Mathf.CeilToInt(width / (itemWidth + spacing_x)) + 1;
				ItemsPerRow = Mathf.Max(2, ItemsPerRow);

				ItemsPerColumn = Mathf.FloorToInt(height / (itemHeight + spacing_y));
				ItemsPerColumn = Mathf.Max(1, ItemsPerColumn);
				ItemsPerColumn = LayoutBridge.ColumnsConstraint(ItemsPerColumn);
			}
			else
			{
				ItemsPerRow = Mathf.FloorToInt(width / (itemWidth + spacing_x));
				ItemsPerRow = Mathf.Max(1, ItemsPerRow);
				ItemsPerRow = LayoutBridge.RowsConstraint(ItemsPerRow);
				
				ItemsPerColumn = Mathf.CeilToInt(height / (itemHeight + spacing_y)) + 1;
				ItemsPerColumn = Mathf.Max(2, ItemsPerColumn);
			}

			maxVisibleItems = ItemsPerRow * ItemsPerColumn;
		}

		/// <summary>
		/// Gets the index of first visible item.
		/// </summary>
		/// <returns>The first visible index.</returns>
		/// <param name="strict">If set to <c>true</c> strict.</param>
		protected override int GetFirstVisibleIndex(bool strict=false)
		{
			return Mathf.Max(0, base.GetFirstVisibleIndex(strict) * ItemsPerBlock());
		}

		/// <summary>
		/// Gets the index of last visible item.
		/// </summary>
		/// <returns>The last visible index.</returns>
		/// <param name="strict">If set to <c>true</c> strict.</param>
		protected override int GetLastVisibleIndex(bool strict=false)
		{
			return (base.GetLastVisibleIndex(strict) + 1) * ItemsPerBlock() - 1;
		}

		/// <summary>
		/// Scrolls the update.
		/// </summary>
		protected override void ScrollUpdate()
		{
			var oldTopHiddenItems = topHiddenItems;
			
			topHiddenItems = GetFirstVisibleIndex();
			if (topHiddenItems > (DataSource.Count - 1))
			{
				topHiddenItems = Mathf.Max(0, DataSource.Count - 2);
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

			// block index
			var pos_block = IsHorizontal() ? point.x : -point.y;
			var block = Mathf.RoundToInt(pos_block / GetItemSize());

			// item index in block
			var pos_elem = IsHorizontal() ? -point.y : point.x;
			var size = (IsHorizontal()) ? itemHeight + GetItemSpacingY() : itemWidth + GetItemSpacingX();
			var k = Mathf.FloorToInt(pos_elem / size);

			return block * GetItemsPerBlock() + k;
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
		/// Gets the blocks count.
		/// </summary>
		/// <returns>The blocks count.</returns>
		/// <param name="items">Items.</param>
		int GetBlocksCount(int items)
		{
			return Mathf.CeilToInt((float)items / (float)ItemsPerBlock());
		}

		/// <summary>
		/// Calculates the size of the bottom filler.
		/// </summary>
		/// <returns>The bottom filler size.</returns>
		protected override float CalculateBottomFillerSize()
		{
			var blocks = GetBlocksCount(bottomHiddenItems);
			if (blocks==0)
			{
				return 0f;
			}
			return Mathf.Max(0, blocks * GetItemSize());
		}

		/// <summary>
		/// Calculates the size of the top filler.
		/// </summary>
		/// <returns>The top filler size.</returns>
		protected override float CalculateTopFillerSize()
		{
			var blocks = GetBlocksCount(topHiddenItems);
			if (blocks==0)
			{
				return 0f;
			}
			return Mathf.Max(0, GetBlocksCount(topHiddenItems) * GetItemSize());
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
			return base.GetNearestItemIndex() * ItemsPerBlock();
		}
		#endregion
	}
}