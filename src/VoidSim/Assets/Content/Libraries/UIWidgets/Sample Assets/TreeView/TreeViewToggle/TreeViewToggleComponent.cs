using UnityEngine;
using UIWidgets;

namespace UIWidgetsSamples
{
	/// <summary>
	/// TreeViewToggleComponent.
	/// How to process clicks on gameobjects inside DefaultItem.
	/// </summary>
	public class TreeViewToggleComponent : TreeViewComponent
	{
		/// <summary>
		/// Process click and activate or deactivate objects depends of item value.
		/// Should be attached to Button.onClick() or other similar event.
		/// </summary>
		public void ProcessClick()
		{
			// toogle Item.Value - 1 = selected, 0 = unselected
			Item.Value = (Item.Value==0) ? 1 : 0;

			if (Item.Value==1)// if node selected
			{
				Debug.Log("selected: " + Item.Name);

				// activate corresponding GameObjects
			}
			else// if node deselected
			{
				Debug.Log("deselected: " + Item.Name);

				// deactivate corresponding GameObjects
			}
		}
	}
}