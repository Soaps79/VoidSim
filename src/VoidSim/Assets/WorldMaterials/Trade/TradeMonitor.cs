using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using Assets.Scripts.Serialization;
using Messaging;
using QGame;
using UnityEngine;

namespace Assets.WorldMaterials.Trade
{
	public class TradeMonitorData
	{
		public List<TradeManifestData> Manifests;
	}

	public class TradeMonitor : SingletonBehavior<TradeMonitor>, IMessageListener
	{
		private readonly List<TradeManifest> _activeManifests = new List<TradeManifest>();

		void Start()
		{
			OnEveryUpdate += UpdateTrades;
			MessageHub.Instance.AddListener(this, TradeMessages.TradeAccepted, true);
			MessageHub.Instance.AddListener(this, GameMessages.PreSave, true);
			if (SerializationHub.Instance.IsLoading)
				HandleGameLoad();
		}

		private void HandleGameLoad()
		{
			throw new System.NotImplementedException();
		}

		private void UpdateTrades(float delta)
		{
			_activeManifests.RemoveAll(i => i.Status == TradeStatus.Complete);
		}

		public TradeManifest GetTradeManifest(int id)
		{
			var manifest =_activeManifests.FirstOrDefault(i => i.Id == id);
			if(manifest == null)
				UberDebug.LogChannel(LogChannels.Trade, "Unknown manifest requested from TradeMonitor");

			return manifest;
		}

		public void HandleMessage(string type, MessageArgs args)
		{
			if (type == TradeMessages.TradeAccepted && args != null)
				HandleTradeAccepted(args as TradeCreatedMessageArgs);

			if (type == GameMessages.PreSave)
				HandlePreSave();
		}

		private void HandlePreSave()
		{
			var serialized = _activeManifests.Select(i => i.GetData()).ToList();
			SerializationHub.Instance.AddCollection("TradeMonitor", new TradeMonitorData{ Manifests = serialized });
		}

		private void HandleTradeAccepted(TradeCreatedMessageArgs args)
		{
			if(args == null)
				throw new UnityException("TradeMonitor recieved bad trade message args");

			if(_activeManifests.TrueForAll(i => i.Id != args.TradeManifest.Id))
				_activeManifests.Add(args.TradeManifest);
		}

		public string Name { get { return "TradeMonitor"; } }
	}
}