using DG.Tweening;
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
        private float _startingDistance;
        [SerializeField] private float _minScale;
        [SerializeField] private float _maxScale;

        public TradeManifestBook ManifestBook { get; private set; }

        // replace with state machine
        public TrafficPhase Phase { get; private set; }

        public void Initialize(Ship parent, ShipBerth berth)
        {
            _parent = parent;
            _berth = berth;
            InitializeScaling();
            OnEveryUpdate += ScaleWithProximity;
            ManifestBook = _parent.ManifestBook;
            Phase = TrafficPhase.None;
        }

        private void InitializeScaling()
        {
            transform.localScale = new Vector2(_maxScale, _maxScale);
            _startingDistance = Vector2.Distance(transform.position, _berth.transform.position);
        }

        private void ScaleWithProximity(float obj)
        {
            var proximity = Vector2.Distance(transform.position, _berth.transform.position);
            var progress = 1 - proximity / _startingDistance;
            var scale = progress * _maxScale - _minScale;
            transform.localScale = new Vector2(_maxScale - scale, _maxScale - scale);
        }

        public void BeginApproach()
        {
            // placeholder for ship moving across the screen
            //var node = StopWatch.AddNode("Approach", 5, true);
            //node.OnTick += ApproachComplete;
            Debug.Log("Ship begin approach");

            var tween = transform.DOMove(_berth.transform.position, 5);

            tween.OnComplete(ApproachComplete);

            

            Phase = TrafficPhase.Approaching;
        }

        private void ApproachComplete()
        {
            _berth.ConfirmLanding(this);
        }

        public void BeginDeparture()
        {
            // placeholder for ship moving across the screen
            //var node = StopWatch.AddNode("Depart", 5, true);
            //node.OnTick += DepartComplete;
            Debug.Log("Ship begin departure");

            transform.DOMove(new Vector3(60, -5, 0), 10)
                .OnComplete(DepartComplete);
            
            Phase = TrafficPhase.Departing;
        }

        private void OnMovement()
        {
            throw new System.NotImplementedException();
        }

        private void DepartComplete()
        {
            _parent.OnTrafficComplete();
            Phase = TrafficPhase.None;
        }
    }
}