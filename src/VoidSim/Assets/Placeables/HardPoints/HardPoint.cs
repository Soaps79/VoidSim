using System;
using QGame;
using TMPro;
using UnityEngine;

namespace Assets.Placeables.HardPoints
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
		public int Number;
		[SerializeField] private string _placedName;

		private SpriteRenderer _sprite;
		public SpriteRenderer Sprite {
			get
			{
				if(_sprite == null)
					InitSprite();
				return _sprite;
			}
		}

		public bool IsUsed { get; private set; }

		// had this as Start(), but didnt get called on half of them?
		void InitSprite()
		{
			_sprite = GetComponent<SpriteRenderer>();
			_sprite.enabled = false;
		}

		public void Show()
		{
			Sprite.enabled = true;
		}

		public void Hide()
		{
			Sprite.enabled = false;
		}

		public void HandlePlacement(Placeable placed)
		{
			placed.HardPointName = name;
			_placedName = placed.name;
			IsUsed = true;
		}

		public void HandleRemoval(Placeable placed)
		{
			_placedName = string.Empty;
			IsUsed = false;
		}
	}
}