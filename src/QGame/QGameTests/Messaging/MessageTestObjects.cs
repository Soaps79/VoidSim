using Messaging;
using System.Collections.Generic;

namespace QGameTests
{
	public class ConcreteListener : IMessageListener
	{
		public Dictionary<string, int> MessagesRecieved = new Dictionary<string, int>();

		public string Name { get { return "ConcreteListener"; } }
		public void HandleMessage(string type, MessageArgs args)
		{
			if (!MessagesRecieved.ContainsKey(type))
			{
				MessagesRecieved.Add(type, 0);
			}
			
			MessagesRecieved[type]++;
		}
	}
}