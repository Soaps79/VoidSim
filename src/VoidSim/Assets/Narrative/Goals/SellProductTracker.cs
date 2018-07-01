using Assets.Logistics;
using Assets.Scripts;
using Assets.WorldMaterials.Trade;
using Messaging;

namespace Assets.Narrative.Goals
{
	/// <summary>
	/// Tracks completed sales of goods by the player
	/// </summary>
	public class SellProductTracker : ProductGoalTrackerBase, IMessageListener
	{
		private TradeMonitor _tradeMonitor;
		public override GoalType GoalType => GoalType.SellProduct;

	    public SellProductTracker()
		{
			Locator.MessageHub.AddListener(this, LogisticsMessages.CargoCompleted);
		}

		public void HandleMessage(string type, MessageArgs args)
		{
			if (type == LogisticsMessages.CargoCompleted && args != null)
				HandleCargoComplete(args as CargoCompletedMessageArgs);
		}

		private void HandleCargoComplete(CargoCompletedMessageArgs args)
		{
			if(args != null && args.Manifest.Shipper == Station.Station.ClientName)
				HandleProductupdate(args.Manifest.ProductId, args.Manifest.TotalAmount);
		}

		public string Name => "SellProductTracker";
	}
}