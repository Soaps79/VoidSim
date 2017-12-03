using System.Collections.Generic;
using Assets.Narrative.Missions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Assets.Narrative.Conversations
{
	public class ConversationNode : ScriptableObject
	{
		public NarrativeActorSO Actor;
		[Title("Descriptive Text", bold: false)]
		[HideLabel]
		[MultiLineProperty(10)]
		public string Text;
		public List<MissionSO> Missions;
	}
}