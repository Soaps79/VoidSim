using System;
using Assets.Controllers.GUI;
using Assets.Placeables.Nodes;
using Assets.Scripts;
using QGame;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Placeables.UI
{
	public class EnergyConsumerViewModel : QScript
	{
		private EnergyConsumer _consumer;
		private float _lastValue;
		[SerializeField] private Slider _slider;
		private Image _sliderFill;
		[SerializeField] private GameColors _colors;

		public void Bind(EnergyConsumer consumer)
		{
			_consumer = consumer;
			OnEveryUpdate += UpdateValues;
			_sliderFill = _slider.fillRect.GetComponent<Image>();
		}

		private void UpdateValues()
		{
			// if value has changed
			if (Math.Abs(_lastValue - _consumer.CurrentFulfillment) < .01)
				return;

			// update slider fill rate and color
			_lastValue = _consumer.CurrentFulfillment;
			_slider.value = _lastValue;

			if (_lastValue > .50)
				_sliderFill.color = _colors.Go;
			else if (_lastValue > .2)
				_sliderFill.color = _colors.Caution;
			else
				_sliderFill.color = _colors.Stop;
		}
	}
}