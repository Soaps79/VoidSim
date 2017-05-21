using Assets.Scripts;
using DG.Tweening;
using QGame;
using TMPro;
using UnityEngine;

namespace Assets.Station.UI
{
	public class TransactionText : QScript
	{
		private Vector2 _endPosition;
		private float _offsetY;
		private float _travelTime;
		[SerializeField] private IndicatorColors _colors;

		public void Initialize(int amount, bool wasBought)
		{
			_offsetY = 5;
			_travelTime = 5;

			var text = GetComponent<TMP_Text>();
			text.color = wasBought ? _colors.Stop : _colors.Go;
			text.text = amount.ToString();

			transform.DOMove(new Vector3(transform.position.x, transform.position.y + _offsetY, 0), _travelTime)
				.OnComplete(() => Destroy(gameObject));
		}
	}
}