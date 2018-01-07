using UnityEngine;
using UnityEngine.UI;
using UIWidgets;
using System.Collections.Generic;
using System.Linq;

namespace UIWidgetsSamples
{
	/// <summary>
	/// TableList component.
	/// </summary>
	public class TableListComponent : ListViewItem, IResizableItem, IViewData<List<int>>
	{
		/// <summary>
		/// The text components.
		/// </summary>
		[SerializeField]
		public List<Text> TextComponents = new List<Text>();

		/// <summary>
		/// Foreground graphics for coloring.
		/// </summary>
		public override Graphic[] GraphicsForeground {
			get {
				return TextComponents.Select(x => x as Graphic).ToArray();
			}
		}

		/// <summary>
		/// Background graphics for coloring.
		/// </summary>
		public override Graphic[] GraphicsBackground {
			get {
				return new Graphic[] { };
			}
		}

		/// <summary>
		/// Gets the objects to resize.
		/// </summary>
		/// <value>The objects to resize.</value>
		public GameObject[] ObjectsToResize {
			get {
				return TextComponents.Select(x => x.transform.parent.gameObject).ToArray();
			}
		}

		/// <summary>
		/// The item.
		/// </summary>
		[SerializeField]
		protected List<int> Item;

		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="item">Item.</param>
		public void SetData(List<int> item)
		{
			Item = item;
			UpdateView();
		}

		/// <summary>
		/// Update text components text.
		/// </summary>
		public void UpdateView()
		{
			TextComponents.ForEach(SetData);
		}

		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="index">Index.</param>
		/// <param name="text">Text.</param>
		protected void SetData(Text text, int index)
		{
			text.text = Item.Count > index ? Item[index].ToString() : "none";
		}
	}
}