using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Placeables.Nodes;
using Assets.Placeables.UI;
using Assets.Scripts;
using Assets.WorldMaterials.Population;
using Messaging;
using QGame;
using TMPro;
using UnityEngine;

namespace Assets.Station.Population
{
    [RequireComponent(typeof(PopulationControl))]
    public class PeopleMover : QScript, IMessageListener, IPopMonitor
    {
        private readonly List<Person> _allPopulation = new List<Person>();
        private readonly List<PopContainerSet> _containersets = new List<PopContainerSet>();

        private readonly Dictionary<PersonNeedsType, List<PopContainer>> _containersByNeed 
            = new Dictionary<PersonNeedsType, List<PopContainer>>();

        private readonly Dictionary<string, PopContainer> _containersByName 
            = new Dictionary<string, PopContainer>();

        private readonly Dictionary<string, PopContainer> _containersByPlaceableName
            = new Dictionary<string, PopContainer>();

        private readonly Dictionary<string, PopContainer> _employerContainers
            = new Dictionary<string, PopContainer>();

        private PopulationControl _control;

        internal void Initialize(PopulationControl populationControl)
        {
            _control = populationControl;
        }

        void Start()
        {
            Locator.MessageHub.AddListener(this, PopContainerSet.MessageName);
            var node = StopWatch.AddNode("mover", 2.0f);
            node.OnTick += CheckForPeopleToMove;
        }

        private void CheckForPeopleToMove()
        {
            // should be for people just arriving at the station, or on level start
            var hasNoLocation = _control.AllPopulation.Where(i => string.IsNullOrEmpty(i.CurrentlyOccupying));
            if (hasNoLocation.Any())
            {
                foreach (var person in hasNoLocation)
                {
                    // the read to work loop will pick these people up
                    if(person.ReadyToWork && !string.IsNullOrEmpty(person.Employer))
                        continue;

                    if(!string.IsNullOrEmpty(person.Home))
                        SendPersonHome(person);
                }
            }

            // if they are ready to work, find their job and send them there
            var readyToWork = _control.AllPopulation.Where(i => i.ReadyToWork);
            if (readyToWork.Any())
            {
                foreach (var person in readyToWork)
                {
                    if (string.IsNullOrEmpty(person.Employer))
                        continue;

                    RemovePersonFromCurrentLocation(person);

                    if(!_employerContainers.ContainsKey(person.Employer))
                        throw new UnityException(string.Format("Employer container {0} not found", person.Employer));

                    var employer = _employerContainers[person.Employer];
                    employer.AddPerson(person);
                    person.ReadyToWork = false;
                }
            }

            var wantsToMove = _control.AllPopulation.Where(i => i.NeedsFulfillment);
            if (wantsToMove.Any())
            {
                foreach (var person in wantsToMove)
                {
                    foreach (var need in person.UnfulfilledNeeds)
                    {
                        if (need.Type == PersonNeedsType.Rest && !string.IsNullOrEmpty(person.Home))
                            SendPersonHome(person);

                        else if (TryFulfillNeed(need, person))
                            break;
                    }
                }
            }
        }

        private void SendPersonHome(Person person)
        {
            if(!_containersByPlaceableName.ContainsKey(person.Home))
                throw new UnityException(string.Format("Home container {0} not found", person.Home));

            RemovePersonFromCurrentLocation(person);
            var container = _containersByPlaceableName[person.Home];
            container.AddPerson(person);
            person.NeedsFulfillment = false;
        }

        private void RemovePersonFromCurrentLocation(Person person)
        {
            if (!string.IsNullOrEmpty(person.CurrentlyOccupying) &&
                _containersByName.ContainsKey(person.CurrentlyOccupying))
            {
                _containersByName[person.CurrentlyOccupying].RemovePerson(person);
                person.CurrentlyOccupying = string.Empty;
            }
        }

        // see if there are any containers to fulfill the person's need
        private bool TryFulfillNeed(NeedsValue need, Person person)
        {
            if (!_containersByNeed.ContainsKey(need.Type))
            {
                UberDebug.LogChannel(LogChannels.Population, string.Format("PeopleMover does not have a container for need: {0}", need.Type));
                return false;
            }

            foreach (var popContainer in _containersByNeed[need.Type])
            {
                if(!popContainer.HasRoom)
                    continue;

                RemovePersonFromCurrentLocation(person);
                popContainer.AddPerson(person);
                person.NeedsFulfillment = false;
                return true;
            }

            return false;
        }

        public void HandleMessage(string type, MessageArgs args)
        {
            if (type == PopContainerSet.MessageName && args != null)
                HandleContainer(args as PopContainerSetMessageArgs);
        }

        public string Name { get { return "PeopleMover"; } }

        private void HandleContainer(PopContainerSetMessageArgs args)
        {
            if (args == null || args.PopContainerSet == null)
                throw new UnityException("PeopleMover given bad message data");

            _containersets.Add(args.PopContainerSet);
            args.PopContainerSet.OnRemove += set => OnRemove(args.PopContainerSet);
            args.PopContainerSet.OnContainersUpdated += OnContainerUpdate;
            UpdateContainers();
        }

        private void OnContainerUpdate(List<PopContainer> obj)
        {
            UpdateContainers();
        }

        private void UpdateContainers()
        {
            _containersByName.Clear();
            _employerContainers.Clear();
            _containersByPlaceableName.Clear();
            ResetNeedsContainerTable();

            foreach (var containerSet in _containersets)
            {
                foreach (var container in containerSet.Containers)
                {
                    _containersByName.Add(container.Name, container);
                    _containersByPlaceableName.Add(container.PlaceableName, container);

                    // hold all employers
                    if (container.Type == PopContainerType.Employment)
                    {
                        _employerContainers.Add(container.PlaceableName, container);
                    }
                    // hold all service
                    else
                    {
                        foreach (var affector in container.Affectors)
                        {
                            if (affector.Value > 0 && !_containersByNeed[affector.Type].Contains(container))
                                _containersByNeed[affector.Type].Add(container);
                        }
                    }
                }
            }
        }

        private void ResetNeedsContainerTable()
        {
            _containersByNeed.Clear();
            foreach (PersonNeedsType value in Enum.GetValues(typeof(PersonNeedsType)))
            {
                _containersByNeed.Add(value, new List<PopContainer>());
            }
        }

        private void OnRemove(PopContainerSet viewModel)
        {
            _containersets.Remove(viewModel);
            UpdateContainers();
        }

        public void HandlePopulationUpdate(List<Person> people, bool wasAdded)
        {
            
        }

        public void HandleDeserialization(List<Person> people)
        {
            
        }
    }
}