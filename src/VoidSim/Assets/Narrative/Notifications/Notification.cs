using System;
using UnityEngine;

namespace Assets.Narrative.Notifications
{
	[Serializable]
	public enum NotificationType
	{
		ConversationStart
	}

	// represents a small data package used by notification buttons
	[Serializable]
	public class Notification
	{
		public NotificationType Type;
		public Sprite IconSprite;
		public string TooltipText;
	}
}