using Assets.Narrative.Missions;
using QGame;
using UnityEngine;

namespace Assets.Narrative.UI
{
	public class MissionGroupViewModel : QScript
	{
		[SerializeField] private MissionViewModel _missionPrefab;
		[SerializeField] private RectTransform _missionList;

		public void Initialize(NarrativeMonitor monitor)
		{
			monitor.OnMissionBegin += HandleMissionBegin;
		}

		private void HandleMissionBegin(Mission mission)
		{
			var viewModel = Instantiate(_missionPrefab, _missionList.transform, false);
			viewModel.Initialize(mission);
		}
	}
}