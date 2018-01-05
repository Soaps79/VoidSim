using System.Collections.Generic;
using Assets.Scripts;
using Assets.WorldMaterials.Population;
using QGame;
using UnityEngine;
using TimeLength = Assets.Scripts.TimeLength;

namespace Assets.Station.Population
{
    /// <summary>
    /// This component will periodically take a group of Persons and tell them to assess their current needs
    /// </summary>
    [RequireComponent(typeof(PopulationControl))]
    public class PopUpdater : QScript
    {
        public int GroupSize;
        public TimeLength UpdateFrequency;
        private int _nextPersonToUpdate;
        private PopulationControl _control;

        void Start()
        {
            var time = Locator.WorldClock.GetSeconds(UpdateFrequency);
            var node = StopWatch.AddNode("update", time);
            node.OnTick += UpdatePeople;

            _control = GetComponent<PopulationControl>();
        }

        private void UpdatePeople()
        {
            var toUpdate = new List<Person>();
            var totalCount = _control.AllPopulation.Count;
            // updates the lesser of the full group size or those remaining in the list
            if (_nextPersonToUpdate + GroupSize > totalCount)
            {
                toUpdate.AddRange(_control.AllPopulation.GetRange(_nextPersonToUpdate, totalCount - _nextPersonToUpdate));
                _nextPersonToUpdate = 0;
            }
            else
            {
                toUpdate.AddRange(_control.AllPopulation.GetRange(_nextPersonToUpdate, GroupSize));
                _nextPersonToUpdate += GroupSize;
            }

            toUpdate.ForEach(i => i.AssessNeeds());
        }
    }
}