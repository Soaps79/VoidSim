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

    // TODO: Does this really have to be a singleton?
	public class TradeMonitor : SingletonBehavior<TradeMonitor>, IMessageListener, ISerializeData<TradeMonitorData>
	{
		private readonly List<TradeManifest> _activeManifests = new List<TradeManifest>();
	    private readonly List<TradeManifest> _deserializedManifests = new List<TradeManifest>();

        private readonly CollectionSerializer<TradeMonitorData> _serializer
			= new CollectionSerializer<TradeMonitorData>();

		public Action<TradeManifest> OnTradeComplete;
	    private readonly Dictionary<string, ProductTrader> _traders = new Dictionary<string, ProductTrader>();

	    void Start()
		{
			OnEveryUpdate += UpdateTrades;
			Locator.MessageHub.AddListener(this, TradeMessages.TradeAccepted, true);
		    Locator.MessageHub.AddListener(this, TradeMessages.TraderCreated, true);

            if (_serializer.HasDataFor(this, "TradeMonitor", true))
                OnNextUpdate += HandleGameLoad;
		}

		private void HandleGameLoad(float f)
		{
			_activeManifests.Clear();
			var data = _serializer.DeserializeData();
			foreach (var manifestData in data.Manifests)
			{
			    var manifest = new TradeManifest(manifestData);
                _activeManifests.Add(manifest);
                _deserializedManifests.Add(manifest);
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
		    else if (type == TradeMessages.TraderCreated && args != null)
		        HandleTraderCreated(args as TraderInstanceMessageArgs);
        }

        // primarily to handle deserialization
	    private void HandleTraderCreated(TraderInstanceMessageArgs args)
	    {
	        if (!_deserializedManifests.Any())
	            return;

            if (args == null || args.Trader == null)
                throw new UnityException("TradeMonitor given bad message args");

            // find deserialized, if any
            var needsDeserialization = _deserializedManifests.Where(
                    i => i.Provider == args.Trader.ClientName || i.Consumer == args.Trader.ClientName).ToList();
	        if (!needsDeserialization.Any())
	            return;

            // tell the trader about them
	        foreach (var manifest in needsDeserialization)
	        {
	            args.Trader.HandleResume(manifest);
	        }
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