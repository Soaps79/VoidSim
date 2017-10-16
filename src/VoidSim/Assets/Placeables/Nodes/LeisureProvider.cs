using Assets.Scripts;
using Messaging;

namespace Assets.Placeables.Nodes
{
	public class LeisureProviderMessageArgs : MessageArgs
	{
		public LeisureProvider LeisureProvider;
	}

	public class LeisureProvider : PlaceableNode<LeisureProvider>
	{
	    protected override LeisureProvider GetThis() { return this; }
		public const string MessageName = "LeisureNodeCreated";
		public int AmountProvided;

		public override void BroadcastPlacement()
		{
			Locator.MessageHub.QueueMessage(MessageName, new LeisureProviderMessageArgs { LeisureProvider = this });
		}

		public override string NodeName
		{
			get { return "LeisureProvider"; }
		}
	}
}