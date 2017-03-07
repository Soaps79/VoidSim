using System;
using System.Collections.Generic;
using Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace QGame.Test.Messaging
{
    [TestClass]
    public class MessagingTests
    {
        private class ConcreteListener : IMessageListener
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
