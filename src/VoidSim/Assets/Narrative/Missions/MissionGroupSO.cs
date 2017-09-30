using System.Collections.Generic;
using UnityEngine;

namespace Assets.Narrative.Missions
{
	/// <summary>
	/// Static representation of mission data for content creation in the editor.
	/// </summary>
	public class MissionGroupSO : ScriptableObject
	{
		public string DisplayName;
		public List<MissionSO> Missions;
	}
}