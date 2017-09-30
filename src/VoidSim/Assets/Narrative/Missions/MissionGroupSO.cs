using System.Collections.Generic;
using UnityEngine;

namespace Assets.Narrative.Missions
{
	public class MissionGroupSO : ScriptableObject
	{
		public string DisplayName;
		public List<MissionSO> Missions;
	}
}