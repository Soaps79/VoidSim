using Assets.Controllers.GUI;
using Assets.Logistics.Ships;
using QGame;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Logistics.UI
{
	public class TransitEntryViewModel : QScript
	{
		public TMP_Text ShipName;

		public GameObject TransitPanel;
		public Slider TransitSlider;
		public TMP_Text TransitText;

		public GameObject TrafficPanel;
		public TMP_Text TrafficText;

		private Ship _ship;
		private SliderBinding _sliderBinding;

		public void Bind(Ship ship)
		{
			_ship = ship;
			_ship.OnTrafficBegin += BeginTraffic;
			_ship.OnTransitBegin += BeginTransit;
			ShipName.text = _ship.Name;

			_sliderBinding = TransitSlider.gameObject.AddComponent<SliderBinding>();
			_sliderBinding.Initialize(() => _ship.TransitTime.TimeRemainingAsZeroToOne);

			TrafficPanel.SetActive(false);
			TransitPanel.SetActive(false);
		}

		private void BeginTraffic()
		{
			TransitPanel.SetActive(false);

			TrafficPanel.SetActive(true);
			var traffic = _ship.TrafficShip;
			TrafficText.text = traffic.Phase.ToString();
			traffic.OnPhaseChanged += HandlePhaseChange;
		}

		private void HandlePhaseChange(TrafficPhase phase)
		{
			TrafficText.text = phase.ToString();
		}

		private void BeginTransit()
		{
			TrafficPanel.SetActive(false);
			TransitPanel.SetActive(true);
			TransitText.text = "To " + _ship.Navigation.CurrentDestination.ClientName;
		}
	}
}
