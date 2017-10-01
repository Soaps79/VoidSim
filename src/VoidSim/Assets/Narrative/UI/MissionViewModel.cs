using Assets.Narrative.Missions;
using DG.Tweening;
using QGame;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Narrative.UI
{
	public class MissionViewModel : QScript
	{
		[SerializeField] private GoalViewModel _goalPrefab;
		[SerializeField] private RectTransform _goalList;
		[SerializeField] private TMP_Text _nameText;
		private string _flavorText;

		public void Initialize(Mission mission)
		{
			mission.OnComplete += HandleUpdateComplete;
			foreach (var goal in mission.Goals)
			{
				var instance = Instantiate(_goalPrefab, _goalList.transform, false);
				instance.Initialize(goal);
			}

			_flavorText = mission.FlavorText;
			_nameText.text = mission.DisplayName;
		}

		private void HandleUpdateComplete(Mission mission)
		{
			var image = GetComponent<Image>();
			image.DOFade(0, .75f).OnComplete(() => Destroy(gameObject));
		}
	}
}