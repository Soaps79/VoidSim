using UnityEngine;
using QGame;

namespace Assets.Logistics
{
    /// <summary>
    /// This behavior will manage the on-screen representation of a Ship, as well
    /// as communicate with the berth along its course
    /// </summary>
    public class TrafficShip : QScript
    {
        private ShipBerth _berth;
        private Ship _parent;

        public void Initialize(Ship parent, ShipBerth berth)
        {
            _parent = parent;
            _berth = berth;
        }

        public void BeginApproach()
        {
            // placeholder for ship moving across the screen
            var node = StopWatch.AddNode("Approach", 5, true);
            node.OnTick += ApproachComplete;
            Debug.Log("Ship begin approach");
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
        }

        private void DepartComplete()
        {
            _parent.OnTrafficComplete();
        }
    }
}