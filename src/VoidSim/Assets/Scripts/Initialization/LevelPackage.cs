using System;
using System.Collections.Generic;
using Assets.Narrative.Conversations;
using UnityEngine;

namespace Assets.Scripts.Initialization
{
	public enum ConversationTriggerType
	{
		LevelStart, MissionComplete
	}

	[Serializable]
	public class ConversationTrigger
	{
		public Conversation Conversation;
		public ConversationTriggerType Type;
		public string Value;
	}

	public class LevelPackage : ScriptableObject
	{
		public List<ConversationTrigger> Conversations;
	}
}