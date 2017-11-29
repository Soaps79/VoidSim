using System;
using System.Collections.Generic;
using Assets.Narrative.Conversations;
using QGame;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Narrative.UI
{
	public class ConversationViewModel : QScript
	{
		[SerializeField] private TMP_Text _conversationNameLabel;
		[SerializeField] private Image _actorPortrait;
		[SerializeField] private TMP_Text _actorNameLabel;
		[SerializeField] private TMP_Text _bodyText;
		[SerializeField] private RectTransform _buttonArray;
		[SerializeField] private Button _buttonPrefab;
		private readonly List<Button> _activeButtons = new List<Button>();

		// most of the logic in here is based on this index being maintained
		private int _nodeIndex;

		private Conversation _conversation;

		public void BeginConversation(Conversation conversation)
		{
			_conversation = conversation;
			_conversationNameLabel.text = conversation.Title;
			BindCurrentNode();
		}

		private bool IsLastNode()
		{
			return _conversation.Nodes.Count == _nodeIndex + 1;
		}

		private void IncrementMessage()
		{
			ClearButtons();

			if (IsLastNode())
			{
				_conversation.Complete();
				return;
			}

			_nodeIndex++;
			BindCurrentNode();
		}

		private void BindCurrentNode()
		{
			if(_nodeIndex + 1 > _conversation.Nodes.Count)
				throw new IndexOutOfRangeException("Tried to bind Conversation node that doesn't exist");

			var node = _conversation.Nodes[_nodeIndex];
			SetValuesFromNode(node);

			var button = Instantiate(_buttonPrefab, _buttonArray, false);
			var buttonLabel = button.transform.Find("button_text").GetComponent<TMP_Text>();
			if (IsLastNode())
				buttonLabel.text = "Accept";
			else
				buttonLabel.text = "Next";

			button.onClick.AddListener(IncrementMessage);
			_activeButtons.Add(button);
		}

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