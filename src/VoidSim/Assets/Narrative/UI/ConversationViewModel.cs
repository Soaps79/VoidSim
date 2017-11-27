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
		[SerializeField] private Image _actorImage;
		[SerializeField] private TMP_Text _actorNameLabel;
		[SerializeField] private TMP_Text _bodyText;
		[SerializeField] private RectTransform _buttonArray;
		[SerializeField] private Button _buttonPrefab;
		private int _nodeIndex;

		private Conversation _conversation;

		public void Initialize(Conversation conversation)
		{
			_conversation = conversation;
			BindNode(0);
		}

		private void BindNode(int index)
		{
			var node = _conversation.Nodes[index];
			_actorNameLabel.text = node.ActorName;
		}
	}
}