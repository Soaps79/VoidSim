using System.Collections.Generic;
using System.Linq;
using Assets.Placeables.Nodes;
using Assets.Scripts;
using Assets.Scripts.Serialization;
using Assets.Station.Efficiency;
using Assets.WorldMaterials.Population;
using Messaging;
using QGame;
using UnityEngine;
using TimeLength = Assets.Scripts.TimeLength;

namespace Assets.Station.Population
{
	/// <summary>
	/// Handles matching population with employment
	/// </summary>
	public class PopEmployerMonitor : QScript, IMessageListener, IPopMonitor
	{
        // maintain these values for editor debugging
		public int CurrentUnemployed;
	    public int CurrentCapacity;

		[SerializeField] private TimeLength _employmentUpdateTimeLength;
		[SerializeField] private int _employmentUpdateCount;
		private float _baseEmployChance;

		private readonly string _stopwatchNodeName = "employment";
		private readonly List<PopEmployer> _employers = new List<PopEmployer>();
		private readonly List<Person> _unemployed = new List<Person>();
	    private readonly List<Person> _deserialized = new List<Person>();

        // passed on to Employers
        private readonly EfficiencyAffector _employerAffector = new EfficiencyAffector("Pop Mood");

	    private List<Person> _allPopulation;

	    public void Initialize(PopulationControl control, PopulationSO scriptable, EfficiencyModule module)
		{
		    _allPopulation = control.AllPopulation;
			
			InitializeEmploymentUpdate(scriptable.EmploymentParams);
			Locator.MessageHub.AddListener(this, PopEmployer.MessageName);
			if(module == null)
				throw new UnityException("EmployerControl recieved bad module");

			module.OnValueChanged += HandleMoodChange;
		}

		private void HandleMoodChange(EfficiencyModule module)
		{
			_employerAffector.Efficiency = module.CurrentAmount;
		}

		// register with stopwatch to regularly check for updates
		private void InitializeEmploymentUpdate(EmploymentParams @params)
		{
			_employmentUpdateTimeLength = @params.EmploymentUpdateTimeLength;
			_employmentUpdateCount = @params.EmploymentUpdateCount;
			_baseEmployChance = @params.BaseEmployChance;

			var time = Locator.WorldClock.GetSeconds(_employmentUpdateTimeLength);
			var node = StopWatch.AddNode(_stopwatchNodeName, time);
			node.OnTick += HandleEmploymentUpdate;
		}

		private void HandleEmploymentUpdate()
		{
			if (!_unemployed.Any() || !_employers.Any(i => i.HasRoom))
				return;

			// making a queue as basic distribution
			var seeking = _employers.Where(i => i.HasRoom).OrderBy(i => i.EmployeeDesirability).ToList();
			var employers = new Queue<PopEmployer>();
			seeking.ForEach(i => employers.Enqueue(i));

			// for each possible employee
			for (int i = 0; i < _employmentUpdateCount; i++)
			{
				if (_unemployed.Count <= 0 || !employers.Any())
					break;

				// see if they want the job
				var roll = Random.value;
				if (roll > _baseEmployChance)
					continue;

				// pop the employer, give him the worker, add to back if he still has room
				var employer = employers.Dequeue();
			    var employee = _unemployed.First();
				employer.AddEmployee(employee);
			    _unemployed.Remove(employee);
				if (employer.HasRoom)
					employers.Enqueue(employer);
			}
		    UpdateCounts();
		}

	    private void UpdateCounts()
	    {
	        CurrentUnemployed = _unemployed.Count;
	        CurrentCapacity = _employers.Sum(i => i.MaxEmployeeCount);
	    }

        public void HandleMessage(string type, MessageArgs args)
		{
			if (type == PopEmployer.MessageName && args != null)
				HandleEmployerPlacement(args as PopEmployerMessageArgs);
		}

		private void HandleEmployerPlacement(PopEmployerMessageArgs args)
		{
			if (args == null || args.PopEmployer == null)
			{
				Debug.Log("PopulationControl given bad employer message args.");
				return;
			}

			var employer = args.PopEmployer;
			employer.RegisterMood(_employerAffector);
			_employers.Add(employer);

			if (_deserialized.Any(i => i.Employer == employer.name))
				HandleExistingEmployer(employer);
			else
				HandleNewEmployer(employer);

			employer.OnRemove += HandleEmployerRemove;
            UpdateCounts();
		}

		private void HandleEmployerRemove(PopEmployer obj)
		{
			if(_employers.Remove(obj))
                UpdateCounts();
		}

		private void HandleExistingEmployer(PopEmployer employer)
		{
		    var employees = _deserialized.Where(i => i.Employer == employer.name).ToList();
            employees.ForEach(employer.ResumeEmployee);
		    _deserialized.RemoveAll(i => employees.Contains(i));
		}

		private void HandleNewEmployer(PopEmployer employer)
		{
			// real basic implementation of placing employees, place half the max amount
			if (_unemployed.Count > 0)
			{
				var countToEmploy = employer.MaxEmployeeCount / 2;
				if (_unemployed.Count > countToEmploy)
				{
				    var employees = _unemployed.Take(countToEmploy).ToList();
                    employees.ForEach(employer.AddEmployee);
                    _unemployed.RemoveRange(0, countToEmploy);
				}
				else
				{
                    _unemployed.ForEach(employer.AddEmployee);
                    _unemployed.Clear();
				}
			}
		}

		public string Name { get { return "EmployerControl"; } }
	    public void HandlePopulationUpdate(List<Person> people, bool wasAdded)
	    {
	        var unemployed = people.Where(i => string.IsNullOrEmpty(i.Employer));
            _unemployed.AddRange(unemployed);

	        var employed = people.Except(unemployed);
	        foreach (var person in employed)
	        {
	            var employer = _employers.FirstOrDefault(i => i.name == person.Employer);
                if(employer != null)
                    employer.AddEmployee(person);
	        }
            UpdateCounts();
	    }

	    public void HandleDeserialization(List<Person> people)
	    {
	        _unemployed.AddRange(people.Where(i => string.IsNullOrEmpty(i.Employer)));
            _deserialized.AddRange(people.Except(_unemployed));
	    }
	}
}