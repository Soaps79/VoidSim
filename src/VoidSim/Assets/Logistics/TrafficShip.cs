using System.Collections.Generic;
using System.Linq;
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

        [SerializeField] private float _minTravelTime;
        [SerializeField] private float _maxTravelTime;
        private float _travelTime;

        private List<Vector3> _waypoints;
        public TradeManifestBook ManifestBook { get; private set; }
        private bool _approachFromLeft;

        // replace with state machine
        public TrafficPhase Phase { get; private set; }

        public void Initialize(Ship parent, ShipBerth berth, List<Vector3> waypoints)
        {
            _parent = parent;
            _berth = berth;
            _waypoints = waypoints;

            transform.position = _waypoints.First();
            if (_waypoints.First().x > 0)
                GetComponent<SpriteRenderer>().flipX = true;

            _travelTime = Random.Range(_minTravelTime, _maxTravelTime);

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
            // currently scales proximity according to starting position
            // should be made independent of that
            var proximity = Vector2.Distance(transform.position, _berth.transform.position);
            var progress = 1 - proximity / _startingDistance;
            var scale = progress * _maxScale - _minScale;
            transform.localScale = new Vector2(_maxScale - scale, _maxScale - scale);
        }

        public void BeginApproach()
        {
            if (transform.position.x < 0)
                _approachFromLeft = true;

            var dir = _waypoints[1] - transform.position;
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(_approachFromLeft ? angle : 180 + angle, Vector3.forward);
            
            transform.DOMove(_waypoints[1], _travelTime)
                .SetEase(Ease.OutSine)
                .OnComplete(ApproachComplete);

            transform.DORotate(_approachFromLeft ? new Vector3(1, 0) : new Vector3(-1, 0), _travelTime);

            Phase = TrafficPhase.Approaching;
        }

        private void ApproachComplete()
        {
            _berth.ConfirmLanding(this);
        }

        public void BeginDeparture()
        {
            transform.DOMove(_waypoints[2], _travelTime)
                .SetEase(Ease.InSine)
                .OnComplete(DepartComplete);

            var dir = _waypoints[2] - transform.position;
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            var rotation = Quaternion.AngleAxis(_approachFromLeft ? angle : 180 + angle, Vector3.forward);

            transform.DORotate(rotation.eulerAngles, _travelTime)
                .SetEase(Ease.OutSine);
            
            Phase = TrafficPhase.Departing;
        }

        private void DepartComplete()
        {
            _parent.OnTrafficComplete();
            Phase = TrafficPhase.None;
        }
    }
}