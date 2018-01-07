using UnityEngine;
using UIWidgets;
using TMPro;

namespace UIWidgets.TMProSupport
{
	/// <summary>
	/// PickerBool.
	/// </summary>
	public class PickerBoolTMPro : PickerBool
	{
		/// <summary>
		/// Message.
		/// </summary>
		[SerializeField]
		protected TextMeshProUGUI MessageTMPro;

		/// <summary>
		/// Set message.
		/// </summary>
		/// <param name="message">Message text.</param>
		public override void SetMessage(string message)
		{
			MessageTMPro.text = message;
		}
	}
}