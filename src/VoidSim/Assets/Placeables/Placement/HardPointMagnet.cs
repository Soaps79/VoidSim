using System.Collections.Generic;
using System.Linq;
using Assets.Placeables.HardPoints;
using Assets.Station;
using QGame;
using UnityEngine;

namespace Assets.Placeables.Placement
{
	public class HardPointMagnet : QScript
	{
		private HardPointMonitor _hardpointMonitor;
		private readonly List<HardPoint> _availableHardPoints = new List<HardPoint>();
		[SerializeField] private float _snapDistance;
		public HardPoint SnappedTo { get; private set; }
		private GameObject _toPlace;

		public void Initialize(HardPointMonitor hardPointMonitor)
		{
			_hardpointMonitor = hardPointMonitor;
		}

		public bool CanPlace()
		{
			return SnappedTo != null;
		}

		private void CheckForSnap(float delta)
		{
			var point = _availableHardPoints.FirstOrDefault(
				i => Vector3.Distance(transform.position, i.transform.position) < _snapDistance);

			if (point == null)
				return;

			SnappedTo = point;
			_toPlace.transform.position = SnappedTo.transform.position;
			OnEveryUpdate -= BindSpritePositionToMouseCursor;
			OnEveryUpdate -= CheckForSnap;
			OnEveryUpdate += CheckForUnsnap;
		}

		private void CheckForUnsnap(float delta)
		{
			var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			if (Vector3.Distance(mousePosition, SnappedTo.transform.position) <= _snapDistance)
				return;

			OnEveryUpdate += BindSpritePositionToMouseCursor;
			OnEveryUpdate += CheckForSnap;
			OnEveryUpdate -= CheckForUnsnap;
		}

		private void BindSpritePositionToMouseCursor(float delta)
		{
			var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			mousePosition.z = 0;
			_toPlace.transform.position = mousePosition;
			transform.position = mousePosition;
		}

		public void Begin(GameObject toPlace, LayerType layer)
		{
			_toPlace = toPlace;
			_availableHardPoints.AddRange(_hardpointMonitor.GetHardPoints(layer));
			enabled = true;
			OnEveryUpdate += BindSpritePositionToMouseCursor;
			OnEveryUpdate += CheckForSnap;
		}

		public void Complete()
		{
			_toPlace = null;
			SnappedTo = null;
			_availableHardPoints.Clear();
			ClearAllDelegates();
		}
	}
}