using UnityEngine;
using UIWidgets;

namespace UIWidgetsSamples
{
	/// <summary>
	/// TreeViewSample continent item.
	/// </summary>
	[System.Serializable]
	public class TreeViewSampleItemContinent : ITreeViewSampleItem
	{
		/// <summary>
		/// OnChange event.
		/// </summary>
		public event OnChange OnChange = () => { };
		
		[SerializeField]
		string name;
		
		/// <summary>
		/// Name.
		/// </summary>
		public string Name {
			get {
				return name;
			}
			set {
				name = value;
				Changed();
			}
		}

		[SerializeField]
		int countries;
		
		/// <summary>
		/// Countries.
		/// </summary>
		public int Countries {
			get {
				return countries;
			}
			set {
				countries = value;
				Changed();
			}
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="itemName">Name.</param>
		/// <param name="itemCountries">Countries.</param>
		public TreeViewSampleItemContinent(string itemName, int itemCountries = 0)
		{
			name = itemName;
			countries = itemCountries;
		}
		
		void Changed()
		{
			OnChange();
		}

		/// <summary>
		/// Display item data using specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		public void Display(TreeViewSampleComponent component)
		{
			component.Icon.sprite = null;
			component.Icon.color = Color.clear;
			component.Text.text = Name + " (Countries: " + Countries + ") ";
		}
	}
}