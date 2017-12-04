using Assets.Narrative.Notifications;
using DG.Tweening;
using QGame;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace Assets.Narrative.UI
{
	[RequireComponent(typeof(CanvasGroup))]
	[RequireComponent(typeof(Button))]
	[RequireComponent(typeof(BoundTooltipTrigger))]
	// currently a pretty simple class wiring up some UI fields
	// mainly driven by its parent list view model
	public class NotificationViewModel : QScript
	{
		[SerializeField] private Image _icon;
		[SerializeField] private Image _highlight;
		public Button Button;
		private CanvasGroup _canvasgroup;

		void Awake()
		{
			_canvasgroup = GetComponent<CanvasGroup>();
			_canvasgroup.alpha = 0;
			_canvasgroup.DOFade(1, .5f);
			Button = GetComponent<Button>();

			var color = _highlight.color;
			var sequence = DOTween.Sequence();
			sequence.PrependInterval(.5f);
			sequence.Append(_highlight.DOFade(.5f, .5f));
			sequence.Append(_highlight.DOFade(0, .5f));
			sequence.SetLoops(5);
			sequence.OnComplete(() => _highlight.color = color);
		}

		// setup sprite and tooltip if they are provided
		public void Initialize(Notification notification)
		{
			if (notification.IconSprite != null)
				_icon.sprite = notification.IconSprite;
			else
				_icon.gameObject.SetActive(false);

			if (!string.IsNullOrEmpty(notification.TooltipText))
			{
				var trigger = GetComponent<BoundTooltipTrigger>();
				trigger.text = notification.TooltipText;
			}
		}

		public void Die()
		{
			_canvasgroup.DOFade(0, .5f).OnComplete(() => Destroy(gameObject));
		}
	}
}