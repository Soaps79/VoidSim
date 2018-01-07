using UnityEngine;
using UIWidgets;
using TMPro;

namespace UIWidgets.TMProSupport {

	/// <summary>
	/// Notify info.
	/// </summary>
	public class NotifyInfoTMPro : NotifyInfoBase {
		/// <summary>
		/// The message.
		/// </summary>
		[SerializeField]
		public TextMeshProUGUI Message;

		/// <summary>
		/// Sets the info.
		/// </summary>
		/// <param name="message">Message.</param>
		public override void SetInfo(string message)
		{
			if ((Message!=null) && (message!=null))
			{
				Message.text = message;
			}
		}
	}
}
