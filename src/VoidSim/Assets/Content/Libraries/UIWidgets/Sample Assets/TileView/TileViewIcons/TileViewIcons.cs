using UnityEngine;
using System;
using UIWidgets;

namespace UIWidgetsSamples
{
	/// <summary>
	/// TileViewIcons.
	/// </summary>
	public class TileViewIcons : TileViewCustom<ListViewIconsItemComponent,ListViewIconsItemDescription>
	{
		/// <summary>
		/// Gets the index of the nearest item.
		/// </summary>
		/// <returns>The nearest item index.</returns>
		/// <param name="point">Point.</param>
		public override int GetNearestIndex(Vector2 point)
		{
			// RoundToInt replaced to FloorToInt
			if (IsSortEnabled())
			{
				return -1;
			}

			// block index
			var pos_block = IsHorizontal() ? point.x : -point.y;
			var block = Mathf.FloorToInt(pos_block / GetItemSize());

			// item index in block
			var pos_elem = IsHorizontal() ? -point.y : point.x;
			var size = (IsHorizontal()) ? itemHeight + GetItemSpacingY() : itemWidth + GetItemSpacingX();
			var k = Mathf.FloorToInt(pos_elem / size);

			return block * GetItemsPerBlock() + k;
		}
	}
}