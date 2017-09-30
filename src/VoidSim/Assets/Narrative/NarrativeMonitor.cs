using System.Collections.Generic;
using System.Linq;
using Assets.Narrative.Goals;
using Assets.Narrative.Missions;
using Assets.Scripts.Serialization;
using Assets.WorldMaterials.Products;
using QGame;
using UnityEngine;

namespace Assets.Narrative
{
	// Saves only what is needed for mission group progress
	public class MissionGroupProgressData
	{
		public string Name;
		public List<MissionProgressData> ActiveMissions;
		public List<string> CompletedMissions;
	}

	/// <summary>
	/// Orchestrate the Narrative. Will probably end up handling too many duties, and jobs will be broken out.
	/// </summary>
	public class NarrativeMonitor : QScript, ISerializeData<MissionGroupProgressData>
	{
		// this is so ugly, extract an interface pls
		private readonly List<ProductGoalTrackerBase> _trackers = new List<ProductGoalTrackerBase>();
		[SerializeField] private MissionGroupSO _missionGroupSO;
		[SerializeField] private List<Mission> _activeMissions;
		private readonly List<string> _completedMissionNames = new List<string>();

		private readonly CollectionSerializer<MissionGroupProgressData> _serializer
			= new CollectionSerializer<MissionGroupProgressData>();

		void Start()
		{
			// delay to let the game objects get set up
			OnNextUpdate += Initialize;
		}

		private void Initialize(float obj)
		{
			// loads necessary static data
			// this is bad form, writing to a SO, fix it
			InitializeProducts();
			
			// trackers will be given goals as the missions are begun
			InitializeCraftProductTracker();
			InitializeAccumulateProductTracker();
			InitializeSellProductTracker();

			// if there is loading data, bring missions up to date
			if (_serializer.HasDataFor(this, "Narrative"))
				LoadMissions();
			else
				CreateStartingMissions();
		}

		// match progress data with static content
		private void LoadMissions()
		{
			var data = _serializer.DeserializeData();
			if(data.Name != _missionGroupSO.name)
				throw new UnityException("Mission group progress data does not match Scriptable");

			_completedMissionNames.AddRange(data.CompletedMissions);
			foreach (var missionData in data.ActiveMissions)
			{
				// create the mission from the SO
				var content = _missionGroupSO.Missions.FirstOrDefault(i => i.name == missionData.Name);
				if(content == null)
					throw new UnityException("In-progress mission not found in scriptable");
				var mission = BeginMission(content);
				// progress it to where it was saved
				mission.SetProgress(missionData);
			}
		}

		// start the mission group fresh
		private void CreateStartingMissions()
		{
			_activeMissions = new List<Mission>();
			foreach (var missionSO in _missionGroupSO.Missions.Where(i => string.IsNullOrEmpty(i.PrereqMissionName)))
			{
				BeginMission(missionSO);
			}
		}

		// create mission with its static content
		private Mission BeginMission(MissionSO missionSO)
		{
			var mission = new Mission
			{
				Name = missionSO.name,
				DisplayName = missionSO.DisplayName,
				FlavorText = missionSO.FlavorText
			};
			foreach (var goalInfo in missionSO.Goals)
			{
				mission.AddAndActivateGoal(new ProductGoal(goalInfo));
			}
			mission.OnComplete += HandleMissionComplete;
			_trackers.ForEach(i => SeeIfTrackerCares(i, mission));
			_activeMissions.Add(mission);
			return mission;
		}

		// complete mission, see if any new ones are triggered
		private void HandleMissionComplete(Mission mission)
		{
			_completedMissionNames.Add(mission.Name);
			_activeMissions.Remove(mission);

			var next = _missionGroupSO.Missions.Where(i => i.PrereqMissionName == mission.Name).ToList();
			if (next.Any())
				next.ForEach(i => BeginMission(i));
		}

		// get static Product data
		private void InitializeProducts()
		{
			var allProducts = ProductLookup.Instance.GetProducts();
			foreach (var mission in _missionGroupSO.Missions)
			{
				foreach (var productGoal in mission.Goals)
				{
					var product = allProducts.FirstOrDefault(i => i.Name == productGoal.ProductName);
					if (product == null)
						throw new UnityException("NarrativeMonitor given bad product");
					productGoal.ProductId = product.ID;
				}
			}
		}

		private void InitializeCraftProductTracker()
		{
			var tracker = new CraftProductTracker();
			KeyValueDisplay.Instance.Add("Make", () => tracker.DisplayString);
			_trackers.Add(tracker);
		}

		private void InitializeAccumulateProductTracker()
		{
			var tracker = new AccumulateProductTracker();
			KeyValueDisplay.Instance.Add("Have", () => tracker.DisplayString);
			_trackers.Add(tracker);
		}

		private void InitializeSellProductTracker()
		{
			var tracker = new SellProductTracker();
			KeyValueDisplay.Instance.Add("Sell", () => tracker.DisplayString);
			_trackers.Add(tracker);
		}

		// finds goals of a tracker's type and hends them off
		private void SeeIfTrackerCares(ProductGoalTrackerBase tracker, Mission mission)
		{
			var goals = mission.Goals.Where(i => i.Type == tracker.GoalType).ToList();
			if (goals.Any())
				goals.ForEach(tracker.AddGoal);
		}

		public MissionGroupProgressData GetData()
		{
			var data = new MissionGroupProgressData
			{
				Name = _missionGroupSO.name,
				ActiveMissions = _activeMissions.Select(i => i.GetData()).ToList(),
				CompletedMissions = _completedMissionNames.ToList()
			};

			return data;
		}
	}
}