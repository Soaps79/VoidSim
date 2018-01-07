﻿using UnityEngine;
using UnityEngine.UI;
using UIWidgets;
using TMPro;

namespace UIWidgets.TMProSupport {

	/// <summary>
	/// Dialog info.
	/// </summary>
	public class DialogInfoTMPro : DialogInfoBase {
		/// <summary>
		/// The title.
		/// </summary>
		[SerializeField]
		public TextMeshProUGUI Title;

		/// <summary>
		/// The message.
		/// </summary>
		[SerializeField]
		public TextMeshProUGUI Message;

		/// <summary>
		/// The icon.
		/// </summary>
		[SerializeField]
		public Image Icon;

		/// <summary>
		/// Sets the info.
		/// </summary>
		/// <param name="title">Title.</param>
		/// <param name="message">Message.</param>
		/// <param name="icon">Icon.</param>
		public override void SetInfo(string title, string message, Sprite icon)
		{
			if (Title!=null)
			{
				Title.text = title;
			}
			if (Message!=null)
			{
				Message.text = message;
			}
			if (Icon!=null)
			{
				Icon.sprite = icon;
			}
		}

	}
}
