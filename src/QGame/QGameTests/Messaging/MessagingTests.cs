using Microsoft.VisualStudio.TestTools.UnitTesting;

using Messaging;

namespace QGameTests
{
	[TestClass]
	public class MessagingTests
	{
		[TestMethod]
		public void AddMessage_CorrectCount()
		{
			var hub = new MessageHub();
			hub.QueueMessage("type", null);
			hub.QueueMessage("type", null);

			Assert.AreEqual(hub.Count, 2);
		}
		 
		[TestMethod]
		public void MessageHub_BroadcastOneMessageToOneListener()
		{
			const string type = "type";
			var hub = new MessageHub();
			var listener = new ConcreteListener();
			hub.AddListener(listener, type);

			hub.QueueMessage(type, null);
			
			hub.Update();
			hub.Update();

			Assert.AreEqual(listener.MessagesRecieved[type], 1);

		}
	}
}