using System;
using System.Collections.Generic;
using Assets.Scripts;
using Assets.WorldMaterials.Population;
using Messaging;
using UnityEngine;
// ReSharper disable FieldCanBeMadeReadOnly.Local

namespace Assets.Placeables.Nodes
{
    public interface IPlaceableNode2<T> where T : PlaceableNode<T>
    {
        Action<T> OnRemove { get; set; }
    }

    // PopHome is the concept of where a Person lives when they are on the station
    // Residents will live in housing, and Visitors will live in lodging

    // Not sure that this will be needed... would have implemented anyway, 
    // but OnRemove coming from base PlaceableNode is more annoying to tie into than I want to deal with right now
    public interface IPopHome
    {
        int CurrentCapacity { get; }
        int CurrentCount { get; }
        void AddResident(Person person);
        //Action<IPopHome> OnRemove { get; }
    }
    
	public class PopHousingMessageArgs : MessageArgs
	{
		public PopHousing PopHome;
	}

	/// <summary>
	/// Population resides here when not assigned to a job
	/// </summary>
	[RequireComponent(typeof(Placeable))]
	[RequireComponent(typeof(PopContainerSet))]
    public class PopHousing : PlaceableNode<PopHousing>, IPopHome
	{
	    protected override PopHousing GetThis() { return this; }
		public override string NodeName { get { return "PopHousing"; } }
		public const string MessageName = "PopHousingCreated";
	    public bool IsForResidents;
		[SerializeField] private int _initialValue;
	    [SerializeField] private ContainerGenerationParams _containerGenerationParams;
        [SerializeField] private List<Person> _housed = new List<Person>();
	    private PopContainer _container;

	    public int CurrentCapacity { get; private set; }
	    public int CurrentCount { get; private set; }

	    void Awake()
		{
			CurrentCapacity = _initialValue;
		    CurrentCount = 0;
		}

	    public override void Initialize(PlaceableData data)
	    {
	        var containers = GetComponent<PopContainerSet>();
	        _container = containers.CreateContainer(new PopContainerParams
	        {
	            Type = _containerGenerationParams.Type,
	            MaxCapacity = CurrentCapacity,
	            Reserved = CurrentCount,
	            Affectors = _containerGenerationParams.Affectors,
	            PlaceableName = name,
	            ActivityPrefix = _containerGenerationParams.ActivityPrefix
	        });
        }

	    public override void BroadcastPlacement()
		{
			Locator.MessageHub.QueueMessage(MessageName, new PopHousingMessageArgs { PopHome = this });
		}

        // tells the person their new home and saves a ref to them
	    public void AddResident(Person person)
	    {
	        if (!_housed.Contains(person))
	        {
	            person.Home = name;
	            _housed.Add(person);
	            CurrentCount = _housed.Count;
                _container.AddReserved(person);
	        }
	    }
	}
}