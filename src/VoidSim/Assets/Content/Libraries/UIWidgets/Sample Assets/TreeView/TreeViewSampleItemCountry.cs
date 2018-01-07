using UnityEngine;
using UIWidgets;

namespace UIWidgetsSamples
{
	/// <summary>
	/// TreeViewSampleItemCountry.
	/// Sample class to display country data.
	/// </summary>
	[System.Serializable]
	public class TreeViewSampleItemCountry : ITreeViewSampleItem
	{
		/// <summary>
		/// OnChange event.
		/// </summary>
		public event OnChange OnChange = () => { };
		
		[SerializeField]
		Sprite icon;
		
		/// <summary>
		/// Icon.
		/// </summary>
		public Sprite Icon {
			get {
				return icon;
			}
			set {
				icon = value;
				Changed();
			}
		}

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

		/// <summary>
		/// Constructur.
		/// </summary>
		/// <param name="itemName">Name.</param>
		/// <param name="itemIcon">Icon.</param>
		public TreeViewSampleItemCountry(string itemName, Sprite itemIcon = null)
		{
			name = itemName;
			icon = itemIcon;
		}
		
		void Changed()
		{
			OnChange();
		}

		/// <summary>
		/// Display data with specified component.
		/// </summary>
		/// <param name="component">Component to display item.</param>
		public void Display(TreeViewSampleComponent component)
		{
			component.Icon.sprite = Icon;
			component.Text.text = Name;
			
			if (component.SetNativeSize)
			{
				component.Icon.SetNativeSize();
			}
			
			component.Icon.color = (component.Icon.sprite==null) ? Color.clear : Color.white;
		}
	}
}