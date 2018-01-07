using UnityEngine;
using UIWidgets;

namespace UIWidgetsSamples.ToDoList
{
	/// <summary>
	/// ToDoList item.
	/// </summary>
	[System.Serializable]
	public class ToDoListItem : IItemHeight
	{
		/// <summary>
		/// Is task done?
		/// </summary>
		[SerializeField]
		public bool Done;

		/// <summary>
		/// Task.
		/// </summary>
		[SerializeField]
		public string Task;

		[SerializeField]
		float height;
		
		/// <summary>
		/// Task height.
		/// </summary>
		public float Height {
			get {
				return height;
			}
			set {
				height = value;
			}
		}
	}
}