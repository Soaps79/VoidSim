using Assets.Placeables.Nodes;
using Assets.Scripts;
using Assets.Station.Efficiency;
using Messaging;
using UnityEngine;

namespace Assets.WorldMaterials.Population
{
	public class LeisureTracker : IMessageListener
	{
		public readonly EfficiencyAffector Affector = new EfficiencyAffector("Leisure");
		private int _currentLeisure;
		private int _currentPopCount;
		private readonly int _baseLeisure;
		private readonly float _maxLeisureBonus;

		public LeisureTracker(MoodParams moodParams, int currentPopCount)
		{
			_baseLeisure = moodParams.BaseLeisure;
			_currentLeisure = _baseLeisure;
			_maxLeisureBonus = moodParams.MaxLeisureBonus;
			_currentPopCount = currentPopCount;
			Locator.MessageHub.AddListener(this, LeisureProvider.MessageName);
			UpdateLeisureAffector();
		}

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
		}

		private void AddLeisure(int amount)
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