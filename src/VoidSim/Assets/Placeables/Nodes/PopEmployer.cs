using Assets.Scripts;
using Messaging;
using UnityEngine;

namespace Assets.Placeables.Nodes
{
	public class PopEmployerMessageArgs : MessageArgs
	{
		public PopEmployer PopEmployer;
	}

	/// <summary>
	/// This node allows pop to be assigned to a job
	/// </summary>
	[RequireComponent(typeof(Placeable))]
	[RequireComponent(typeof(EfficiencyNode))]

	public class PopEmployer : PlaceableNode
	{
		public const string MessageName = "PopEmployerCreated";
		public int CurrentEmployeeCount;
		public int MaxEmployeeCount;
		public bool HasRoom {  get { return MaxEmployeeCount > CurrentEmployeeCount; } }

		public override void BroadcastPlacement()
		{
			Locator.MessageHub.QueueMessage(MessageName, new PopEmployerMessageArgs { PopEmployer = this });
		}

		public override string NodeName { get { return "PopEmployer"; } }
	}
}