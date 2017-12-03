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
	public class NotificationViewModel : QScript
	{
		[SerializeField] private Image _icon;
		public Button Button;
		private CanvasGroup _canvasgroup;

		void Awake()
		{
			_canvasgroup = GetComponent<CanvasGroup>();
			_canvasgroup.alpha = 0;
			_canvasgroup.DOFade(1, .5f);
			Button = GetComponent<Button>();
		}

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