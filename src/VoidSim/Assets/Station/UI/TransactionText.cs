﻿using Assets.Scripts;
using DG.Tweening;
using QGame;
using TMPro;
using UnityEngine;

namespace Assets.Station.UI
{
	public class TransactionText : QScript
	{
		[SerializeField] private float _offsetY;
		[SerializeField] private float _travelTime;
		[SerializeField] private IndicatorColors _colors;

		public void Initialize(int amount, bool wasBought)
		{
			var text = GetComponent<TMP_Text>();
			text.color = wasBought ? _colors.Stop : _colors.Go;
			text.text = amount.ToString();

			transform.DOMove(new Vector3(transform.position.x, transform.position.y + _offsetY, 0), _travelTime)
				.OnComplete(() => Destroy(gameObject));
			text.DOFade(0, _travelTime)
				.SetEase(Ease.InSine);
		}
	}
}