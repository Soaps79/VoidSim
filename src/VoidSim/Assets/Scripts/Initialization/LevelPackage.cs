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
		public DialogueChain Conversation;
		public ConversationTriggerType Type;
		public List<string> Values;
	}

	public class LevelPackage : ScriptableObject
	{
		public List<ConversationTrigger> Conversations;
	}
}