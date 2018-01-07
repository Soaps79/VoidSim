using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UIWidgets;

namespace UIWidgetsSamples.Shops
{
	/// <summary>
	/// HarborListViewComponent.
	/// </summary>
	public class HarborListViewComponent : ListViewItem, IViewData<HarborOrderLine>
	{
		/// <summary>
		/// Foreground graphics for coloring.
		/// </summary>
		public override Graphic[] GraphicsForeground {
			get {
				return new Graphic[] {Name, BuyPrice, SellPrice, AvailableBuyCount, AvailableSellCount, };
			}
		}

		/// <summary>
		/// Name.
		/// </summary>
		[SerializeField]
		public Text Name;

		/// <summary>
		/// Sell price.
		/// </summary>
		[SerializeField]
		public Text SellPrice;

		/// <summary>
		/// Buy price.
		/// </summary>
		[SerializeField]
		public Text BuyPrice;
		
		/// <summary>
		/// Available buy count.
		/// </summary>
		[SerializeField]
		public Text AvailableBuyCount;

		/// <summary>
		/// Available sell count.
		/// </summary>
		[SerializeField]
		public Text AvailableSellCount;

		/// <summary>
		/// Count slider.
		/// </summary>
		[SerializeField]
		protected CenteredSlider Count;
		
		/// <summary>
		/// Current order line.
		/// </summary>
		public HarborOrderLine OrderLine;

		/// <summary>
		/// Add listeners.
		/// </summary>
		protected override void Start()
		{
			Count.OnValuesChange.AddListener(ChangeCount);
			base.Start();
		}

		/// <summary>
		/// Change count on left and right movements.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public override void OnMove(AxisEventData eventData)
		{
			switch (eventData.moveDir)
			{
				case MoveDirection.Left:
					Count.Value -= 1;
					break;
				case MoveDirection.Right:
					Count.Value += 1;
					break;
				default:
					base.OnMove(eventData);
					break;
			}
		}

		/// <summary>
		/// Sets the data.
		/// </summary>
		/// <param name="item">Order line.</param>
		public void SetData(HarborOrderLine item)
		{
			OrderLine = item;
			
			Name.text = OrderLine.Item.Name;

			BuyPrice.text = OrderLine.BuyPrice.ToString();
			SellPrice.text = OrderLine.SellPrice.ToString();

			AvailableBuyCount.text = OrderLine.BuyCount.ToString();
			AvailableSellCount.text = OrderLine.SellCount.ToString();

			Count.LimitMin = -OrderLine.SellCount;
			Count.LimitMax = OrderLine.BuyCount;
			Count.Value = OrderLine.Count;
		}
		
		void ChangeCount(int value)
		{
			OrderLine.Count = value;
		}

		/// <summary>
		/// Remove listeners.
		/// </summary>
		protected override void OnDestroy()
		{
			if (Count!=null)
			{
				Count.OnValuesChange.RemoveListener(ChangeCount);
			}
		}
	}
}