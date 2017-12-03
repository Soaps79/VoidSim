using Assets.Narrative.Missions;
using Assets.Scripts.UI;
using DG.Tweening;
using QGame;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace Assets.Narrative.UI
{
	public class MissionViewModel : QScript
	{
		[SerializeField] private GoalViewModel _goalPrefab;
		[SerializeField] private RectTransform _goalList;
		[SerializeField] private TMP_Text _nameText;
		[SerializeField] private Image _infoIcon;
		[SerializeField] private CanvasGroup _canvasGroup;
		private string _flavorText;

		public void Initialize(Mission mission)
		{
			mission.OnComplete += HandleUpdateComplete;
			foreach (var goal in mission.Goals)
			{
				var instance = Instantiate(_goalPrefab, _goalList.transform, false);
				instance.Initialize(goal);
			}

			var trigger = _infoIcon.GetComponent<BoundTooltipTrigger>();
			if (trigger != null)
			{
				trigger.OnHoverActivate += tooltipTrigger => tooltipTrigger.text =
					mission.FlavorText;
			}

			_nameText.text = mission.DisplayName;
			var canvasGroup = gameObject.GetComponent<CanvasGroup>();
			canvasGroup.alpha = 0;
			canvasGroup.DOFade(1, .5f);
		}

		private void HandleUpdateComplete(Mission mission)
		{
			var image = GetComponent<Image>();
			image.DOFade(0, .75f).OnComplete(() => Destroy(gameObject));
		}
	}
}