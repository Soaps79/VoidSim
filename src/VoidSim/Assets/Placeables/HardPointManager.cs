using System.Collections.Generic;
using System.Linq;
using ModestTree;
using QGame;

namespace Assets.Placeables
{
	public class HardPointManager : QScript
	{
		private readonly List<HardPoint> _points = new List<HardPoint>();

		void Start()
		{
			var points = GetComponentsInChildren<HardPoint>();
			_points.AddRange(points);
		}

		public bool TryActivateHardpoints()
		{
			var open = _points.Where(i => !i.IsUsed);

			if (!open.Any())
				return false;

			open.ForEach(i => i.Show());
			return true;
		}

		public void CompletePlacement()
		{
			_points.ForEach(i => i.Hide());
		}
	}
}