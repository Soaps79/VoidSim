using UnityEngine;
using System;
using UIWidgets;

namespace UIWidgetsSamples
{
	/// <summary>
	/// SimpleTable item.
	/// </summary>
	[Serializable]
	public class SimpleTableItem : IItemHeight
	{
		/// <summary>
		/// Item height.
		/// </summary>
		public float Height {
			get; set;
		}

		/// <summary>
		/// Field1.
		/// </summary>
		[SerializeField]
		public string Field1;

		/// <summary>
		/// Field2.
		/// </summary>
		[SerializeField]
		public string Field2;
	}
}