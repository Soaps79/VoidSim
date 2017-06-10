using System;
using System.ComponentModel.Design;
using Messaging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Assets.WorldMaterials.Trade
{
	public enum TradeStatus
	{
		Accepted, Rejected, Complete
	}

	public class TradeCreatedMessageArgs : MessageArgs
	{
		public TradeManifest TradeManifest;
	}

	[Serializable]
	public class TradeManifestData
	{
		public int Id;
		public string Provider;
		public string Consumer;
		public int ProductId;
		public int CreditsTotal;
		public int CreditsComplete;
		public int AmountTotal;
		public int AmountComplete;
		[JsonConverter(typeof(StringEnumConverter))]
		public TradeStatus Status;
	}

	/// <summary>
	/// Holds data pertaining to a trade matched up in ProductTradingHub between two traders.
	/// Serves as a monitor for actual fulfillment, in that the traded goods are distributed to CargoManifests, 
	/// which report back to this object when they are completed, eventually completing the TradeManifest itself
	/// </summary>
	public class TradeManifest
	{
		public int Id;
		public string Provider;
		public string Consumer;
		public int ProductId;

		public int CreditsTotal;
		public int CreditsComplete { get; private set; }

		public int AmountTotal;
		public int AmountComplete { get; private set; }

		public Action<TradeManifest> OnStatusChanged;
		public Action<TradeManifest> OnAmountCompleted;

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

		public TradeManifest() { }

		public TradeManifest(TradeManifestData data)
		{
			Id = data.Id;
			Provider = data.Provider;
			Consumer = data.Consumer;
			ProductId = data.ProductId;
			CreditsTotal = data.CreditsTotal;
			CreditsComplete = data.CreditsComplete;
			AmountTotal = data.AmountTotal;
			AmountComplete = data.AmountComplete;
			_status = data.Status;
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

		public TradeManifestData GetData()
		{
			return new TradeManifestData
			{
				Id = Id,
				Provider = Provider,
				Consumer = Consumer,
				ProductId = ProductId,
				CreditsTotal = CreditsTotal,
				CreditsComplete = CreditsComplete,
				AmountTotal = AmountTotal,
				AmountComplete = AmountComplete,
				Status = _status
			};
		}
	}
}