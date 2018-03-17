using System.Collections.Generic;
using System.Linq;
using Assets.Placeables.HardPoints;
using Assets.Station;
using QGame;
using UnityEngine;

namespace Assets.Placeables.Placement
{
	/// <summary>
	/// This class handles the placeable sprit during placement.
	/// It stick the sprite to the cursor, sticking it to a HardPoint when one is near.
	/// </summary>
	public class HardPointMagnet : QScript
	{
		private HardPointMonitor _hardpointMonitor;
		private readonly List<HardPoint> _availableHardPoints = new List<HardPoint>();
		[SerializeField] private float _snapDistance;
		public HardPoint SnappedTo { get; private set; }
		private GameObject _toPlace;

		public void Initialize(HardPointMonitor hardPointMonitor)
		{
			// monitor provides list of hard points for given layer when placement is started
			_hardpointMonitor = hardPointMonitor;
		}

		// Is placeable snapped to an available HardPoint?
		public bool CanPlace()
		{
			return SnappedTo != null;
		}

		// placeable is stuck to cursor, checking for proximity to available hard points
		private void CheckForSnap()
		{
			var point = _availableHardPoints.FirstOrDefault(
				i => Vector3.Distance(transform.position, i.transform.position) < _snapDistance);

			if (point == null)
				return;

			SnappedTo = point;
			_toPlace.transform.position = SnappedTo.transform.position;
			// after snapping to position, un-stick it from mouse cursor
			OnEveryUpdate -= BindSpritePositionToMouseCursor;
			OnEveryUpdate -= CheckForSnap;
			OnEveryUpdate += CheckForUnsnap;
		}

		private void CheckForUnsnap()
		{
			var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			if (Vector3.Distance(mousePosition, SnappedTo.transform.position) <= _snapDistance)
				return;

			// when mouse cursor is far enough away from snapped point, placeable goes back to sticking to it
			OnEveryUpdate += BindSpritePositionToMouseCursor;
			OnEveryUpdate += CheckForSnap;
			OnEveryUpdate -= CheckForUnsnap;
		}

		// keeps placebale stuck to mouse cursor
		private void BindSpritePositionToMouseCursor()
		{
			var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			mousePosition.z = 0;
			_toPlace.transform.position = mousePosition;
			transform.position = mousePosition;
		}

		// get available hard points, stick sprite to cursor, start looking for nearby hardpoints
		public void Begin(GameObject toPlace, LayerType layer)
		{
			_toPlace = toPlace;
			_availableHardPoints.AddRange(_hardpointMonitor.GetHardPoints(layer));
			enabled = true;
			OnEveryUpdate += BindSpritePositionToMouseCursor;
			OnEveryUpdate += CheckForSnap;
		}

		// null all the things
		public void Complete()
		{
			_toPlace = null;
			SnappedTo = null;
			_availableHardPoints.Clear();
			ClearAllDelegates();
		}
	}
}