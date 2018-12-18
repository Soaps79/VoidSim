using System;
using Assets.Logistics.Ships;
using Assets.Scripts;
using Assets.Scripts.Serialization;
using DG.Tweening;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
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

	public class ShipBerthData
	{
		[JsonConverter(typeof(StringEnumConverter))]
		public ShipSize ShipSize;
		[JsonConverter(typeof(StringEnumConverter))]
		public BerthState State;
		public bool IsInUse;
		public string Name;
	}

    // This represents a spot where a ship can land in the Traffic system,
    // as well as its representation on-screen
	public class ShipBerth : QScript, ISerializeData<ShipBerthData>
	{
		public ShipSize ShipSize;
		private TrafficShip _ship;
		public bool IsInUse;
		private BerthState _state;
		public int Index;
        public Ship ShipBeingServiced { get; private set; }

		[SerializeField] private SpriteRenderer _indicator;

	    private string _name;
	    public string Name
	    {
	        get { return _name; }
	        set
	        {
	            name = value;
	            _name = value;
	        }
	    }

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

		[SerializeField] private GameColors _colors;

		public Action<TrafficShip> OnShipDock;
		public Action<TrafficShip> OnShipUndock;

		// ship has docked and is ready to be serviced
		public void ConfirmLanding(TrafficShip ship)
		{
			_ship = ship;
            OnShipDock?.Invoke(_ship);
        }

		public void CompleteServicing()
		{
			_ship.BeginDeparture();
			State = BerthState.Empty;

            OnShipUndock?.Invoke(_ship);
        }

		public void Initialize()
		{
			// this wouldn't work when in ctor, why?
			UpdateIndicator();
			var rend = GetComponent<SpriteRenderer>();
			_indicator.sortingLayerID = rend.sortingLayerID;
			_indicator.sortingOrder = rend.sortingOrder + 1;
		}

		public void SetFromData(ShipBerthData data)
		{
			ShipSize = data.ShipSize;
			IsInUse = data.IsInUse;
			State = data.State;
		}

		public void Resume(TrafficShip ship)
		{
			_ship = ship;
			switch (_ship.Phase)
			{
				case TrafficPhase.Approaching:
					State = BerthState.Reserved;
					break;
				case TrafficPhase.Docked:
					State = BerthState.Transfer;
                    OnShipDock?.Invoke(_ship);
                    break;
			}
		}

		public ShipBerthData GetData()
		{
			return new ShipBerthData
			{
				ShipSize = ShipSize,
				IsInUse = IsInUse,
				State = State,
				Name = name
			};
		}
	}
}