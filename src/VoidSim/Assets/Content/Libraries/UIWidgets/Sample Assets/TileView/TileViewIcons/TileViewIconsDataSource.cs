using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UIWidgets;

namespace UIWidgetsSamples
{
	/// <summary>
	/// TileViewIcons DataSource.
	/// </summary>
	[RequireComponent(typeof(TileViewIcons))]
	public class TileViewIconsDataSource : MonoBehaviour
	{
		/// <summary>
		/// Icons.
		/// </summary>
		[SerializeField]
		protected List<Sprite> Icons;

		/// <summary>
		/// Generate and set TileViewIcons DataSource.
		/// </summary>
		protected virtual void Start()
		{
			var n = Icons.Count - 1;
			var tiles = GetComponent<TileViewIcons>();
			tiles.Init();

			var items = tiles.DataSource;
			items.BeginUpdate();
			foreach (var i in Enumerable.Range(0, 140))
			{
				tiles.Add(new ListViewIconsItemDescription(){
					Name = "Tile " + i,
					Icon = Icons.Count > 0 ? Icons[Random.Range(0, n)] : null
				});
			}
			items.EndUpdate();
		}
	}
}