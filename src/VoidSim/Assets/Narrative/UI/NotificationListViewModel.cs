using System.Collections.Generic;
using Assets.Narrative.Conversations;
using Assets.Narrative.Notifications;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Assets.Narrative.UI
{
	// handles showing notifications to the player in a list
	// there's not much to maintaining the list, buttons clear themselves when they should
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

		// this could remain the preferred method; a function per type
		// or it could be better worked into the architecture
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