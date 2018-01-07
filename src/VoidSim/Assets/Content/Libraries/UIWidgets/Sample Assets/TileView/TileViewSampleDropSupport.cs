using UnityEngine;
using UIWidgets;

namespace UIWidgetsSamples
{
	/// <summary>
	/// TileViewSample drop support.
	/// </summary>
	[RequireComponent(typeof(TileViewSample))]
	public class TileViewSampleDropSupport : ListViewCustomDropSupport<TileViewSample,TileViewComponentSample,TileViewItemSample>
	{
	}
}