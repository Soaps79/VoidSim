using System;
using System.Collections.Generic;
using Assets.Scripts;
using Assets.Station.Efficiency;
using Assets.WorldMaterials.Population;
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
	[RequireComponent(typeof(PopContainerSet))]
    public class PopEmployer : PlaceableNode<PopEmployer>
	{
        // basic node needs
	    protected override PopEmployer GetThis() { return this; }
		public const string MessageName = "PopEmployerCreated";

        // currently kept up to date for UI debugging purposes
        public int CurrentEmployeeCount;
		public int MaxEmployeeCount;

	    [SerializeField] private List<Person> _employees = new List<Person>();
		private EfficiencyAffector _countAffector;
		public bool HasRoom {  get { return MaxEmployeeCount > CurrentEmployeeCount; } }

		[SerializeField] private float _initialDesirability;
		public float EmployeeDesirability { get { return _initialDesirability; } }

		public Action OnEmployeesChanged;
	    private PopContainer _container;
	    [SerializeField] private NeedsAffectorList _affectors;

        public override void BroadcastPlacement()
		{
			// hook into efficiency system
			_countAffector = new EfficiencyAffector("Employees");

			var efficiency = GetComponent<EfficiencyNode>();
			efficiency.Module.RegisterAffector(_countAffector);

		    var containers = GetComponent<PopContainerSet>();
		    _container = containers.CreateContainer(new PopContainerParams
		    {
		        Type = PopContainerType.Employment,
		        MaxCapacity = MaxEmployeeCount,
		        Reserved = CurrentEmployeeCount,
		        Affectors = _affectors.Affectors,
                Name = name + "_employer"
		    });

		    Locator.MessageHub.QueueMessage(MessageName, new PopEmployerMessageArgs { PopEmployer = this });
		}

		public void RegisterMood(EfficiencyAffector affector)
		{
			var efficiency = GetComponent<EfficiencyNode>();
			efficiency.Module.RegisterAffector(affector);
		}

		public void AddEmployee(Person person)
		{
		    person.Employer = name;
            _employees.Add(person);
		    UpdateEmployees();
		}

	    private void UpdateEmployees()
	    {
	        CurrentEmployeeCount = _employees.Count;
            _container.SetMaxCapacity(MaxEmployeeCount);
	        _container.SetReserved(CurrentEmployeeCount);
	        _countAffector.Efficiency = (float) CurrentEmployeeCount / MaxEmployeeCount;
	        if (OnEmployeesChanged != null)
	            OnEmployeesChanged();
	    }

	    public void RemoveEmployee(Person person)
	    {
	        if (person.Employer != name)
	            return;

	        person.Employer = string.Empty;
	        _employees.Remove(person);
            UpdateEmployees();
	    }

		public override string NodeName { get { return "PopEmployer"; } }
	}
}