using System.Collections.Generic;
using System.Linq;
using Assets.Placeables.Nodes;
using Assets.Scripts;
using Assets.WorldMaterials;
using Assets.WorldMaterials.Population;
using Assets.WorldMaterials.Products;
using Messaging;
using QGame;
using UnityEngine;
using TimeLength = Assets.Scripts.TimeLength;

// ReSharper disable FieldCanBeMadeReadOnly.Local
// - leaving them not-readonly to see in the editor

namespace Assets.Station.Population
{
    /// <summary>
    /// Finds housing for population
    /// </summary>
    public class PopHomeMonitor : QScript, IMessageListener, IPopMonitor
    {
        // currently used to space out placement, will be replaced when people choose their homes
        public float ToHouseChance;
        private List<Person> _allPopulation;
        [SerializeField] private List<Person> _needsHousing = new List<Person>();
        private List<Person> _deserialized = new List<Person>();
        [SerializeField] private List<PopHousing> _residentHousing = new List<PopHousing>();
        private const string _nodeName = "Check";
        private const string _placeableNameSuffix = "pop_housing_";

        public int MaxCapacity;
        public int OccupiedCapacity;
        private WorldMaterials.StationInventory _stationInventory;

        [SerializeField] private TimeLength _updateFrequency;
        private int _initialCapacity;

        public void Initialize(PopulationControl control, WorldMaterials.StationInventory stationInventory, PopulationSO scriptable)
        {
            var time = Locator.WorldClock.GetSeconds(_updateFrequency);
            var node = StopWatch.AddNode(_nodeName, time);
            node.OnTick += TickPlacement;

            _allPopulation = control.AllPopulation;
            Locator.MessageHub.AddListener(this, PopHousing.MessageName);
            _stationInventory = stationInventory;
            _initialCapacity = scriptable.BaseCapacity;
        }

        private void TickPlacement()
        {
            if (!_needsHousing.Any() || !_residentHousing.Any(i => i.CurrentCapacity > i.CurrentCount))
                return;

            // add people to this list when they find homes, remove them from _needs afterwards
            var foundHomes = new List<Person>();

            foreach (var person in _needsHousing)
            {
                var home = _residentHousing.FirstOrDefault(i => i.CurrentCapacity > i.CurrentCount);
                // no homes left with room, stop trying
                if (home == null)
                    break;

                var rand = Random.value;
                if (ToHouseChance > rand)
                {
                    home.AddResident(person);
                    foundHomes.Add(person);
                }
            }

            _needsHousing.RemoveAll(i => foundHomes.Contains(i));
            UpdateCapacity();
        }

        // add to list, placed in housing during Tick()
        public void HandlePopulationUpdate(List<Person> persons, bool wasAdded)
        {
            var residents = persons.Where(i => i.IsResident);
            foreach (var resident in residents)
            {
                if (string.IsNullOrEmpty(resident.Home) || _residentHousing.All(i => i.InstanceName != resident.Home))
                    _needsHousing.Add(resident);
                else
                {
                    var home = _residentHousing.FirstOrDefault(i => i.InstanceName == resident.Home);
                    if(home != null)
                        home.AddResident(resident);
                }
            }
        }

        public void HandleDeserialization(List<Person> people)
        {
            // either queue the person to find a home, 
            var homeless = people.Where(i => string.IsNullOrEmpty(i.Home)).ToList();
            if (homeless.Any())
            {
                _needsHousing.AddRange(homeless);
            }

            //var homed = people.Except(homeless);
            //foreach (var person in homed)
            //{
            //    // find their home if it is already in the scene,
            //    if (_residentHousing.Any(i => i.name == person.Home))
            //        _residentHousing.First(i => i.name == person.Home).AddResident(person);
                
            //    // or put them aside for when the homes come in
            //    else
            //        _deserialized.Add(person);
            //}
        }

        public void HandleMessage(string type, MessageArgs args)
        {
            if (type == PopHousing.MessageName && args != null)
                HandleHousingAdd(args as PopHousingMessageArgs);
        }

        private void HandleHousingAdd(PopHousingMessageArgs args)
        {
            if(args == null && args.PopHome == null)
                throw new UnityException("PopHomeMonitor given bad message args");

            var home = args.PopHome;
            
            if (home.IsForResidents)
                _residentHousing.Add(home);
            home.OnRemove += HandleHousingRemove;

            // housing data is propagated as the Placeables are placed
            if (_deserialized.Any())
                CheckDeserialized(home);

            UpdateCapacity();
        }

        // see if anyone is waiting to move back in
        // place in home and remove them from waiting list if so
        private void CheckDeserialized(PopHousing home)
        {
            var waitingForHousing = _deserialized.Where(i => i.Home == home.InstanceName).ToList();
            if (!waitingForHousing.Any())
                return;

            waitingForHousing.ForEach(home.ResumeResident);
            _deserialized.RemoveAll(i => waitingForHousing.Contains(i));
        }

        // put all residents living there back in _needsHousing
        private void HandleHousingRemove(PopHousing housing)
        {
            _residentHousing.Remove(housing);
            var homeless = _allPopulation.Where(i => i.Home == housing.InstanceName);
            _needsHousing.AddRange(homeless);
            UpdateCapacity();
        }

        private void UpdateCapacity()
        {
            var previousMax = MaxCapacity;
            MaxCapacity = _residentHousing.Sum(i => i.CurrentCapacity) + _initialCapacity;
            OccupiedCapacity = _residentHousing.Sum(i => i.CurrentCount);

            if(previousMax != MaxCapacity)
                _stationInventory.Products.SetProductMaxAmount(ProductIdLookup.Population, MaxCapacity);
        }

        public string Name { get { return "PopHomeMonitor"; } }
    }
}