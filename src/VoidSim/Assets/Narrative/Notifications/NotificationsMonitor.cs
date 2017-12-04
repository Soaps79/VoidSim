using System.Collections.Generic;
using System.Linq;
using Assets.Narrative.Conversations;
using Assets.Narrative.Missions;
using Assets.Narrative.UI;
using Assets.Scripts.Initialization;
using QGame;
using UnityEngine;

namespace Assets.Narrative.Notifications
{
	[RequireComponent(typeof(MissionsMonitor))]
	public class NotificationsMonitor : QScript
	{
		private class MissionStartTriggers
		{
			public Conversation Conversation;
			public Dictionary<string, bool> MissionNames = new Dictionary<string, bool>();
		}

		private readonly List<MissionStartTriggers> _unstartedMissionTriggers = new List<MissionStartTriggers>();
		private NotificationListViewModel _viewModel;

		public void Initialize(NotificationListViewModel notificationsViewModel)
		{
			_viewModel = notificationsViewModel;
			var missions = GetComponent<MissionsMonitor>();
			missions.OnMissionComplete += CheckTriggersOnMissionComplete;
		}

		private void CheckTriggersOnMissionComplete(Mission obj)
		{
			// tell all triggers the mission has completed
			foreach (var missionTrigger in _unstartedMissionTriggers)
			{
				if (missionTrigger.MissionNames.ContainsKey(obj.Name))
					missionTrigger.MissionNames[obj.Name] = true;
			}

			// if it finishing completed the trigger, start it
			var toStart = _unstartedMissionTriggers.Where(i =>
				!i.MissionNames.ContainsValue(false)).ToList();

			if (!toStart.Any())
				return;

			foreach (var trigger in toStart)
			{
				_viewModel.AddConversationNotification(trigger.Conversation);
				_unstartedMissionTriggers.Remove(trigger);
			}
		}

		public void HandleLevelPackage(LevelPackage package)
		{
			var triggers = package.Conversations;
			foreach (var conversationTrigger in triggers)
			{
				switch (conversationTrigger.Type)
				{
					case ConversationTriggerType.LevelStart:
						_viewModel.AddConversationNotification(conversationTrigger.Conversation);
						break;
					case ConversationTriggerType.MissionComplete:
						var missionTrigger = new MissionStartTriggers()
						{
							Conversation = conversationTrigger.Conversation
						};
						conversationTrigger.Values.ForEach(i => missionTrigger.MissionNames.Add(i, false));
						_unstartedMissionTriggers.Add(missionTrigger);
						break;
				}
			}
		}
	}
}