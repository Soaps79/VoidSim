using UnityEngine;
using System.Linq;
using UIWidgets;

namespace UIWidgetsSamples
{
	/// <summary>
	/// Test ListView with Slider.
	/// </summary>
	public class TestListViewSlider : MonoBehaviour
	{
		/// <summary>
		/// ListViewSlider.
		/// </summary>
		[SerializeField]
		protected ListViewSlider ListView;

		/// <summary>
		/// Set ListView DataSource.
		/// </summary>
		public void SetList()
		{
			var data = new ObservableList<ListViewSliderItem>();

			foreach (var i in Enumerable.Range(0, 100))
			{
				data.Add(new ListViewSliderItem(){Value = i});
				data[i].Value = Random.Range(0, 100);
			}

			ListView.DataSource = data;
		}
	}
}