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
	    [SerializeField] private ParticleSystem _particleSystem;
	    [SerializeField] private float _waitBeforeFade;
		private string _flavorText;

		public void Initialize(Mission mission)
		{
			mission.OnComplete += HandleMissionComplete;
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
            _canvasGroup.alpha = 0;
            _canvasGroup.DOFade(1.0f, .5f);
        }

		private void HandleMissionComplete(Mission mission)
		{
		    var particles = Instantiate(_particleSystem, transform.parent);
            particles.transform.position = transform.position;
            particles.transform.localScale = Vector3.one;

		    StopWatch.AddNode("", _waitBeforeFade).OnTick += () =>
		    {
		        _canvasGroup.DOFade(0, .75f).OnComplete(() => Destroy(gameObject));
		    };
		}
	}
}