using Assets.Narrative.Goals;
using QGame;

namespace Assets.Narrative
{
	public class NarrativeMonitor : QScript
	{
		private CraftGoalTracker _craftGoalTracker;

		void Start()
		{
			// delay to let the game objects get set up
			OnNextUpdate += InitializeTrackers;
		}

		private void InitializeTrackers(float obj)
		{
			_craftGoalTracker = new CraftGoalTracker();
		}
	}
}