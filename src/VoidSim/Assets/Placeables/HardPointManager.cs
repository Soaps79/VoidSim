using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Station;
using ModestTree;
using QGame;

namespace Assets.Placeables
{
	public interface IHardPointManager
	{
		void ActivateHardpoints();
		void CompletePlacement();
	}

	public class NullHardpointManager : IHardPointManager
	{
		public void ActivateHardpoints() { }

		public void CompletePlacement() { }
	}

	public class HardPointManager : QScript, IHardPointManager
	{
		private readonly List<HardPoint> _points = new List<HardPoint>();
		private LayerType _layer;

		public void Initialize(LayerType layer)
		{
			_layer = layer;
			var points = GetComponentsInChildren<HardPoint>();
			var index = 1;
			foreach (var hardPoint in points)
			{
				hardPoint.name = "hardpoint_" + GetAbbreviation(_layer) +"_" + index;
				_points.Add(hardPoint);
				index++;
			}
		}

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

		public void ActivateHardpoints()
		{
			var open = _points.Where(i => !i.IsUsed);

			if (!open.Any())
				return;

			open.ForEach(i => i.Show());
		}

		public void CompletePlacement()
		{
			_points.ForEach(i => i.Hide());
		}
	}
}