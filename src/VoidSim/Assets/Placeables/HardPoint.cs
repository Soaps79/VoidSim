using System;
using QGame;
using TMPro;
using UnityEngine;

namespace Assets.Placeables
{
	[Serializable]
	public enum HardPointType { Small, Module }

	/// <summary>
	/// Owned by a station layer, provides a position on which a placeable can be placed
	/// </summary>
	[RequireComponent(typeof(SpriteRenderer))]
	public class HardPoint : QScript
	{
		public HardPointType HardPointType;
		public SpriteRenderer Sprite;

		public bool IsUsed { get; private set; }

		void Start()
		{
			Sprite = GetComponent<SpriteRenderer>();
			Sprite.enabled = false;
		}

		public void Show()
		{
			Sprite.enabled = true;
		}

		public void Hide()
		{
			Sprite.enabled = false;
		}
	}
}