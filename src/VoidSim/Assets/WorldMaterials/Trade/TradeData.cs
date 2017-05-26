using System;
using Messaging;

namespace Assets.WorldMaterials.Trade
{
	public enum TradeStatus
	{
		Accepted, Rejected, Complete
	}

	public class TradeCreatedMessageArgs : MessageArgs
	{
		public TradeInfo TradeInfo;
	}

	public class TradeInfo
	{
		public const string MessageName = "TradeInfo updated";

		public int Id;
		public ProductTrader Provider;
		public ProductTrader Consumer;
		public int ProductId;

		public int CreditsTotal;
		public int CreditsComplete { get; private set; }

		public int AmountTotal;
		public int AmountComplete { get; private set; }

		public Action<TradeInfo> OnStatusChanged;
		public Action<TradeInfo> OnAmountCompleted;

		private TradeStatus _status;
		/// <summary>
		/// Beware that changing this will result in a message being sent
		/// </summary>
		public TradeStatus Status
		{
			get { return _status; }
			set
			{
				if (_status == value)
					return;

				_status = value;
				if (OnStatusChanged != null)
					OnStatusChanged(this);
			}
		}

		public void CompleteAmount(int productAmount, int currencyAmount)
		{
			if (productAmount == 0)
				return;

			CreditsComplete += currencyAmount;
			AmountComplete += productAmount;
			if (OnAmountCompleted != null)
				OnAmountCompleted(this);

			if (AmountComplete >= AmountTotal)
				Status = TradeStatus.Complete;
		}
	}
}