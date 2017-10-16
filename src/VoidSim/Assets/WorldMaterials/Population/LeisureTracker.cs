using Assets.Placeables.Nodes;
using Assets.Scripts;
using Assets.Station.Efficiency;
using Messaging;
using UnityEngine;

namespace Assets.WorldMaterials.Population
{
	/// <summary>
	/// Listens for LeisureProvider nodes to be placed.
	/// Affects population accordingly.
	/// </summary>
	public class LeisureTracker : IMessageListener
	{
		public readonly EfficiencyAffector Affector = new EfficiencyAffector("Leisure");
		// leisure is calculated as the amount provided over the amount needed
		// there is a max amount that leisure will affect population's mood
		private int _currentLeisure;
		private int _currentPopCount;
		private int _baseLeisure;
		private float _maxLeisureBonus;

		public LeisureTracker() { }

		public LeisureTracker(MoodParams moodParams, int currentPopCount)
		{
			Initialize(moodParams, currentPopCount);
			Locator.MessageHub.AddListener(this, LeisureProvider.MessageName);
		}

		// exposed for use in testing
		public void Initialize(MoodParams moodParams, int currentPopCount)
		{
			_baseLeisure = moodParams.BaseLeisure;
			_currentLeisure = _baseLeisure;
			_maxLeisureBonus = moodParams.MaxLeisureBonus;
			_currentPopCount = currentPopCount;
			UpdateLeisureAffector();
		}

		// calculate the current leisure and inform Affector
		private void UpdateLeisureAffector()
		{
			if (_currentPopCount == 0) { return; }

			var leisure = (float)_currentLeisure / _currentPopCount;

			if (leisure > 1)
			{
				if (_currentLeisure <= _baseLeisure)
					leisure = 1;
				else if (leisure > _maxLeisureBonus)
					leisure = _maxLeisureBonus;
			}

			Affector.Efficiency = leisure;
		}

		public void HandleMessage(string type, MessageArgs args)
		{
			if (type == LeisureProvider.MessageName && args != null)
			{;
				HandleLeisureNode(args as LeisureProviderMessageArgs);
			}
		}

		private void HandleLeisureNode(LeisureProviderMessageArgs args)
		{
			if (args == null || args.LeisureProvider == null)
				throw new UnityException("LeisureTracker given bad leisure node args");

			AddLeisure(args.LeisureProvider.AmountProvided);
			args.LeisureProvider.OnRemove += HandleRemove;
		}

		private void HandleRemove(LeisureProvider obj)
		{
			AddLeisure(-obj.AmountProvided);
		}

		// exposed for use in testing
		public void AddLeisure(int amount)
		{
			_currentLeisure += amount;
			UpdateLeisureAffector();
		}


		public string Name { get { return "LeisureTracker"; } }

		public void UpdatePopCount(int currentPopCount)
		{
			_currentPopCount = currentPopCount;
			UpdateLeisureAffector();
		}
	}
}