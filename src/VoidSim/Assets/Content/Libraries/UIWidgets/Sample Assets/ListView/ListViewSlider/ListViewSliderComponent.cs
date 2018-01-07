using UnityEngine.UI;
using UnityEngine.EventSystems;
using UIWidgets;

namespace UIWidgetsSamples
{
	/// <summary>
	/// ListViewSlider component.
	/// </summary>
	public class ListViewSliderComponent : ListViewItem, IViewData<ListViewSliderItem>
	{
		/// <summary>
		/// Foreground graphics for coloring.
		/// </summary>
		public override Graphic[] GraphicsForeground {
			get {
				return new Graphic[] { };
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
		/// Slider.
		/// </summary>
		public Slider Slider;

		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="item">Item.</param>
		public void SetData(ListViewSliderItem item)
		{
			Slider.value = item.Value;
		}

		/// <summary>
		/// Handle OnMove event.
		/// Redirect left and right movements events to slider.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public override void OnMove(AxisEventData eventData)
		{
			switch (eventData.moveDir)
			{
				case MoveDirection.Left:
				case MoveDirection.Right:
					Slider.OnMove(eventData);
					break;
				default:
					base.OnMove(eventData);
					break;
			}
		}
	}
}