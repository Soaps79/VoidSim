using Assets.WorldMaterials.Population;
using NUnit.Framework;

namespace Assets.Editor
{
	public class LeisureTests
	{
		[Test]
		public void Tracker_BasicFunctions()
		{
			const int baseAmount = 10;
			const int increment = baseAmount / 2;
			const int total = baseAmount * 2;

			var mood = new MoodParams
			{
				BaseLeisure = baseAmount
			};

			var tracker = new LeisureTracker();

			tracker.Initialize(mood, total);
			Assert.AreEqual(.5, tracker.Affector.Efficiency);

			tracker.AddLeisure(increment);
			Assert.AreEqual(.75, tracker.Affector.Efficiency);

			tracker.AddLeisure(increment);
			Assert.AreEqual(1, tracker.Affector.Efficiency);
		}

		[Test]
		public void Tracker_MaxLeisureIsHonored()
		{
			const int baseAmount = 10;
			const int increment = 8;
			const float maxBonus = 1.5f;

			var mood = new MoodParams
			{
				MaxLeisureBonus	= maxBonus,
				BaseLeisure = baseAmount
			};

			var tracker = new LeisureTracker();
			tracker.Initialize(mood, baseAmount);

			Assert.AreEqual(1, tracker.Affector.Efficiency);

			tracker.AddLeisure(increment);

			Assert.AreEqual(maxBonus, tracker.Affector.Efficiency);
		}
	}
}