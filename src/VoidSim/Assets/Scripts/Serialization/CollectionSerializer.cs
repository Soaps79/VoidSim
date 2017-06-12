using Messaging;
using Newtonsoft.Json;
#pragma warning disable 1570

namespace Assets.Scripts.Serialization
{
	/// <summary>
	/// An interface to handle collections of data in regards to game save files.
	/// Consumers must implement the ISerializeData<T> interface. It will subscribe to 
	/// the MessageHub to know of saves, and call GetData() on the consumer for them.
	/// The HasData() function also initializes when called the first time.
	/// 
	/// Sample Init:
	/// private readonly CollectionSerializer<PlaceablesMonitorData> _serializer 
	///		= new CollectionSerializer<PlaceablesMonitorData>();
	/// 
	/// Sample Load:
	/// if(_serializer.HasDataFor(this, "Placeables")) HandleLoadingPlaceables();
	/// 
	/// </summary>
	/// <typeparam name="T">Data Type</typeparam>
	public class CollectionSerializer<T> : IMessageListener where T: class
	{
		private ISerializeData<T> _consumer;
		private string _data;
		private string _collectionName;
		private bool _isInitialized;

		/// <summary>
		/// Currently also serves as initialization.
		/// </summary>
		public bool HasDataFor(ISerializeData<T> consumer, string name, bool isStatic = false)
		{
			if (consumer == null || string.IsNullOrEmpty(name))
			{
				UberDebug.LogChannel(LogChannels.Serialization, string.Format("CollectionSerializer given bad data and is quitting, name: {0}", name));
				return false;
			}

			// this function does more than advertised, but until this 
			// becomes a problem, it provides a nice simple interface for consumers
			if (!_isInitialized)
			{
				_consumer = consumer;
				_collectionName = name;
				Name = name + "_serializer";
				MessageHub.Instance.AddListener(this, GameMessages.PreSave, isStatic);
				_isInitialized = true;
			}

			_data = SerializationHub.Instance.GetCollection(name);
			return !string.IsNullOrEmpty(_data);
		}

		/// <summary>
		/// Call this to retrieve data
		/// </summary>
		public T DeserializeData()
		{
			return string.IsNullOrEmpty(_data) ? null : JsonConvert.DeserializeObject<T>(_data);
		}

		// responds to messages by pulling consumer data and passing to SerializationHub
		public void HandleMessage(string type, MessageArgs args)
		{
			if (type == GameMessages.PreSave)
				HandlePreSave();
		}

		private void HandlePreSave()
		{
			var data = _consumer.GetData();
			if (data == null)
				return;

			SerializationHub.Instance.AddCollection(_collectionName, data);
		}

		public string Name { get; private set; }
	}
}