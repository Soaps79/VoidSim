using DG.Tweening;
using QGame;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Controllers.GUI
{
	[RequireComponent(typeof(Image))]
	[RequireComponent(typeof(Toggle))]
	public class ToggleColorBinder : QScript
	{
		[SerializeField] private Color _colorOn;
		[SerializeField] private float _duration = .5f;
		private Color _baseColor;
		private Toggle _toggle;
		private Image _image;


		void Start()
		{
			_image = GetComponent<Image>();
			_baseColor = _image.color;

			_toggle = GetComponent<Toggle>();
			_toggle.onValueChanged.AddListener(HandleValueChanged);
		}

		private void HandleValueChanged(bool value)
		{
			_image.DOKill();

			if (value)
			{
				_image.color = _baseColor;
				_image.DOColor(_colorOn, _duration);
			}
			else
			{
				_image.color = _colorOn;
				_image.DOColor(_baseColor, _duration);
			}
		}
	}
}