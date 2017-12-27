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

namespace Assets.Station.Population
{
    public class PeopleHouser : QScript, IMessageListener
    {
        // currently used to space out placement, will be replaced when people choose their homes
        public float ToHouseChance;
        private List<Person> _allPopulation;
        [SerializeField] private List<Person> _needsHousing = new List<Person>();
        [SerializeField] private List<PopHousing> _residentHousing = new List<PopHousing>();
        private const string _nodeName = "Check";

        public int MaxCapacity;
        public int OccupiedCapacity;
        private Inventory _inventory;

        public void Initialize(PopulationControl control, Inventory inventory)
        {
            control.OnPopulationUpdated += HandlePopulationUpdate;
            var node = StopWatch.AddNode(_nodeName, 5);
            node.OnTick += HandleTimeTick;
            _allPopulation = control.AllPopulation;
            Locator.MessageHub.AddListener(this, PopHousing.MessageName);
            _inventory = inventory;
        }

        private void HandleTimeTick()
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
        }

        // add to list, placed in housing during Tick()
        // Handles both new pop and pop coming from serialization
        private void HandlePopulationUpdate(List<Person> persons, bool wasAdded)
        {
            var residents = persons.Where(i => i.IsResident);
            foreach (var resident in residents)
            {
                if (string.IsNullOrEmpty(resident.Home) || _residentHousing.All(i => i.name != resident.Home))
                    _needsHousing.Add(resident);
                else
                {
                    var home = _residentHousing.FirstOrDefault(i => i.name == resident.Home);
                    if(home != null)
                        home.AddResident(resident);
                }
            }
        }

        public void HandleMessage(string type, MessageArgs args)
        {
            if (type == PopHousing.MessageName && args != null)
                HandleHousingAdd(args as PopHousingMessageArgs);
        }

        private void HandleHousingAdd(PopHousingMessageArgs args)
        {
            if(args == null && args.PopHome == null)
                throw new UnityException("PeopleHouser given bad message args");

            var home = args.PopHome;

            if(home.IsForResidents)
                _residentHousing.Add(home);
            home.OnRemove += HandleHousingRemove;
            var waitingForHousing = _needsHousing.Where(i => i.Home == home.name).ToList();
            if(waitingForHousing.Any())
                waitingForHousing.ForEach(i => home.AddResident(i));

            UpdateCapacity();
        }

        private void HandleHousingRemove(PopHousing housing)
        {
            _residentHousing.Remove(housing);
            var homeless = _allPopulation.Where(i => i.Home == housing.name);
            _needsHousing.AddRange(homeless);
            UpdateCapacity();
        }

        private void UpdateCapacity()
        {
            MaxCapacity = _residentHousing.Sum(i => i.CurrentCapacity);
            OccupiedCapacity = _residentHousing.Sum(i => i.CurrentCount);
            _inventory.SetProductMaxAmount(ProductIdLookup.Population, MaxCapacity);
        }

        public string Name { get { return "PeopleHouser"; } }
    }
}