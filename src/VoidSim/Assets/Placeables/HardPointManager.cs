using System.Collections.Generic;
using System.Linq;
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

		void Start()
		{
			var points = GetComponentsInChildren<HardPoint>();
			_points.AddRange(points);
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