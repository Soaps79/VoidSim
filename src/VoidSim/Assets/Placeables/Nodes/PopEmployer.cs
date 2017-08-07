using Assets.Scripts;
using Messaging;

namespace Assets.Placeables.Nodes
{
	public class PopEmployerMessageArgs : MessageArgs
	{
		public PopEmployer PopEmployer;
	}

	public class PopEmployer : PlaceableNode
	{
		public const string MessageName = "PopEmployerCreated";
		public int EmployeeCount;

		public override void BroadcastPlacement()
		{
			Locator.MessageHub.QueueMessage(MessageName, new PopEmployerMessageArgs { PopEmployer = this });
		}

		public override string NodeName { get { return "PopEmployer"; } }
	}
}