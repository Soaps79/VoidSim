using UnityEngine;

using System.Collections.Generic;
using System.Linq;

namespace Messaging
{
	public interface IMessageHub
	{
		void QueueMessage(string type, MessageArgs args);
		void AddListener(IMessageListener listener, string type);
		void RemoveListener(IMessageListener listener);
		void FireMessage(string type, MessageArgs args);
	}

	internal class NullMessageHub : IMessageHub
	{
		public void QueueMessage(string type, MessageArgs args)
		{
			Debug.Log("NullMessageHub Accessed");
		}

		public void AddListener(IMessageListener listener, string type)
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

	public class MessageHub : IMessageHub
	{
		public const string AllMessages = "AllMessages";

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

		private Dictionary<string, List<IMessageListener>> _listenerList;
		private Queue<KeyValuePair<string, IMessageListener>> _addListenerQueue;
		private List<List<QueuedMessage>> _messageQueue;

		private int _activeQueue;
		private bool _isFiring;
		private List<IMessageListener> _toRemove;

		public MessageHub()
		{
			_listenerList = new Dictionary<string, List<IMessageListener>>();
			_listenerList.Add(AllMessages, new List<IMessageListener>());

			_messageQueue = new List<List<QueuedMessage>> {new List<QueuedMessage>(), new List<QueuedMessage>()};

			_toRemove = new List<IMessageListener>();

			_addListenerQueue = new Queue<KeyValuePair<string, IMessageListener>>();

			_activeQueue = 0;
		}

		public void QueueMessage(string type, MessageArgs args)
		{
			var evt = new QueuedMessage(type, args);
			_messageQueue[_activeQueue].Add(evt);
		}

		public void AddListener(IMessageListener listener, string type)
		{
			if (_isFiring)
			{
				_addListenerQueue.Enqueue(new KeyValuePair<string, IMessageListener>(type, listener));
				return;
			}

			List<IMessageListener> list;
			_listenerList.TryGetValue(type, out list);

			if (list == null)
			{
				list = new List<IMessageListener>();
				_listenerList.Add(type, list);
			}

			if (!list.Contains(listener))
			{
				list.Add(listener);
			}
		}

		public void RemoveListener(IMessageListener listener)
		{
			_toRemove.Add(listener);
		}

		private void RemoveListenerActual()
		{
			var list = new List<string>();

			// for each unsubscribing entity
			foreach (IMessageListener listener in _toRemove)
			{
				// check each list to see if it is subscribed
				foreach (var pair in _listenerList)
				{
					if (pair.Value.Remove(listener))
					{
						// if it was removed, and list is now empty, flag list for removal
						if (pair.Value.Any())
						{
							list.Add(pair.Key);
						}
					}
				}

				// remove all empty lists
				if (list.Any())
				{
					foreach (var type in list)
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

			List<IMessageListener> list;
			_listenerList.TryGetValue(type, out list);

			if (list != null)
			{
				foreach (IMessageListener l in list)
				{
                    l.HandleMessage(type, args);
				}
			}

			if (_listenerList[AllMessages].Count > 0)
			{
				foreach (IMessageListener l in _listenerList[AllMessages])
				{
					l.HandleMessage(type, args);
				}
			}

			_isFiring = false;

			if (_addListenerQueue.Count > 0)
			{
				while (_addListenerQueue.Count > 0)
				{
					KeyValuePair<string, IMessageListener> pair = _addListenerQueue.Dequeue();
					AddListener(pair.Value, pair.Key);
				}
			}
		}
	}
}
