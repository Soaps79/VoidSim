using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using Assets.Scripts.Serialization;
using Messaging;
using Newtonsoft.Json;
using QGame;
using UnityEngine;

namespace Assets.WorldMaterials.Trade
{
	public class TradeMonitorData
	{
		public List<TradeManifestData> Manifests;
	}

	public class TradeMonitor : SingletonBehavior<TradeMonitor>, IMessageListener, ISerializeData<TradeMonitorData>
	{
		private readonly List<TradeManifest> _activeManifests = new List<TradeManifest>();

		private readonly CollectionSerializer<TradeMonitorData> _serializer
			= new CollectionSerializer<TradeMonitorData>();

		public Action<TradeManifest> OnTradeComplete;

		void Start()
		{
			OnEveryUpdate += UpdateTrades;
			Locator.MessageHub.AddListener(this, TradeMessages.TradeAccepted, true);

			if (_serializer.HasDataFor(this, "TradeMonitor", true))
				HandleGameLoad();
		}

		private void HandleGameLoad()
		{
			_activeManifests.Clear();
			var data = _serializer.DeserializeData();
			foreach (var manifestData in data.Manifests)
			{
				_activeManifests.Add(new TradeManifest(manifestData));
			}
		}

		private void UpdateTrades(float delta)
		{
			if (!_activeManifests.Any())
				return;

			var complete = _activeManifests.Where(i => i.Status == TradeStatus.Complete).ToList();
			if (!complete.Any())
				return;

			if (OnTradeComplete != null)
			{
				complete.ForEach(i => OnTradeComplete(i));
			}
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
		}

		private void HandleTradeAccepted(TradeCreatedMessageArgs args)
		{
			if(args == null)
				throw new UnityException("TradeMonitor recieved bad trade message args");

			if(_activeManifests.TrueForAll(i => i.Id != args.TradeManifest.Id))
				_activeManifests.Add(args.TradeManifest);
		}

		public string Name { get { return "TradeMonitor"; } }
		public TradeMonitorData GetData()
		{
			return new TradeMonitorData { Manifests = _activeManifests.Select(i => i.GetData()).ToList() };
		}
	}
}