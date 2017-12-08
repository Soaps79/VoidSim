using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Narrative.Conversations;
using Assets.Narrative.Goals;
using Assets.Narrative.UI;
using Assets.Scripts;
using Assets.Scripts.Initialization;
using Assets.WorldMaterials.Products;
using QGame;
using UnityEngine;

namespace Assets.Narrative.Missions
{
	// Object to manage the queue of missions available. 
	// Initializes missions when they start, and also handles their completion
	public class MissionsMonitor : QScript
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
			FindMissionsInLevelPackage(package);
			InitializeMissionsUI();

			// trackers will be given goals as the missions are begun
			InitializeCraftProductTracker();
			InitializeAccumulateProductTracker();
			InitializeSellProductTracker();
			InitializePlacePlaceableTracker();
		}

		// uses the Extract function below to walk the conversation trees and grab all of their possible missions
		private void FindMissionsInLevelPackage(LevelPackage package)
		{
			foreach (var conversation in package.Conversations)
			{
				ExtractMissionsFromConversations(conversation.Conversation.InitialEntry);
			}
		}

		private void ExtractMissionsFromConversations(ConversationEntry conversationEntry)
		{
			foreach (var mission in conversationEntry.Node.Missions)
			{
				if(!_levelMissions.ContainsKey(mission.name))
					_levelMissions.Add(mission.name, mission);
			}

			foreach (var nodeTransition in conversationEntry.Transitions)
			{
				ExtractMissionsFromConversations(nodeTransition.Next);
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

			Locator.MessageHub.QueueMessage(Mission.MessageName, 
				new MissionUpdateMessageArgs
				{
					Mission = mission,
					Status = MissionUpdateStatus.Begin
				});
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
	}
}