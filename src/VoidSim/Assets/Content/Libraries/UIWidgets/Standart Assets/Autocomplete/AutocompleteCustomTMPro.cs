#if UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1
#else
using UnityEngine;
using TMPro;

namespace UIWidgets {
	public abstract class AutocompleteCustomTMPro<TValue,TListViewComponent,TListView> : AutocompleteCustom<TValue,TListViewComponent,TListView>
		where TListView : ListViewCustom<TListViewComponent,TValue>
		where TListViewComponent : ListViewItem
	{
		/// <summary>
		/// InputField for autocomplete.
		/// </summary>
		[SerializeField]
		protected TMP_InputField InputFieldTMPro;
		
		/// <summary>
		/// Gets the InputFieldProxy.
		/// </summary>
		protected override IInputFieldProxy InputFieldProxy {
			get {
				if (inputFieldProxy==null)
				{
					inputFieldProxy = new InputFieldTMProProxy(InputFieldTMPro);
				}
				return inputFieldProxy;
			}
		}
	}
}
#endif