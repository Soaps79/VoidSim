using System.Collections.Generic;
using Assets.Narrative.Missions;
using UnityEngine;

namespace Assets.Narrative.Conversations
{
	public class ConversationNode : ScriptableObject
	{
		public NarrativeActorSO Actor;
		public string Text;
		public bool HasDecision;
		public MissionGroupSO Missions;
	}
}