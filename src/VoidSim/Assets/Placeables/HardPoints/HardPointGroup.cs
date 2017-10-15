using System.Collections.Generic;
using System.Linq;
using Assets.Station;
using Messaging;
using QGame;
using UnityEngine;

namespace Assets.Placeables.HardPoints
{
	public interface IHardPointGroup
	{
		void ActivateHardpoints();
		void DeactivateHardpoints();
		IEnumerable<HardPoint> GetAvailableHardPoints();
		void HandlePlacement(Placeable placeable);
		void HandleRemoval(Placeable placeable);
	}

	// exists so that non-active layers (core) do not have
	// to null check all over the place
	public class NullHardpointGroup : IHardPointGroup
	{
		public void ActivateHardpoints() { }
		public void DeactivateHardpoints() { }
		public IEnumerable<HardPoint> GetAvailableHardPoints() { return new List<HardPoint>(); }
		public void HandlePlacement(Placeable placeable) { }
		public void HandleRemoval(Placeable placeable) { }
	}

	public class HardPointGroupUpdateMessage : MessageArgs
	{
		public IHardPointGroup Group;
		public LayerType Layer;
	}

	/// <summary>
	/// This object holds references to a group of hardpoints, and facilitates outside interaction with them
	/// </summary>
	public class HardPointGroup : QScript, IHardPointGroup
	{
		public const string MessageName = "HardPointGroup";
		private readonly List<HardPoint> _points = new List<HardPoint>();
		private LayerType _layer;

		public void Initialize(LayerType layer)
		{
			_layer = layer;
			var points = GetComponentsInChildren<HardPoint>();
			// name the hardpoint and set its drawing layer
			foreach (var hardPoint in points)
			{
				hardPoint.name = "hardpoint_" + GetAbbreviation(_layer) +"_" + hardPoint.Number;
				if(hardPoint.Sprite == null)
					throw new UnityException("Hardpoint missing sprite");
				hardPoint.Sprite.sortingLayerName = layer.ToString();
				hardPoint.Sprite.sortingOrder = 1;
				_points.Add(hardPoint);
			}
		}

		// for naming, move outside if gets used more
		private string GetAbbreviation(LayerType layer)
		{
			switch (layer)
			{
				case LayerType.Top:
					return "top";
				case LayerType.Middle:
					return "mid";
				case LayerType.Bottom:
					return "bot";
				case LayerType.Core:
					return "core";
			}

			return "notfound";
		}

		// show all hardpoints on screen
		public void ActivateHardpoints()
		{
			var open = _points.Where(i => !i.IsUsed).ToList();

			if (!open.Any())
				return;

			open.ForEach(i => i.Show());
		}

		// hide all hardpoints
		public void DeactivateHardpoints()
		{
			_points.ForEach(i => i.Hide());
		}

		public IEnumerable<HardPoint> GetAvailableHardPoints()
		{
			return _points.Where(i => !i.IsUsed);
		}

		public void HandlePlacement(Placeable placeable)
		{
			var point = _points.FirstOrDefault(i => i.name == placeable.HardPointName);
			if(point != null)
				point.HandlePlacement(placeable);
		}

		public void HandleRemoval(Placeable placeable)
		{
			var point = _points.FirstOrDefault(i => i.name == placeable.HardPointName);
			if (point != null)
				point.HandleRemoval(placeable);
		}
	}
}