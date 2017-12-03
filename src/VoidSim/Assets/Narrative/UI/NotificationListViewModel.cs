using System.Collections.Generic;
using Assets.Narrative.Conversations;
using Assets.Narrative.Missions;
using QGame;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Assets.Narrative.UI
{
	public class NotificationListViewModel : SerializedMonoBehaviour
	{
		[SerializeField] private NotificationViewModel _buttonPrefab;
		[SerializeField] private RectTransform _contentHolder;

		[DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.Foldout)]
		public Dictionary<NotificationType, Sprite> _sprites;

		private ConversationViewModel _conversationViewModel;

		public void Initialize(ConversationViewModel conversation)
		{
			_conversationViewModel = conversation;
		}

		public void AddConversationNotification(Conversation convo)
		{
			var viewModel = Instantiate(_buttonPrefab, _contentHolder.transform, false);
			const NotificationType type = NotificationType.ConversationStart;
			var notification = new Notification
			{
				Type = type,
				IconSprite = _sprites.ContainsKey(type) ? _sprites[type] : null,
				TooltipText = convo.Title
			};
			viewModel.Initialize(notification);
			viewModel.Button.onClick.AddListener(() => _conversationViewModel.BeginConversation(convo));
			convo.OnComplete += conversation => { viewModel.Die(); };
		}
	}
}