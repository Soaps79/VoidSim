using Assets.Scripts.Serialization;
using Messaging;
using QGame;

namespace Assets.Scripts.Initialization
{
	public static class ServiceInitializer
	{
		/// <summary>
		/// Will initialize any services unknown to the Locator
		/// </summary>
		public static void Initialize()
		{
			InitializeMessaging();
			InitializeSerializationHub();
			InitializeLastIds();
		}

		private static void InitializeMessaging()
		{
			var existing = Locator.MessageHub;
			if (existing == null)
				ServiceLocator.Register<IMessageHub>(new MessageHub());
		}

		private static void InitializeSerializationHub()
		{
			var existing = Locator.Serialization;
			if(existing == null)
				ServiceLocator.Register<ISerializationHub>(new SerializationHub());
		}

		private static void InitializeLastIds()
		{
			var existing = Locator.LastId;
			if(existing == null)
				ServiceLocator.Register<ILastIdManager>(new LastIdManager());
		}
	}
}