using System;
using System.Collections.Generic;
using Assets.Narrative.Goals;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Assets.Narrative.Missions
{
	[Serializable]
	public class ProductGoalInfo
	{
		public GoalType Type;
		public string ProductName;
		public int TotalAmount;
		public bool NeedsPlacement;
		[HideInInspector] public int ProductId;
	}

	/// <summary>
	/// Static representation of mission data for content creation in the editor.
	/// </summary>
	[Serializable]
	public class MissionSO : ScriptableObject
	{
		public string DisplayName;
		[Title("Tooltip Text", bold: false)]
		[HideLabel]
		[MultiLineProperty(6)]
		public string FlavorText;
		public string PrereqMissionName;
		public List<ProductGoalInfo> Goals;
	}
}