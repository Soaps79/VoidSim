using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Narrative.Conversations;
using Assets.Narrative.Missions;
using Assets.Narrative.UI;
using Assets.Scripts.Initialization;
using Assets.Scripts.Serialization;
using QGame;
using UnityEngine;

namespace Assets.Narrative.Notifications
{
	public class ConversationsProgressData
	{
		public List<string> Completed;
		public List<string> Queued;
	}

	public class MissionTriggerProgressData
	{
		public string ConversationName;
		public List<string> CompletedMissionNames;
	}

	[RequireComponent(typeof(MissionsMonitor))]
	public class ConversationsMonitor : QScript, ISerializeData<ConversationsProgressData>
	{
		private class MissionStartTriggers
		{
			public Conversation Conversation;
			public Dictionary<string, bool> MissionNames = new Dictionary<string, bool>();
		}

		private readonly List<MissionStartTriggers> _unstartedMissionTriggers = new List<MissionStartTriggers>();
		private NotificationListViewModel _viewModel;
		private List<string> _completedConversationNames = new List<string>();
		private List<string> _queuedConversationNames = new List<string>();
		private readonly Dictionary<string, ConversationTrigger> _levelTriggers 
			= new Dictionary<string, ConversationTrigger>();
		private ConversationsProgressData _data;

		public void InitializeUi(NotificationListViewModel notificationsViewModel)
		{
			_viewModel = notificationsViewModel;
			var missions = GetComponent<MissionsMonitor>();
			missions.OnMissionComplete += CheckTriggersOnMissionComplete;
		}

		private void CheckTriggersOnMissionComplete(Mission mission)
		{
			// tell all triggers the mission has completed
			foreach (var missionTrigger in _unstartedMissionTriggers)
			{
				if (missionTrigger.MissionNames.ContainsKey(mission.Name))
					missionTrigger.MissionNames[mission.Name] = true;
			}

			// if it finishing completed the trigger, start it
			var toStart = _unstartedMissionTriggers.Where(i =>
				!i.MissionNames.ContainsValue(false)).ToList();

			if (!toStart.Any())
				return;

			foreach (var trigger in toStart)
			{
				ActivateConversation(trigger.Conversation);
				_unstartedMissionTriggers.Remove(trigger);
			}
		}

		// create a conversation's UI entry, update serialization lists, hook into OnComplete
		private void ActivateConversation(Conversation conversation)
		{
			_viewModel.AddConversationNotification(conversation);
			_queuedConversationNames.Add(conversation.name);
			conversation.OnComplete += HandleConversationComplete;
		}

		// convo complete, update serialization lists
		private void HandleConversationComplete(Conversation conversation)
		{
			_queuedConversationNames.RemoveAll(i => i == conversation.name);
			_completedConversationNames.Add(conversation.name);
		}

		public void HandleLevelPackage(LevelPackage package, List<string> completedMissions)
		{
			var triggers = package.Conversations;
			triggers.ForEach(i => _levelTriggers.Add(i.Conversation.name, i));

			foreach (var conversationTrigger in triggers)
			{
				switch (conversationTrigger.Type)
				{
					// active
					case ConversationTriggerType.LevelStart:
						HandleLevelStartTrigger(conversationTrigger);
						break;
					case ConversationTriggerType.MissionComplete:
						HandleMissionStartTrigger(conversationTrigger, completedMissions);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		}

		private void HandleLevelStartTrigger(ConversationTrigger conversationTrigger)
		{
			// start these, unless deserialized data says they're already complete
			if(_data == null || !_data.Completed.Contains(conversationTrigger.Conversation.name))
				ActivateConversation(conversationTrigger.Conversation);
		}

		private void HandleMissionStartTrigger(ConversationTrigger conversationTrigger, IEnumerable<string> completedMissions)
		{
			if (_completedConversationNames.Contains(conversationTrigger.Conversation.name))
				return;

			// immediately activate conversation if data says to
			if (_data != null && _data.Queued.Contains(conversationTrigger.Conversation.name))
			{
				ActivateConversation(conversationTrigger.Conversation);
				return;
			}

			// create mission trigger, complete missions if data says to
			var missionTrigger = new MissionStartTriggers()
			{
				Conversation = conversationTrigger.Conversation
			};
			conversationTrigger.Values.ForEach(
				i => missionTrigger.MissionNames.Add(i, completedMissions.Contains(i)));
			_unstartedMissionTriggers.Add(missionTrigger);
		}

		public void SetFromData(ConversationsProgressData data)
		{
			_data = data;
			_completedConversationNames.AddRange(data.Completed);
		}

		public ConversationsProgressData GetData()
		{
			return new ConversationsProgressData
			{
				Completed = _completedConversationNames,
				Queued = _queuedConversationNames
			};
		}
	}
}