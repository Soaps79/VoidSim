using UnityEngine;
using System.ComponentModel;
using UnityEngine.Serialization;

namespace UIWidgets
{
	/// <summary>
	/// ListViewIcons item description.
	/// </summary>
	[System.Serializable]
	public class ListViewIconsItemDescription : INotifyPropertyChanged
	{
		[SerializeField]
		[FormerlySerializedAs("Icon")]
		Sprite icon;

		/// <summary>
		/// The icon.
		/// </summary>
		public Sprite Icon {
			get {
				return icon;
			}
			set {
				icon = value;
				Changed("Icon");
			}
		}

		[SerializeField]
		[FormerlySerializedAs("Name")]
		string name;

		/// <summary>
		/// The name.
		/// </summary>
		public string Name {
			get {
				return name;
			}
			set {
				name = value;
				Changed("Name");
			}
		}

		[System.NonSerialized]
		string localizedName;

		/// <summary>
		/// The localized name.
		/// </summary>
		public string LocalizedName {
			get {
				return localizedName;
			}
			set {
				localizedName = value;
				Changed("LocalizedName");
			}
		}

		[SerializeField]
		[FormerlySerializedAs("Value")]
		int val;

		/// <summary>
		/// The value.
		/// </summary>
		public int Value {
			get {
				return val;
			}
			set {
				val = value;
				Changed("Value");
			}
		}

		/// <summary>
		/// Occurs when a property value changes.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged = (x, y) => { };

		void Changed(string propertyName)
		{
			PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}