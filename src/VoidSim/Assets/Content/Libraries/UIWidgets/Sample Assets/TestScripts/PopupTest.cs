using UnityEngine;
using UIWidgets;
using System.Linq;
using UnityEngine.Serialization;

namespace UIWidgetsSamples
{
	/// <summary>
	/// Test Popup.
	/// </summary>
	public class PopupTest : MonoBehaviour
	{
		/// <summary>
		/// Popup template.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("popup")]
		protected Popup PopupTemplate;

		Popup currentPopup;

		ListViewInt currentListView;

		/// <summary>
		/// Show picker.
		/// </summary>
		public void ShowPicker()
		{
			currentPopup = PopupTemplate.Clone();
			currentPopup.Show();
			currentListView = currentPopup.GetComponentInChildren<ListViewInt>();

			// fill list with values
			currentListView.DataSource = Enumerable.Range(1, 100).ToObservableList();

			// deselect
			currentListView.SelectedIndex = -1;
			currentListView.OnSelectObject.AddListener(Callback);
		}

		void Callback(int index)
		{
			// do something with value
			Debug.Log(currentListView.DataSource[index]);

			currentListView.OnSelectObject.RemoveListener(Callback);
			currentPopup.Close();
		}
	}
}