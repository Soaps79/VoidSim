using Assets.WorldMaterials.Trade;
using UnityEngine;

namespace Assets.Narrative.Goals
{
	/// <summary>
	/// Tracks completed sales of goods by the player
	/// </summary>
	public class SellProductTracker : ProductGoalTrackerBase
	{
		private TradeMonitor _tradeMonitor;
		public override GoalType GoalType { get { return GoalType.SellProduct; } }

		public SellProductTracker()
		{
			_tradeMonitor = GameObject.Find("TradeMonitor").GetComponent<TradeMonitor>();
			_tradeMonitor.OnTradeComplete += HandleTradeComplete;
		}

		private void HandleTradeComplete(TradeManifest trade)
		{
			if (trade.Provider != "Station")
				return;

			HandleProductupdate(trade.ProductId, trade.AmountComplete);
		}
	}
}