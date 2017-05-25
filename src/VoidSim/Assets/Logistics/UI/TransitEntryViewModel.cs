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
		public TMP_Text TradeCountText;

		// this class manages 2 views of the ship,
		// one for a slider and static text
		public GameObject TickerPanel;
		public Slider TickerSlider;
		public TMP_Text TickerText;
		public Image TickerFill;

		// used for transit and simple holds
		public Color TransitColor;
		public Color HoldColor;

		// and one with a single color and traffic phase text
		public GameObject TrafficPanel;
		public TMP_Text TrafficText;

		private Ship _ship;
		private SliderBinding _sliderBinding;

		// Ship is designed to expose anything needed for its UI rep
		public void Bind(Ship ship)
		{
			// hook into ship
			_ship = ship;
			_ship.OnHoldBegin += BeginTraffic;
			_ship.OnTransitBegin += BeginTransit;
			ShipName.text = _ship.Name;
			_ship.ManifestBook.OnContentsUpdated += UpdateTradeCount;
			UpdateTradeCount();

			// hook into UI slider
			_sliderBinding = TickerSlider.gameObject.AddComponent<SliderBinding>();
			_sliderBinding.Initialize(() => _ship.Ticker.TimeRemainingAsZeroToOne);

			// start with both displays off, wait for a callback to turn one on
			TrafficPanel.SetActive(false);
			TickerPanel.SetActive(false);
		}

		private void UpdateTradeCount()
		{
			TradeCountText.text = _ship.ManifestBook.ActiveManifests.Count.ToString();
		}

		private void BeginTraffic()
		{
			TickerPanel.SetActive(false);

			if (_ship.Navigation.CurrentDestination.IsSimpleHold)
			{
				BeginHold();
				return;
			}

			// hook into traffic ship for its short life
			TrafficPanel.SetActive(true);
			var traffic = _ship.TrafficShip;
			TrafficText.text = traffic.Phase.ToString();
			traffic.OnPhaseChanged += HandlePhaseChange;
		}

		private void BeginHold()
		{
			TrafficPanel.SetActive(false);
			TickerPanel.SetActive(true);
			TickerFill.color = HoldColor;
			TickerText.text = "Hold at " + _ship.Navigation.CurrentDestination.ClientName;
		}

		private void HandlePhaseChange(TrafficPhase phase)
		{
			TrafficText.text = phase.ToString();
		}

		private void BeginTransit()
		{
			TrafficPanel.SetActive(false);
			TickerPanel.SetActive(true);
			TickerFill.color = TransitColor;
			TickerText.text = "To " + _ship.Navigation.CurrentDestination.ClientName;
		}
	}
}
