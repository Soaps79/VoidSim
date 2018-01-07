using UnityEngine;
using UnityEngine.UI;
using UIWidgets;

namespace UIWidgetsSamples
{
	/// <summary>
	/// Disable Combobox input.
	/// </summary>
	[RequireComponent(typeof(Combobox))]
	public class ComboboxDisableDefaultInput : MonoBehaviour
	{
		/// <summary>
		/// Disable combobox input.
		/// </summary>
		protected virtual void Start()
		{
			// disable default combobox input, this also disable input field
			GetComponent<Combobox>().Editable = false;

			// enable input field back
			GetComponent<InputField>().interactable = true;
		}
	}
}