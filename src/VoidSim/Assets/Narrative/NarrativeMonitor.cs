using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Narrative.Conversations;
using Assets.Narrative.Goals;
using Assets.Narrative.Missions;
using Assets.Narrative.UI;
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
		private readonly List<IGoalTracker> _trackers = new List<IGoalTracker>();
		[SerializeField] private List<Mission> _activeMissions;
		private readonly List<string> _completedMissionNames = new List<string>();
		private readonly List<MissionSO> _unstartedMissions = new List<MissionSO>();

		[SerializeField] private Conversation _initialConversation;
		[SerializeField] private ConversationViewModel _conversationViewModelPrefab;
		private ConversationViewModel _conversationViewModel;

		private readonly CollectionSerializer<MissionGroupProgressData> _serializer
			= new CollectionSerializer<MissionGroupProgressData>();

		[SerializeField] private MissionGroupViewModel _missionViewModelPrefab;
		public Action<Mission> OnMissionBegin;
		private GameObject _canvas;

		void Start()
		{
			// delay to let the game objects get set up
			OnNextUpdate += Initialize;
		}

		private void Initialize(float obj)
		{
			_canvas = GameObject.Find("InfoCanvas");
			InitializeConversations();
			InitializeMissionsUI();
			
			// trackers will be given goals as the missions are begun
			InitializeCraftProductTracker();
			InitializeAccumulateProductTracker();
			InitializeSellProductTracker();
			InitializePlacePlaceableTracker();

			// if there is loading data, bring missions up to date
			if (_serializer.HasDataFor(this, "Narrative"))
				LoadMissions();
		}

		private void InitializeConversations()
		{
			_conversationViewModel = Instantiate(_conversationViewModelPrefab, _canvas.transform, false);
			_conversationViewModel.BeginConversation(_initialConversation);
			_conversationViewModel.OnMissionsNeedStart += ActivateMissionGroup;
		}

		// instantiate and wire up the mission view model
		private void InitializeMissionsUI()
		{
			var viewModel = Instantiate(_missionViewModelPrefab, _canvas.transform, false);
			viewModel.Initialize(this);
		}

		// match progress data with static content
		private void LoadMissions()
		{
			//var data = _serializer.DeserializeData();
			//if(data.Name != _missionGroupSO.name)
			//	throw new UnityException("Mission group progress data does not match Scriptable");

			//_completedMissionNames.AddRange(data.CompletedMissions);
			//foreach (var missionData in data.ActiveMissions)
			//{
			//	// create the mission from the SO
			//	var content = _missionGroupSO.Missions.FirstOrDefault(i => i.name == missionData.Name);
			//	if(content == null)
			//		throw new UnityException("In-progress mission not found in scriptable");
			//	var mission = BeginMission(content);
			//	// progress it to where it was saved
			//	mission.SetProgress(missionData);
			//}
		}

		// start the mission group fresh
		public void ActivateMissionGroup(MissionGroupSO group)
		{
			InitializeMissionGroupProducts(group);
			var toStart = group.Missions.Where(
				i => string.IsNullOrEmpty(i.PrereqMissionName) 
					|| _completedMissionNames.Contains(i.PrereqMissionName)).ToList();
			foreach (var missionSO in toStart)
			{
				BeginMission(missionSO);
			}
			_unstartedMissions.AddRange(group.Missions.Except(toStart));
		}

		// create mission with its static content
		private void BeginMission(MissionSO missionSO)
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
			if (OnMissionBegin != null)
				OnMissionBegin(mission);
		}

		// complete mission, see if any new ones are triggered
		private void HandleMissionComplete(Mission mission)
		{
			_completedMissionNames.Add(mission.Name);
			_activeMissions.Remove(mission);
			_trackers.ForEach(i => i.Prune());

			var next = _unstartedMissions.Where(i => i.PrereqMissionName == mission.Name).ToList();
			if (next.Any())
				next.ForEach(i => BeginMission(i));
		}

		// get static Product data
		// this is bad form, writing to a SO, fix it
		private void InitializeMissionGroupProducts(MissionGroupSO group)
		{
			var allProducts = ProductLookup.Instance.GetProducts();
			foreach (var mission in group.Missions)
			{
				foreach (var productGoal in mission.Goals)
				{
					var product = allProducts.FirstOrDefault(i => i.Name == productGoal.ProductName);
					if (product != null)
						productGoal.ProductId = product.ID;
				}
			}
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

		public MissionGroupProgressData GetData()
		{
			//var data = new MissionGroupProgressData
			//{
			//	Name = _missionGroupSO.name,
			//	ActiveMissions = _activeMissions.Select(i => i.GetData()).ToList(),
			//	CompletedMissions = _completedMissionNames.ToList()
			//};

			return new MissionGroupProgressData();
		}
	}
}