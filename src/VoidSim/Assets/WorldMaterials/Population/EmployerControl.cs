using System.Collections.Generic;
using System.Linq;
using Assets.Placeables.Nodes;
using Assets.Scripts;
using Assets.Scripts.Serialization;
using Assets.Station;
using Assets.Station.Efficiency;
using Messaging;
using QGame;
using UnityEngine;
using TimeLength = Assets.Scripts.TimeLength;

namespace Assets.WorldMaterials.Population
{
	public class EmployerControlData
	{
		public int CurrentUnemployed;
		public List<EmployerData> Employers;
	}

	public class EmployerData
	{
		public string EmployerName;
		public int Count;
	}

	/// <summary>
	/// Handles matching population with employment
	/// </summary>
	public class EmployerControl : QScript, ISerializeData<EmployerControlData>, IMessageListener
	{
		public int CurrentUnemployed;

		[SerializeField] private TimeLength _employmentUpdateTimeLength;
		[SerializeField] private int _employmentUpdateCount;
		private float _baseEmployChance;

		private readonly string _stopwatchNodeName = "employment";
		private readonly List<PopEmployer> _employers = new List<PopEmployer>();
		private readonly List<EmployerData> _deserialized = new List<EmployerData>();

		// passed on to Employers
		private readonly EfficiencyAffector _employerAffector = new EfficiencyAffector("Pop Mood");

		public void Initialize(PopulationSO scriptable, EfficiencyModule module, int currentEmployed)
		{
			CurrentUnemployed = currentEmployed;
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


		public void Deserialize(EmployerControlData deserialized)
		{
			CurrentUnemployed = deserialized.CurrentUnemployed;
			_deserialized.AddRange(deserialized.Employers);
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
			if (CurrentUnemployed <= 0 || !_employers.Any(i => i.HasRoom))
				return;

			// making a queue as basic distribution
			var seeking = _employers.Where(i => i.HasRoom).OrderBy(i => i.EmployeeDesirability).ToList();
			var employers = new Queue<PopEmployer>();
			seeking.ForEach(i => employers.Enqueue(i));

			// for each possible employee
			for (int i = 0; i < _employmentUpdateCount; i++)
			{
				if (CurrentUnemployed < 0 || !employers.Any())
					break;

				// see if they want the job
				var roll = Random.value;
				if (roll > _baseEmployChance)
					continue;

				// pop the employer, give him the worker, add to back if he still has room
				var employer = employers.Dequeue();
				employer.AddEmployee(1);
				CurrentUnemployed -= 1;
				if (employer.HasRoom)
					employers.Enqueue(employer);
			}
		}

		public EmployerControlData GetData()
		{
			return new EmployerControlData
			{
				CurrentUnemployed = CurrentUnemployed,
				Employers = _employers.Select(i => new EmployerData
				{
					EmployerName = i.name,
					Count = i.CurrentEmployeeCount
				}).ToList()
			};
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

			if (_deserialized.Any(i => i.EmployerName == employer.name))
				HandleExistingEmployer(employer);
			else
				HandleNewEmployer(employer);
		}

		private void HandleExistingEmployer(PopEmployer employer)
		{
			var data = _deserialized.First(i => i.EmployerName == employer.name);
			employer.AddEmployee(data.Count);
		}

		private void HandleNewEmployer(PopEmployer employer)
		{
			// real basic implementation of placing employees, place half the max amount
			if (CurrentUnemployed > 0)
			{
				var countToEmploy = employer.MaxEmployeeCount / 2;
				if (CurrentUnemployed > countToEmploy)
				{
					employer.AddEmployee(countToEmploy);
					CurrentUnemployed -= countToEmploy;
				}
				else
				{
					employer.CurrentEmployeeCount = CurrentUnemployed;
					CurrentUnemployed = 0;
				}
			}
		}

		public string Name { get { return "EmployerControl"; } }
	}
}