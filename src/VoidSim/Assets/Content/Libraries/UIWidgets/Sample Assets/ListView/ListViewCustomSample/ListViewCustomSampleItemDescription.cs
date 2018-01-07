using UnityEngine;
using System;

namespace UIWidgetsSamples
{
	/// <summary>
	/// ListViewCustom sample item description.
	/// </summary>
	[Serializable]
	public class ListViewCustomSampleItemDescription
	{
		/// <summary>
		/// Icon.
		/// </summary>
		[SerializeField]
		public Sprite Icon;

		/// <summary>
		/// Name.
		/// </summary>
		[SerializeField]
		public string Name;

		/// <summary>
		/// Progress.
		/// </summary>
		[SerializeField]
		public int Progress;
	}
}