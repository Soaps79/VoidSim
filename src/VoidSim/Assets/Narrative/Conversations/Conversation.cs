using System;
using System.Collections.Generic;
using Assets.Narrative.Missions;
using UnityEngine;

namespace Assets.Narrative.Conversations
{
	[Serializable]
	public class ConversationNode
	{
		public NarrativeActorSO Actor;
		public string Text;
		public bool HasDecision;
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
	public class Conversation : ScriptableObject
	{
		public List<MissionGroupSO> Missions;
		public List<ConversationNode> Nodes;
		public string Title;

		public Action<Conversation> OnComplete;

		public void Complete()
		{
			if (OnComplete != null)
				OnComplete(this);
		}
	}
}