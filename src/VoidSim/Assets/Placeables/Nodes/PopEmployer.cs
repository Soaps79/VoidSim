using System;
using System.Collections.Generic;
using Assets.Scripts;
using Assets.Station.Efficiency;
using Assets.WorldMaterials.Population;
using Messaging;
using UnityEngine;
#pragma warning disable 649

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

	    [SerializeField] private readonly List<Person> _employees = new List<Person>();
		private EfficiencyAffector _countAffector;
		public bool HasRoom {  get { return MaxEmployeeCount > CurrentEmployeeCount; } }

		[SerializeField] private float _initialDesirability;
		public float EmployeeDesirability { get { return _initialDesirability; } }

		public Action OnEmployeesChanged;
	    private PopContainer _container;
	    [SerializeField] private ContainerGenerationParams _containerGenerationParams;

	    public override void Initialize(PlaceableData data)
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
	            Affectors = _containerGenerationParams.Affectors,
	            PlaceableName = name,
	            ActivityPrefix = _containerGenerationParams.ActivityPrefix
	        });
        }

	    public override void BroadcastPlacement()
		{
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
            _container.AddReserved(person);
		    UpdateEmployees();
		}

	    public void ResumeEmployee(Person person)
	    {
	        _employees.Add(person);
	        UpdateEmployees();
        }

	    private void UpdateEmployees()
	    {
	        CurrentEmployeeCount = _employees.Count;
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
	        _container.RemoveReserved(person);
            UpdateEmployees();
	    }

		public override string NodeName { get { return "PopEmployer"; } }
	}
}