using UnityEngine;

using System.Collections.Generic;
using System.Linq;
using QGame;

namespace Messaging
{
	public interface IMessageHub
	{
		void QueueMessage(string type, MessageArgs args);
		void AddListener(IMessageListener listener, string type, bool ignoresClear = false);
		void RemoveListener(IMessageListener listener);
		void FireMessage(string type, MessageArgs args);
	}

	internal class NullMessageHub : IMessageHub
	{
		public void QueueMessage(string type, MessageArgs args)
		{
			Debug.Log("NullMessageHub Accessed");
		}

		public void AddListener(IMessageListener listener, string type, bool ignoresClear = false)
		{
			Debug.Log("NullMessageHub Accessed");
		}

		public void RemoveListener(IMessageListener listener)
		{
			Debug.Log("NullMessageHub Accessed");
		}

		public void FireMessage(string type, MessageArgs args)
		{
			Debug.Log("NullMessageHub Accessed");
		}
	}

	public class MessageHub : SingletonBehavior<MessageHub>, IMessageHub
	{
		public const string AllMessages = "AllMessages";

		private class Entry
		{
			public IMessageListener Listener;
			public bool IgnoresClear;
		}

		private struct QueuedMessage
		{
			public string type;
			public MessageArgs args;

			public QueuedMessage(string typ, MessageArgs arg)
			{
				type = typ;
				args = arg;
			}
		}
		public int Count
		{
			get { return _messageQueue[_activeQueue].Count; }
		}

		private Dictionary<string, List<Entry>> _listenerList;
		private Queue<KeyValuePair<string, Entry>> _addListenerQueue;
		private List<List<QueuedMessage>> _messageQueue;

		private int _activeQueue;
		private bool _isFiring;
		private List<IMessageListener> _toRemove;

		public MessageHub()
		{
			_listenerList = new Dictionary<string, List<Entry>>();
			_listenerList.Add(AllMessages, new List<Entry>());

			_messageQueue = new List<List<QueuedMessage>> {new List<QueuedMessage>(), new List<QueuedMessage>()};

			_toRemove = new List<IMessageListener>();

			_addListenerQueue = new Queue<KeyValuePair<string, Entry>>();

			_activeQueue = 0;
		}

		public void QueueMessage(string type, MessageArgs args)
		{
			var evt = new QueuedMessage(type, args);
			_messageQueue[_activeQueue].Add(evt);
		}

		public void AddListener(IMessageListener listener, string type, bool ignoresClear = false)
		{
			if (_isFiring)
			{
				_addListenerQueue.Enqueue(new KeyValuePair<string, Entry>(type, new Entry{ Listener = listener, IgnoresClear = ignoresClear} ));
				return;
			}

			List<Entry> list;
			_listenerList.TryGetValue(type, out list);

			if (list == null)
			{
				list = new List<Entry>();
				_listenerList.Add(type, list);
			}

			if (list.All(i => i.Listener != listener))
			{
				list.Add(new Entry{ Listener = listener, IgnoresClear = ignoresClear });
			}
		}

		public void ClearListeners()
		{
			var toRemove = new List<string>();
			foreach (var valuePair in _listenerList)
			{
				if(valuePair.Value.RemoveAll(i => !i.IgnoresClear) > 0
					&& !valuePair.Value.Any())
					toRemove.Add(valuePair.Key);
			}

			toRemove.ForEach(i => _listenerList.Remove(i));
		}

		public void RemoveListener(IMessageListener listener)
		{
			_toRemove.Add(listener);
		}

		private void RemoveListenerActual()
		{
			var listsToRemove = new List<string>();

			// for each unsubscribing entity
			foreach (IMessageListener listener in _toRemove)
			{
				// check each list to see if it is subscribed
				foreach (var pair in _listenerList)
				{
					if (pair.Value.RemoveAll(i => i.Listener == listener) > 0)
					{
						// if it was removed, and list is now empty, flag list for removal
						if (pair.Value.Any())
						{
							listsToRemove.Add(pair.Key);
						}
					}
				}

				// remove all empty lists
				if (listsToRemove.Any())
				{
					foreach (var type in listsToRemove)
					{
						_listenerList.Remove(type);
					}
				}
			}
		}

		public void Update()
		{
			if (_toRemove.Count > 0)
			{
				RemoveListenerActual();
			}

			// switch activeQueues so list will not keep getting larger as
			// events spawn other events
			int indexQueue = _activeQueue;
			_activeQueue ^= 1;

			if (_messageQueue[indexQueue].Count <= 0)
			{
				return;
			}

			foreach (QueuedMessage pair in _messageQueue[indexQueue])
			{
				FireMessage(pair.type, pair.args);
			}

			_messageQueue[indexQueue].Clear();
		}

		public void FireMessage(string type, MessageArgs args)
		{
			_isFiring = true;

			List<Entry> list;
			_listenerList.TryGetValue(type, out list);

			if (list != null)
			{
				foreach (Entry entry in list)
				{
                    entry.Listener.HandleMessage(type, args);
				}
			}

			if (_listenerList[AllMessages].Count > 0)
			{
				foreach (Entry entry in _listenerList[AllMessages])
				{
					entry.Listener.HandleMessage(type, args);
				}
			}

			_isFiring = false;

			if (_addListenerQueue.Count > 0)
			{
				while (_addListenerQueue.Count > 0)
				{
					var pair = _addListenerQueue.Dequeue();
					AddListener(pair.Value.Listener, pair.Key, pair.Value.IgnoresClear);
				}
			}
		}
	}
}
