using System;
using Assets.Scripts;
using DG.Tweening;
using QGame;
using UnityEngine;

namespace Assets.Logistics
{
	public enum ShipSize
	{
		Corvette, Freighter
	}

	public enum BerthState
	{
		Empty, Reserved, Transfer
	}

	public class ShipBerth : QScript
	{
		public ShipSize ShipSize;
		private TrafficShip _ship;
		public bool IsInUse;
		private BerthState _state;

		[SerializeField] private SpriteRenderer _indicator;

		public BerthState State
		{
			get { return _state; }
			set
			{
				if (value != _state)
				{
					_state = value;
					IsInUse = _state != BerthState.Empty;
					UpdateIndicator();
				}
			}
		}

		private void UpdateIndicator()
		{
			switch (State)
			{
				case BerthState.Empty:
					ResetIndicator(_colors.Go);
					break;
				case BerthState.Reserved:
					ResetIndicator(_colors.Stop);
					break;
				case BerthState.Transfer:
					ResetIndicator(_colors.Caution);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void ResetIndicator(Color color)
		{
			_indicator.DOKill();
			_indicator.color = Color.clear;
			_indicator.DOColor(color, .5f);
		}

		[SerializeField] private IndicatorColors _colors;

		public Action<TrafficShip> OnShipDock;
		public Action<TrafficShip> OnShipUndock;

		// ship has docked and is ready to be serviced
		public void ConfirmLanding(TrafficShip ship)
		{
			_ship = ship;
			if (OnShipDock != null)
				OnShipDock(_ship);
		}

		public void CompleteServicing()
		{
			_ship.BeginDeparture();
			State = BerthState.Empty;

			if (OnShipUndock != null)
				OnShipUndock(_ship);
		}

		public void Initialize()
		{
			// this wouldn't work when in ctor, why?
			UpdateIndicator();
		}
	}
}