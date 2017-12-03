using System;
using UnityEngine;

namespace Assets.Narrative
{
	[Serializable]
	public enum NotificationType
	{
		ConversationStart
	}

	[Serializable]
	public class Notification
	{
		public NotificationType Type;
		public Sprite IconSprite;
		public string TooltipText;
	}
}