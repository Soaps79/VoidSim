using System;
using QGame;
using TMPro;
using UnityEngine;

namespace Assets.Placeables
{
	[Serializable]
	public enum HardPointType { Small, Module }

	[RequireComponent(typeof(SpriteRenderer))]
	public class HardPoint : QScript
	{
		public HardPointType HardPointType;
		private SpriteRenderer _sprite;

		public bool IsUsed { get; private set; }

		void Start()
		{
			_sprite = GetComponent<SpriteRenderer>();
			_sprite.enabled = false;
		}

		public void Show()
		{
			_sprite.enabled = true;
		}

		public void Hide()
		{
			_sprite.enabled = false;
		}
	}
}