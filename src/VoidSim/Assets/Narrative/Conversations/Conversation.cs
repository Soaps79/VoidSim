﻿using System;
using System.Collections.Generic;
using Assets.Narrative.Missions;
using UnityEngine;

namespace Assets.Narrative.Conversations
{
	[Serializable]
	public class ConversationNodeTransition
	{
		public string ButtonText;
		public ConversationEntry Next;
	}

	[Serializable]
	public class ConversationResultGroup
	{
		
	}

	public interface IConversationResult
	{
		void HandleResult(string result);
	}

	[Serializable]
	public class MissionStartResult : IConversationResult
	{
		public void HandleResult(string result)
		{
			throw new System.NotImplementedException();
		}
	}

	[Serializable]
	public class ConversationEntry
	{
		public ConversationNode Node;
		public List<ConversationNodeTransition> Transitions;
	}

	[Serializable]
	public class Conversation : ScriptableObject
	{
		public string Title;
		public ConversationEntry InitialEntry;
		public Action<Conversation> OnComplete;

		public void Complete()
		{
			if (OnComplete != null)
				OnComplete(this);
		}
	}
}