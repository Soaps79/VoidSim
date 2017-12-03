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
		[SerializeField] private Conversation _initialConversation;
		[SerializeField] private ConversationViewModel _conversationViewModelPrefab;
		private ConversationViewModel _conversationViewModel;

		private readonly CollectionSerializer<MissionGroupProgressData> _serializer
			= new CollectionSerializer<MissionGroupProgressData>();

		private GameObject _canvas;
		private MissionsMonitor _missionsMonitor;


		void Start()
		{
			// delay to let the game objects get set up
			OnNextUpdate += Initialize;
			_missionsMonitor = gameObject.GetComponent<MissionsMonitor>();
		}

		private void Initialize(float obj)
		{
			_canvas = GameObject.Find("InfoCanvas");
			_missionsMonitor.Initialize();

			InitializeConversations();

			// if there is loading data, bring missions up to date
			if (_serializer.HasDataFor(this, "Narrative"))
				LoadMissions();
		}

		private void InitializeConversations()
		{
			_conversationViewModel = Instantiate(_conversationViewModelPrefab, _canvas.transform, false);
			_conversationViewModel.BeginConversation(_initialConversation);
			_conversationViewModel.OnMissionsNeedStart += _missionsMonitor.ActivateMissionGroup;
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