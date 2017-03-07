namespace Messaging
{
	public interface IMessageListener
	{
		string Name { get; }
		void HandleMessage(string type, MessageArgs args);
	}

	public interface IMessageManager
	{
		// adds an event to be fired on next cycle
		void QueueMessage(string type, MessageArgs args);
		// adds a listener to specified event type
		void AddListener(IMessageListener listener, string type);
		// removes a listener from all events
		void RemoveListener(IMessageListener listener);
		// immediately fires an event
		void FireEvent(string type, MessageArgs args);
	}
}