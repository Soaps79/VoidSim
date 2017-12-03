using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Narrative.Conversations;
using Assets.Narrative.Missions;
using DG.Tweening;
using QGame;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Narrative.UI
{
	public class ConversationViewModel : QScript
	{
		private class ButtonEntry
		{
			public Button Button;
			
		}

		[SerializeField] private TMP_Text _conversationNameLabel;
		[SerializeField] private Image _actorPortrait;
		[SerializeField] private TMP_Text _actorNameLabel;
		[SerializeField] private TMP_Text _bodyText;
		[SerializeField] private RectTransform _buttonArray;
		[SerializeField] private Button _buttonPrefab;
		[SerializeField] private CanvasGroup _canvasGroup;
		
		private Conversation _conversation;
		private readonly List<Button> _activeButtons = new List<Button>();
		private ConversationEntry _currentNode;
		public Action<MissionGroupSO> OnMissionsNeedStart { get; set; }

		public void BeginConversation(Conversation conversation)
		{
			_conversation = conversation;
			_conversationNameLabel.text = conversation.Title;
			_canvasGroup.DOPlay();
			if (gameObject.activeSelf == false)
			{
				gameObject.SetActive(true);
				_canvasGroup.DOFade(1f, .5f);
			}
			BindEntry(conversation.InitialEntry);
		}

		// load the next message if there is one, otherwise fade out
		private void IncrementMessage(ConversationEntry entry)
		{
			if (_currentNode.Node.Missions != null)
				OnMissionsNeedStart(_currentNode.Node.Missions);

			ClearButtons();

			if (entry == null)
			{
				_conversation.Complete();
				_canvasGroup.DOFade(0, .5f)
					.OnComplete(() => { gameObject.SetActive(false); });
				return;
			}

			BindEntry(entry);
		}

		// Refreshes the UI to represent a conversation node
		private void BindEntry(ConversationEntry entry)
		{
			_currentNode = entry;
			SetValuesFromNode(_currentNode.Node);

			if (!entry.Transitions.Any())
				CreateButton("Finish", null);
			else
			{
				entry.Transitions.ForEach(i => CreateButton(i.ButtonText, i.Next));
			}
		}

		// create a button, and give it the entry it will trigger to load
		private void CreateButton(string text, ConversationEntry entry)
		{
			var button = Instantiate(_buttonPrefab, _buttonArray, false);
			var buttonLabel = button.transform.Find("button_text").GetComponent<TMP_Text>();
			buttonLabel.text = text;

			button.onClick.AddListener(() =>
			{
				IncrementMessage(entry);
			});
			_activeButtons.Add(button);
		}

		// maps node data to the ui
		private void SetValuesFromNode(ConversationNode node)
		{
			_actorNameLabel.text = node.Actor.DisplayName;
			_actorPortrait.sprite = node.Actor.PortraitSprite;
			_bodyText.text = node.Text;
		}

		private void ClearButtons()
		{
			_activeButtons.ForEach(i => Destroy(i.gameObject));
			_activeButtons.Clear();
		}
	}
}