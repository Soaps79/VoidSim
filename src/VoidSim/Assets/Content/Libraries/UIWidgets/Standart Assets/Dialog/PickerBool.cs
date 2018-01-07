using UnityEngine;
using UnityEngine.UI;

namespace UIWidgets
{
	/// <summary>
	/// PickerBool.
	/// </summary>
	[AddComponentMenu("UI/UIWidgets/PickerBool")]
	public class PickerBool : Picker<bool,PickerBool>
	{
		/// <summary>
		/// Message.
		/// </summary>
		[SerializeField]
		protected Text Message;

		/// <summary>
		/// Set message.
		/// </summary>
		/// <param name="message">Message text.</param>
		public virtual void SetMessage(string message)
		{
			Message.text = message;
		}

		/// <summary>
		/// Prepare picker to open.
		/// </summary>
		/// <param name="defaultValue">Default value.</param>
		public override void BeforeOpen(bool defaultValue)
		{
		}

		/// <summary>
		/// Select true value.
		/// </summary>
		public void Yes()
		{
			Selected(true);
		}

		/// <summary>
		/// Select false value.
		/// </summary>
		public void No()
		{
			Selected(false);
		}

		/// <summary>
		/// Prepare picker to close.
		/// </summary>
		public override void BeforeClose()
		{
		}
	}
}