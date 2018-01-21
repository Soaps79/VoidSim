using System.Collections;
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

        private readonly List<Person> _toUpdate = new List<Person>();
        private float _time;

        void Start()
        {
            _time = Locator.WorldClock.GetSeconds(UpdateFrequency);
            var node = StopWatch.AddNode("update", _time);
            node.OnTick += UpdatePeople;

            _control = GetComponent<PopulationControl>();
        }

        private void UpdatePeople()
        {
            var totalCount = _control.AllPopulation.Count;
            _toUpdate.Clear();
            // updates the lesser of the full group size or those remaining in the list
            if (_nextPersonToUpdate + GroupSize > totalCount)
            {
                _toUpdate.AddRange(_control.AllPopulation.GetRange(_nextPersonToUpdate, totalCount - _nextPersonToUpdate));
                _nextPersonToUpdate = 0;
            }
            else
            {
                _toUpdate.AddRange(_control.AllPopulation.GetRange(_nextPersonToUpdate, GroupSize));
                _nextPersonToUpdate += GroupSize;
            }

            StartCoroutine(UpdateList());
        }

        private IEnumerator UpdateList()
        {
            var startTime = Time.time;
            for (int i = 0; i < _toUpdate.Count; i++)
            {
                _toUpdate[i].AssessNeeds();
                yield return null;
            }

            if(Time.time - startTime > _time)
                throw new UnityException("PopUpdater took longer to assess needs than its given tickTime, problems coming.");
        }
    }
}