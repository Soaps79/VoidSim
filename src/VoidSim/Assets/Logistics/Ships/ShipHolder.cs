using System.Collections.Generic;
using System.Linq;
using QGame;
using UnityEngine;

namespace Assets.Logistics.Ships
{
	public class ShipHolder : QScript
	{
		[SerializeField] private TimeLength _shipDelayTime;
		private float _shipDelaySeconds;

		private readonly List<Ship> _shipsOnHold = new List<Ship>();
		private readonly List<Ship> _shipstoRemove = new List<Ship>();

		public int Count {  get { return _shipsOnHold.Count; } }

		void Start()
		{
			_shipDelaySeconds = WorldClock.Instance.GetSeconds(_shipDelayTime);
			OnEveryUpdate += UpdateShips;
		}

		private void UpdateShips(float delta)
		{
			if (!_shipsOnHold.Any())
				return;

			foreach (var ship in _shipsOnHold)
			{
				ship.Ticker.ElapsedTicks += delta;
				if (ship.Ticker.IsComplete)
					_shipstoRemove.Add(ship);
			}

			if (!_shipstoRemove.Any())
				return;

			_shipsOnHold.RemoveAll(i => _shipstoRemove.Contains(i));
			_shipstoRemove.ForEach(i => i.CompleteTraffic());
			_shipstoRemove.Clear();
		}

		public void BeginHold(Ship ship)
		{
			ship.Ticker.Reset(_shipDelaySeconds);
			ship.BeginHold(null, null);
			_shipsOnHold.Add(ship);
		}

		public List<Ship> ReleaseShips()
		{
			var list = _shipsOnHold.ToList();
			_shipsOnHold.Clear();
			return list;
		}
	}
}