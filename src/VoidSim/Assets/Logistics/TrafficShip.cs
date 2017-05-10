using UnityEngine;
using QGame;

namespace Assets.Logistics
{
    public enum TrafficPhase
    {
        None, Approaching, Docked, Departing
    }

    /// <summary>
    /// This behavior will manage the on-screen representation of a Ship, as well
    /// as communicate with the berth along its course
    /// </summary>
    public class TrafficShip : QScript
    {
        private ShipBerth _berth;
        private Ship _parent;
        public TradeManifestBook ManifestBook { get; private set; }

        // replace with state machine
        public TrafficPhase Phase { get; private set; }

        public void Initialize(Ship parent, ShipBerth berth)
        {
            _parent = parent;
            _berth = berth;
            ManifestBook = _parent.ManifestBook;
            Phase = TrafficPhase.None;
        }

        public void BeginApproach()
        {
            // placeholder for ship moving across the screen
            var node = StopWatch.AddNode("Approach", 5, true);
            node.OnTick += ApproachComplete;
            Debug.Log("Ship begin approach");

            Phase = TrafficPhase.Approaching;
        }

        private void ApproachComplete()
        {
            _berth.ConfirmLanding(this);
        }

        public void BeginDeparture()
        {
            // placeholder for ship moving across the screen
            var node = StopWatch.AddNode("Depart", 5, true);
            node.OnTick += DepartComplete;
            Debug.Log("Ship begin departure");

            Phase = TrafficPhase.Departing;
        }

        private void DepartComplete()
        {
            _parent.OnTrafficComplete();
            Phase = TrafficPhase.None;
        }
    }
}