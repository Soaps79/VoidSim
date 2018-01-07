using UnityEngine;
using System.Linq;
using UIWidgets;

namespace UIWidgetsSamples
{
	/// <summary>
	/// TileViewSample DataSource.
	/// </summary>
	[RequireComponent(typeof(TileViewSample))]
	public class TileViewSampleDataSource : MonoBehaviour
	{
		/// <summary>
		/// Start this instances.
		/// </summary>
		protected virtual void Start()
		{
			GenerateItems(40);
		}

		/// <summary>
		/// Generate DataSource with specified items count.
		/// </summary>
		/// <param name="count">Items count.</param>
		public void GenerateItems(int count)
		{
			var tiles = GetComponent<TileViewSample>();
			tiles.Init();

			var items = tiles.DataSource;
			items.BeginUpdate();
			items.Clear();
			foreach (var i in Enumerable.Range(0, count))
			{
				items.Add(new TileViewItemSample(){
					Name = "Tile " + i,
					Capital = "",
					Area = Random.Range(10, 10*6),
					Population = Random.Range(100, 100*6),
				});
			}
			items.EndUpdate();

		}
	}
}