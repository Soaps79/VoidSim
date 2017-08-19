using Assets.Scripts;
using Messaging;
using UnityEngine;

namespace Assets.Placeables.Nodes
{
	public class PopHousingMessageArgs : MessageArgs
	{
		public PopHousing PopHousing;
	}

	/// <summary>
	/// Population resides here when not assigned to a job
	/// </summary>
	[RequireComponent(typeof(Placeable))]
	public class PopHousing : PlaceableNode
	{
		public override string NodeName { get { return "PopHousing"; } }
		public const string MessageName = "PopHousingCreated";
		[SerializeField] private int _initialValue;

		public int Capacity { get; private set; }
		public int LeisureProvided;

		void Awake()
		{
			Capacity = _initialValue;
		}

		public override void BroadcastPlacement()
		{
			Locator.MessageHub.QueueMessage(MessageName, new PopHousingMessageArgs { PopHousing = this });
		}
	}
}