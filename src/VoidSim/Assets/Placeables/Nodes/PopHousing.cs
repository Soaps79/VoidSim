using Assets.Scripts;
using Assets.WorldMaterials.Population;
using Messaging;
using UnityEngine;

namespace Assets.Placeables.Nodes
{
    public interface IPopResidence
    {
        int CurrentCapacity { get; }
        int CurrentResidents { get; }
        void AddResidents(int count);
    }
    
	public class PopHousingMessageArgs : MessageArgs
	{
		public PopHousing PopHousing;
	}

	/// <summary>
	/// Population resides here when not assigned to a job
	/// </summary>
	[RequireComponent(typeof(Placeable))]
	public class PopHousing : PlaceableNode<PopHousing>, IPopResidence
	{
	    protected override PopHousing GetThis() { return this; }
		public override string NodeName { get { return "PopHousing"; } }
		public const string MessageName = "PopHousingCreated";
		[SerializeField] private int _initialValue;

		public int CurrentCapacity { get; private set; }
	    public int CurrentResidents { get; private set; }

	    void Awake()
		{
			CurrentCapacity = _initialValue;
		}

		public override void BroadcastPlacement()
		{
			Locator.MessageHub.QueueMessage(MessageName, new PopHousingMessageArgs { PopHousing = this });
		}

	    //public int MaxCapacity { get; private set; }
	    public void AddResidents(int count)
	    {
	        throw new System.NotImplementedException();
	    }
	}
}