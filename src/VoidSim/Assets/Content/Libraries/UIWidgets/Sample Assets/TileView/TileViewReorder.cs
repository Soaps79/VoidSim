﻿using UnityEngine;
using System.Linq;
using UIWidgets;

namespace UIWidgetsSamples
{
	/// <summary>
	/// Reorder TileView items.
	/// </summary>
	public class TileViewReorder : MonoBehaviour
	{
		/// <summary>
		/// TileView.
		/// </summary>
		[SerializeField]
		public TileViewSample Tiles;

		/// <summary>
		/// Original items.
		/// </summary>
		public ObservableList<TileViewItemSample> OriginalItems = new ObservableList<TileViewItemSample>();

		/// <summary>
		/// Empty item.
		/// </summary>
		[SerializeField]
		public TileViewItemSample EmptyItem = null;

		/// <summary>
		/// Per row.
		/// </summary>
		protected int PerRow = 1;

		/// <summary>
		/// Per column.
		/// </summary>
		protected int PerColumn = 1;

		/// <summary>
		/// Per page.
		/// </summary>
		protected int PerPage = 1;

		/// <summary>
		/// Start this instance.
		/// </summary>
		protected virtual void Start()
		{
			foreach (var i in Enumerable.Range(0, 40))
			{
				OriginalItems.Add(new TileViewItemSample(){
					Name = "Tile " + i,
					Capital = "",
					Area = Random.Range(10, 10*6),
					Population = Random.Range(100, 100*6),
				});
			}

			OriginalItems.OnCollectionChange += ReorderItems;

			SizeChanged();

			var listener = Utilites.GetOrAddComponent<ResizeListener>(Tiles.ScrollRect);
			listener.OnResize.AddListener(SizeChanged);
		}

		/// <summary>
		/// Handge size changed event.
		/// </summary>
		public void SizeChanged()
		{
			var size = (Tiles.ScrollRect.transform as RectTransform).rect.size;
			var margin = Tiles.GetLayoutMargin();
			size.x -= margin.x + margin.y;
			size.y -= margin.z + margin.w;

			PerRow = Mathf.Max(1, Mathf.FloorToInt((size.x + Tiles.GetItemSpacingX()) / (Tiles.GetDefaultItemWidth() + Tiles.GetItemSpacingX())));
			PerColumn = Mathf.Max(1, Mathf.FloorToInt((size.y + Tiles.GetItemSpacingY()) / (Tiles.GetDefaultItemHeight() + Tiles.GetItemSpacingY())));
			PerPage = PerRow * PerColumn;

			ReorderItems();
		}

		/// <summary>
		/// Reorder items.
		/// </summary>
		public void ReorderItems()
		{
			var items = Tiles.DataSource;
			items.BeginUpdate();
			items.Clear();

			var total = Mathf.CeilToInt((float)OriginalItems.Count / (float)PerPage) * PerPage;
			for (int i = 0; i < total; i++)
			{
				var k = ((i / PerPage) * PerPage) + ((i % PerColumn) * PerRow) + ((i % PerPage) / PerColumn);
				items.Add((k < OriginalItems.Count) ? OriginalItems[k] : null);
			}

			items.EndUpdate();
		}

		/// <summary>
		/// Remove listeners.
		/// </summary>
		protected virtual void OnDestroy()
		{
			OriginalItems.OnCollectionChange -= ReorderItems;

			if ((Tiles!=null) && (Tiles.ScrollRect!=null))
			{
				var listener = Tiles.ScrollRect.GetComponent<ResizeListener>();
				listener.OnResize.RemoveListener(SizeChanged);
			}
		}
	}
}