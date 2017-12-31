using System;
using System.Collections.Generic;
using Assets.Scripts;
using Assets.Station.Efficiency;
using Assets.WorldMaterials.Population;
using Messaging;
using QGame;
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

	public class PopEmployer : PlaceableNode<PopEmployer>
	{
	    protected override PopEmployer GetThis() { return this; }
		public const string MessageName = "PopEmployerCreated";
		public int CurrentEmployeeCount;
		public int MaxEmployeeCount;
		[SerializeField] private float _weight = 1.0f;
        [SerializeField] private List<Person> _employees = new List<Person>();
		private EfficiencyAffector _countAffector;
		public bool HasRoom {  get { return MaxEmployeeCount > CurrentEmployeeCount; } }

		[SerializeField] private float _initialDesirability;
		public float EmployeeDesirability { get { return _initialDesirability; } }

		public Action OnEmployeesChanged;


		public override void BroadcastPlacement()
		{
			// hook into efficiency system
			_countAffector = new EfficiencyAffector("Employees");

			var efficiency = GetComponent<EfficiencyNode>();
			efficiency.Module.RegisterAffector(_countAffector);

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
		    CurrentEmployeeCount = _employees.Count;
			_countAffector.Efficiency = (float)CurrentEmployeeCount / MaxEmployeeCount;
			if (OnEmployeesChanged != null)
				OnEmployeesChanged();
		}

	    public void RemoveEmployee(Person person)
	    {
	        if (person.Employer != name)
	            return;

	        person.Employer = string.Empty;
	        _employees.Remove(person);
	    }

		public override string NodeName { get { return "PopEmployer"; } }
	}
}