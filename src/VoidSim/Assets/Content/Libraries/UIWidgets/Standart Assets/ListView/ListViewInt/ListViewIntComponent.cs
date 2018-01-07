using UnityEngine;
using UnityEngine.UI;

namespace UIWidgets
{
	/// <summary>
	/// ListViewInt component.
	/// </summary>
	public class ListViewIntComponent : ListViewIntComponentBase, IViewData<int>
	{
		/// <summary>
		/// Foreground graphics for coloring.
		/// </summary>
		public override Graphic[] GraphicsForeground {
			get {
				return new Graphic[] {Number, };
			}
		}

		/// <summary>
		/// The number.
		/// </summary>
		[SerializeField]
		public Text Number;

		/// <summary>
		/// Sets the data.
		/// </summary>
		/// <param name="item">Item.</param>
		public override void SetData(int item)
		{
			Number.text = item.ToString();
		}
	}
}