using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using QGame;
using UnityEngine;
using TimeLength = Assets.Scripts.TimeLength;
using WorldClock = Assets.Scripts.WorldClock;

namespace Assets.Logistics.Ships
{
	public class ShipHolder : QScript
	{
		[SerializeField] private TimeLength _shipDelayTime;
		private float _shipDelaySeconds;

		public List<Ship> ShipsOnHold { get; private set; }
		private readonly List<Ship> _shipstoRemove = new List<Ship>();

		public int Count {  get { return ShipsOnHold.Count; } }

		void Start()
		{
			ShipsOnHold = new List<Ship>();
			_shipDelaySeconds = Locator.WorldClock.GetSeconds(_shipDelayTime);
			OnEveryUpdate += UpdateShips;
		}

		private void UpdateShips()
		{
			if (!ShipsOnHold.Any())
				return;

		    foreach (var ship in ShipsOnHold)
			{
				ship.Ticker.ElapsedTicks += GetDelta();
				if (ship.Ticker.IsComplete)
					_shipstoRemove.Add(ship);
			}

			if (!_shipstoRemove.Any())
				return;

			ShipsOnHold.RemoveAll(i => _shipstoRemove.Contains(i));
			_shipstoRemove.ForEach(i => i.CompleteTraffic());
			_shipstoRemove.Clear();
		}

		public void BeginHold(Ship ship, bool isResume = false)
		{
			if(!isResume)
				ship.Ticker.Reset(_shipDelaySeconds);
			ship.BeginHold(null, null);
			ShipsOnHold.Add(ship);
		}

		public List<Ship> ReleaseShips()
		{
			var list = ShipsOnHold.ToList();
			ShipsOnHold.Clear();
			return list;
		}

	    public List<Ship> RemoveShips(int count)
	    {
	        var toRemove = Mathf.Min(ShipsOnHold.Count, count);

            var toReturn = ShipsOnHold.GetRange(0, toRemove);
            ShipsOnHold.RemoveRange(0, toRemove);
	        return toReturn;
	    }
	}
}