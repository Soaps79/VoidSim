using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Logistics.Ships;
using Assets.Logistics.Transit;
using Assets.Logistics.UI;
using Assets.Scripts;
using Assets.Scripts.Serialization;
using Assets.WorldMaterials;
using DG.Tweening;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;
using QGame;

namespace Assets.Logistics
{
	[Serializable]
	public enum TrafficPhase
	{
		None, Approaching, Docked, Departing
	}

	public class TrafficShipData
	{
		[JsonConverter(typeof(StringEnumConverter))]
		public TrafficPhase Phase;
		public Vector3Data Position;
		public QuaternionData Rotation;
		public Vector3Data TargetRotation;
		public float StartingDistance;
		public float TravelTime;
		public bool ApproachFromLeft;
		public string BerthName;
		public List<Vector3Data> Waypoints;
		public Vector3Data LocalScale;
		public float ElapsedMovement;
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
		private float _minScale;
		private float _maxScale;

		private float _minTravelTime;
		private float _maxTravelTime;
		private float _travelTime;

		private List<Vector3> _waypoints;
		public CargoManifestBook ManifestBook { get; private set; }
		private bool _approachFromLeft;
		private float _lastMovementBeginTime;
		private Vector3 _targetRotation;
		public string BerthName { get; private set; }
	    public ProductInventory Inventory { get; private set; }

        // replace with state machine
        private TrafficPhase _phase;

		public TrafficPhase Phase
		{
			get
			{
				return _phase;
			}
			private set
			{
				if (value == _phase) return;
				_phase = value;
				CheckPhaseChangeCallback();
			}
		}

		public Action<TrafficPhase> OnPhaseChanged;
		private ShipSO _scriptable;

		[SerializeField] private TrafficShipViewModel _viewModelPrefab;
		private TrafficShipViewModel _viewModel;

		internal void SetScriptable(ShipSO scriptable)
		{
			_scriptable = scriptable;
			_minScale = _scriptable.MinScale;
			_maxScale = _scriptable.MaxScale;
			_minTravelTime = _scriptable.MinTravelTime;
			_maxTravelTime = _scriptable.MaxTravelTime;
			_travelTime = _scriptable.RandomizedTravelTime;
		}

		public void Initialize(Ship parent, ShipBerth berth, List<Vector3> waypoints)
		{
			_parent = parent;
		    Inventory = parent.Inventory;
			SetName();
			_berth = berth;
			_waypoints = waypoints;
			BerthName = _berth.name;

			transform.position = _waypoints.First();
			InitializeGraphics();
			

			InitializeScaling();
			OnEveryUpdate += ScaleWithProximity;
			ManifestBook = _parent.ManifestBook;
			Phase = TrafficPhase.None;
		}

	    private void SetName()
		{
			name = "traffic_" + _parent.Name;
		}

		private void InitializeGraphics()
		{
			GenerateSprite();
			GenerateUI();
		}

		// creates view model and ui panel
		// should probably be refactored to only make ui portion on load
		private void GenerateUI()
		{
			// create view model
			var canvas = Locator.CanvasManager.GetCanvas(CanvasType.LowUpdate);
            _viewModel = Instantiate(_viewModelPrefab, canvas.transform, false);
			_viewModel.Bind(this);
			_viewModel.gameObject.SetActive(false);
		}

		private void GenerateSprite()
		{
			var rend = gameObject.GetComponent<SpriteRenderer>();
			rend.sortingLayerName = "Ships";

			if (_waypoints != null && _waypoints.Any() && _waypoints.First().x > 0)
				rend.flipX = true;
		}


		private void CheckPhaseChangeCallback()
		{
			if (OnPhaseChanged != null)
				OnPhaseChanged(Phase);
		}

		private void InitializeScaling()
		{
			transform.localScale = new Vector2(_maxScale, _maxScale);
			_startingDistance = Vector2.Distance(transform.position, _berth.transform.position);
		}

		private void ScaleWithProximity()
		{
			// currently scales proximity according to starting position
			// should be made independent of that
			var proximity = Vector2.Distance(transform.position, _waypoints[1]);
			var progress = 1 - proximity / _startingDistance;
			var scale = progress * _maxScale - _minScale;
			transform.localScale = new Vector2(_maxScale - scale, _maxScale - scale);
		}

		public void BeginApproach()
		{
			if (transform.position.x < 0)
				_approachFromLeft = true;

			_lastMovementBeginTime = Time.time;

			var dir = _waypoints[1] - transform.position;
			var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.AngleAxis(_approachFromLeft ? angle : 180 + angle, Vector3.forward);

			TweenApproach(RotationWhenDocked, _travelTime);

			Phase = TrafficPhase.Approaching;
		}

		private Vector3 RotationWhenDocked
		{
			get { return _approachFromLeft ? new Vector3(1, 0) : new Vector3(-1, 0); }
		}

		// move and rotate from edge of traffic to berth
		private void TweenApproach(Vector3 rotateTo, float time)
		{
			transform.DOMove(_waypoints[1], time)
				.SetEase(Ease.OutSine)
				.OnComplete(ApproachComplete);

			transform.DORotate(rotateTo, time)
				.SetEase(Ease.InSine);
		}

		private void ApproachComplete()
		{
			_berth.ConfirmLanding(this);
			Phase = TrafficPhase.Docked;
		}

		public void BeginDeparture()
		{
			UpdateTargetRotation();
			TweenDeparture(_targetRotation, _travelTime);
			Phase = TrafficPhase.Departing;
		}

		private void UpdateTargetRotation()
		{
			var dir = _waypoints[2] - transform.position;
			var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
			var rotation = Quaternion.AngleAxis(_approachFromLeft ? angle : 180 + angle, Vector3.forward);
			_targetRotation = rotation.eulerAngles;
		}

		public void BeginEarlyDeparture()
		{
			transform.DOKill();
			UpdateTargetRotation();
			TweenDeparture(_targetRotation, _travelTime);
			Phase = TrafficPhase.Departing;
		}

		// move and rotate from berth to edge of traffic
		private void TweenDeparture(Vector3 rotateTo, float time)
		{
			transform.DOMove(_waypoints[2], time)
				.SetEase(Ease.InSine)
				.OnComplete(CompleteDeparture);

			transform.DORotate(rotateTo, time)
				.SetEase(Ease.OutSine);
		}

		private void CompleteDeparture()
		{
			_parent.CompleteTraffic();
			Destroy(_viewModel.gameObject);
			Phase = TrafficPhase.None;
		}

		#region Serialization
		// this overload is called when a ship that was serialized in traffic has been loaded
		public void Initialize(Ship parent, TrafficShipData data)
		{
			_parent = parent;
			SetName();
			_waypoints = new List<Vector3>();
			_startingDistance = data.StartingDistance;
			_targetRotation = data.TargetRotation;
			_approachFromLeft = data.ApproachFromLeft;
			BerthName = data.BerthName;
			data.Waypoints.ForEach(i => _waypoints.Add(i));
			_travelTime = data.TravelTime;

			InitializeGraphics();
			
			ManifestBook = _parent.ManifestBook;
			OnEveryUpdate += ScaleWithProximity;

			transform.localScale = data.LocalScale;
			transform.position = data.Position;
			transform.rotation = data.Rotation;

			_phase = data.Phase;

			switch (Phase)
			{
				case TrafficPhase.Approaching:
					ResumeApproach(data.ElapsedMovement);
					break;
				case TrafficPhase.Departing:
					ResumeDeparture(data.ElapsedMovement);
					break;
			}
		}

		private void ResumeDeparture(float elapsed)
		{
			TweenDeparture(_targetRotation, _travelTime - elapsed);
		}

		private void ResumeApproach(float elapsed)
		{
			TweenApproach(RotationWhenDocked, _travelTime - elapsed);
		}

		public TrafficShipData GetData()
		{
			return new TrafficShipData
			{
				Phase = Phase,
				Position = transform.position,
				Rotation = transform.rotation,
				TargetRotation = _targetRotation,
				StartingDistance = _startingDistance,
				TravelTime = _travelTime,
				ApproachFromLeft = _approachFromLeft,
				Waypoints = new List<Vector3Data>{ _waypoints[0], _waypoints[1], _waypoints[2] },
				LocalScale = transform.localScale,
				ElapsedMovement = Time.time - _lastMovementBeginTime,
				BerthName = BerthName
			};
		}

		#endregion

		public void Resume(ShipBerth berth)
		{
			_berth = berth;
		}
	}
}