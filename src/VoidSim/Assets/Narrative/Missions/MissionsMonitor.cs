using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Narrative.Goals;
using Assets.Narrative.UI;
using Assets.Scripts;
using Assets.Scripts.Initialization;
using Assets.Scripts.Serialization;
using Assets.WorldMaterials.Products;
using Messaging;
using QGame;
using UnityEngine;

namespace Assets.Narrative.Missions
{
	// Saves only what is needed for mission group progress
	public class MissionsProgressData
	{
		public List<MissionProgressData> ActiveMissions;
		public List<string> CompletedMissions;
		public List<string> UnstartedMissions;
	}

	// Object to manage the queue of missions available. 
	// Initializes missions when they start, and also handles their completion
	public class MissionsMonitor : QScript, ISerializeData<MissionsProgressData>, IMessageListener
	{
		private readonly List<IGoalTracker> _trackers = new List<IGoalTracker>();
		[SerializeField] private List<Mission> _activeMissions;
		private readonly List<string> _completedMissionNames = new List<string>();
		private readonly List<MissionSO> _unstartedMissions = new List<MissionSO>();
		private readonly Dictionary<string, MissionSO> _levelMissions = new Dictionary<string, MissionSO>();

		[SerializeField] private MissionListViewModel _missionViewModelPrefab;
		public Action<Mission> OnMissionBegin;
		private GameObject _canvas;

		public Action<Mission> OnMissionComplete;

		public void Initialize(LevelPackage package)
		{
			_canvas = GameObject.Find("InfoCanvas");
			InitializeTrackers();

			FindLevelMissions();
			InitializeMissionsUI();

            Locator.MessageHub.AddListener(this, Mission.MessageName);
		}

		private void InitializeTrackers()
		{
			// trackers will be given goals as the missions are begun
			InitializeCraftProductTracker();
			InitializeAccumulateProductTracker();
			InitializeSellProductTracker();
			InitializePlacePlaceableTracker();
		}

		// currently loads all missions
        // can be made smarter to load only certain ones in the future
		private void FindLevelMissions()
		{
		    var missions = Resources.LoadAll<MissionSO>("Narrative/Missions");
		    foreach (var missionSO in missions)
		    {
		        _levelMissions.Add(missionSO.name, missionSO);
		    }
		}

		// instantiate and wire up the mission view model
		private void InitializeMissionsUI()
		{
			var viewModel = Instantiate(_missionViewModelPrefab, _canvas.transform, false);
			viewModel.Initialize(this);
		}

		// start the mission group fresh
		public void ActivateMissionGroup(List<MissionSO> group)
		{
			InitializeMissionGroupProducts(group);
			var toStart = group.Where(
				i => string.IsNullOrEmpty(i.PrereqMissionName)
					 || _completedMissionNames.Contains(i.PrereqMissionName)).ToList();
			foreach (var missionSO in toStart)
			{
				BeginMission(missionSO);
			}
			_unstartedMissions.AddRange(group.Except(toStart));
		}

		// get static Product data
		// this is bad form, writing to a SO, fix it
		private void InitializeMissionGroupProducts(IList<MissionSO> group)
		{
			var allProducts = ProductLookup.Instance.GetProducts();
			foreach (var mission in group)
			{
				foreach (var productGoal in mission.Goals)
				{
					var product = allProducts.FirstOrDefault(i => i.Name == productGoal.ProductName);
					if (product != null)
						productGoal.ProductId = product.ID;
				}
			}
		}

		// complete mission, see if any new ones are triggered
		private void HandleMissionComplete(Mission mission)
		{
			_completedMissionNames.Add(mission.Name);
			_activeMissions.Remove(mission);
			_trackers.ForEach(i => i.Prune());

			var toStart = _unstartedMissions.Where(i => i.PrereqMissionName == mission.Name).ToList();
			if (toStart.Any())
				toStart.ForEach(BeginMission);

			if (OnMissionComplete != null)
				OnMissionComplete(mission);

			Locator.MessageHub.QueueMessage(Mission.MessageName,
				new MissionUpdateMessageArgs
				{
					Mission = mission,
					Status = MissionUpdateStatus.Complete
				});
		}

		// create mission with its static content
		private void BeginMission(MissionSO missionSO)
		{
			var mission = ActivateMission(missionSO);

			Locator.MessageHub.QueueMessage(Mission.MessageName, 
				new MissionUpdateMessageArgs
				{
					Mission = mission,
					Status = MissionUpdateStatus.Begin
				});
		}

		private Mission ActivateMission(MissionSO missionSO)
		{
			var mission = new Mission
			{
				Name = missionSO.name,
				DisplayName = missionSO.DisplayName,
				FlavorText = missionSO.FlavorText,
				Scriptable = missionSO
			};
			foreach (var goalInfo in missionSO.Goals)
			{
				mission.AddAndActivateGoal(new ProductGoal(goalInfo));
			}
			mission.OnComplete += HandleMissionComplete;
			_trackers.ForEach(i => SeeIfTrackerCares(i, mission));
			_activeMissions.Add(mission);
			if (OnMissionBegin != null)
				OnMissionBegin(mission);
			return mission;
		}

		private void InitializeCraftProductTracker()
		{
			var tracker = new CraftProductTracker();
			_trackers.Add(tracker);
		}

		private void InitializeAccumulateProductTracker()
		{
			var tracker = new AccumulateProductTracker();
			_trackers.Add(tracker);
		}

		private void InitializeSellProductTracker()
		{
			var tracker = new SellProductTracker();
			_trackers.Add(tracker);
		}

		private void InitializePlacePlaceableTracker()
		{
			var tracker = new PlacePlaceableTracker();
			_trackers.Add(tracker);
		}

		// finds goals of a tracker's type and hands them off
		private void SeeIfTrackerCares(IGoalTracker tracker, Mission mission)
		{
			var goals = mission.Goals.Where(i => i.Type == tracker.GoalType).ToList();
			if (goals.Any())
				goals.ForEach(tracker.AddGoal);
		}

		// match progress data with static content
		public void SetFromData(MissionsProgressData data)
		{
			_completedMissionNames.AddRange(data.CompletedMissions);
			
			foreach (var missionData in data.ActiveMissions)
			{
				// create the mission from the SO
				if(!_levelMissions.ContainsKey(missionData.Name))
					throw new UnityException("In-progress mission not found in scriptable");
				var content = _levelMissions[missionData.Name];
				
				var mission = ResumeMission(content);
				// progress it to where it was saved
				mission.SetProgress(missionData);
			}

			if (data.UnstartedMissions != null)
			{
				foreach (var missionName in data.UnstartedMissions)
				{
					if (!_levelMissions.ContainsKey(missionName))
						throw new UnityException("Unstarted mission not found in scriptable");

					_unstartedMissions.Add(_levelMissions[missionName]);
				}
			}
		}

		private Mission ResumeMission(MissionSO content)
		{
			var mission = ActivateMission(content);
			Locator.MessageHub.QueueMessage(Mission.MessageName,
				new MissionUpdateMessageArgs
				{
					Mission = mission,
					Status = MissionUpdateStatus.Resume
				});
			return mission;
		}

		public MissionsProgressData GetData()
		{
			var data = new MissionsProgressData
			{
				ActiveMissions = _activeMissions.Select(i => i.GetData()).ToList(),
				CompletedMissions = _completedMissionNames.ToList(),
				UnstartedMissions = _unstartedMissions.Select(i => i.name).ToList()
			};
			return data;
		}

		public void HandleMessage(string type, MessageArgs args)
		{
			if (type == Mission.MessageName && args != null)
				HandleMissionUpdate(args as MissionUpdateMessageArgs);
		}

		private void HandleMissionUpdate(MissionUpdateMessageArgs args)
		{
			if(args != null && args.Status == MissionUpdateStatus.RequestBegin && args.MissionSO != null)
				BeginMission(args.MissionSO);
		}

		public string Name { get { return "MissionsMonitor"; } }
	}
}