using Assets.Narrative.Goals;
using Assets.Narrative.Missions;
using NUnit.Framework;

namespace Assets.Editor
{
	//public class TestProductGoalTracker : ProductGoalTrackerBase
	//{
	//	private GoalType _goalType;
	//	public override GoalType GoalType { get { return _goalType; } }

	//	public TestProductGoalTracker(GoalType type)
	//	{
	//		_goalType = type;
	//	}
	//}

	public class NarrativeTests
	{
		[Test]
		public void Mission_GoalIsAddedAndActivated()
		{
			var mission = new Mission();
			var goal = new ProductAmountGoal();
			mission.AddAndActivateGoal(goal);

			Assert.AreEqual(1, mission.Goals.Count);
			Assert.IsTrue(mission.Goals[0].IsActive);
		}

		[Test]
		public void Mission_SingleGoal()
		{
			var mission = new Mission();
			var triggered = false;
			var marked = false;
			mission.OnComplete += (m) =>
				{
					triggered = true;
					marked = m.IsComplete;
				};

			var goal = new ProductAmountGoal();
			mission.AddAndActivateGoal(goal);
			goal.TriggerComplete(true);

			Assert.IsTrue(triggered);
			Assert.IsTrue(marked);
		}

		[Test]
		public void Mission_MultipleGoals()
		{
			var mission = new Mission();
			var triggered = false;
			var marked = false;
			mission.OnComplete += (m) =>
			{
				triggered = true;
				marked = m.IsComplete;
			};

			var goal = new ProductAmountGoal();
			mission.AddAndActivateGoal(goal);

			var goal2 = new ProductAmountGoal();
			mission.AddAndActivateGoal(goal2);

			goal.TriggerComplete(true);
			Assert.IsFalse(triggered);
			Assert.IsFalse(marked);

			goal2.TriggerComplete(true);
			Assert.IsTrue(triggered);
			Assert.IsTrue(marked);
		}
	}
}