using Assets.Scripts;
using Assets.WorldMaterials.Population;
using Messaging;
using UnityEngine;

namespace Assets.Placeables.Nodes
{
	public class LeisureProviderMessageArgs : MessageArgs
	{
		public LeisureProvider LeisureProvider;
	}

	[RequireComponent(typeof(PopContainerSet))]
	public class LeisureProvider : PlaceableNode<LeisureProvider>
	{
		protected override LeisureProvider GetThis() { return this; }
		public const string MessageName = "LeisureNodeCreated";
		public int AmountProvided;
		[SerializeField] private ContainerGenerationParams _containerGenerationParams;
		private PopContainer _container;
		public int CurrentCapacity;

	    public override void Initialize(PlaceableData data)
	    {
	        var containers = GetComponent<PopContainerSet>();
	        _container = containers.CreateContainer(new PopContainerParams
	        {
	            Type = PopContainerType.Fulfillment,
	            MaxCapacity = CurrentCapacity,
	            Affectors = _containerGenerationParams.Affectors,
	            PlaceableName = InstanceName,
	            ActivityPrefix = _containerGenerationParams.ActivityPrefix
	        });
        }

	    public override void BroadcastPlacement()
		{
			Locator.MessageHub.QueueMessage(MessageName, new LeisureProviderMessageArgs { LeisureProvider = this });
		}

		public override string NodeName { get { return "LeisureProvider"; } }
	}
}