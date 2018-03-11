using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    public class NeedsContainerEntry
    {
        public PersonNeedsType Type;
        public float Value;
        public PopContainer PopContainer;
    }

    [RequireComponent(typeof(PopulationControl))]
    public class PeopleMover : QScript, IMessageListener, IPopMonitor
    {
        private readonly List<Person> _allPopulation = new List<Person>();
        private readonly List<PopContainerSet> _containersets = new List<PopContainerSet>();

        private readonly Dictionary<PersonNeedsType, List<NeedsContainerEntry>> _containersByNeed 
            = new Dictionary<PersonNeedsType, List<NeedsContainerEntry>>();

        private readonly Dictionary<string, PopContainer> _containersByName 
            = new Dictionary<string, PopContainer>();

        private readonly Dictionary<string, List<PopContainer>> _containersByPlaceableName
            = new Dictionary<string, List<PopContainer>>();

        // replace with a dictionary
        private readonly Dictionary<string, PopContainer> _employerContainers
            = new Dictionary<string, PopContainer>();

        private PopulationControl _control;
        private WaitForSeconds _enumerator;
        private const float _tickTime = 2.0f;

        internal void Initialize(PopulationControl populationControl)
        {
            _control = populationControl;
        }

        void Start()
        {
            Locator.MessageHub.AddListener(this, PopContainerSet.MessageName);
            var node = StopWatch.AddNode("PeopleMover", _tickTime);
            node.OnTick += () => StartCoroutine(CheckForPeopleToMove());
        }

        private float _startTime;

        private void StartTimer()
        {
            _startTime = Time.time;
        }

        private void CheckTimer(int step)
        {
            UberDebug.LogChannel(LogChannels.Performance, string.Format("PeopleMoverUpdate {0}: {1}", step, Time.time - _startTime));
        }

        private IEnumerator CheckForPeopleToMove()
        {
            var startTime = Time.time;
            StartTimer();

            // should be for people just arriving at the station, or on level start
            var hasNoLocation = _control.AllPopulation.Where(i => string.IsNullOrEmpty(i.CurrentlyOccupying)).ToList();
            CheckTimer(1);
            yield return null;

            StartTimer();
            if (hasNoLocation.Any())
            {
                for (int i = 0; i < hasNoLocation.Count(); i++)
                {
                    var person = hasNoLocation[i];
                    // the ready to work loop will pick these people up
                    if (person.Wants.IsRequesting(PopContainerType.Employment) && !string.IsNullOrEmpty(person.Employer))
                        continue;

                    if(!string.IsNullOrEmpty(person.Home))
                        SendPersonHome(person);

                    yield return null;
                }
            }
            CheckTimer(2);
            yield return null;

            StartTimer();
            // if they are ready to work, find their job and send them there
            var readyToWork = _control.AllPopulation.Where(i => i.Wants.IsRequesting(PopContainerType.Employment)).ToList();
            CheckTimer(3);
            yield return null;

            StartTimer();
            if (readyToWork.Any())
            {
                for (int i = 0; i < readyToWork.Count; i++)
                {
                    var person = readyToWork[i];
                    if (string.IsNullOrEmpty(person.Employer))
                        continue;

                    if(!_employerContainers.ContainsKey(person.Employer))
                        throw new UnityException(string.Format("Employer container {0} not found", person.Employer));

                    var employer = _employerContainers[person.Employer];
                    MovePerson(employer, person);
                    yield return null;
                }
            }
            CheckTimer(4);
            yield return null;
            
            // if they need fulfillment, try and find it
            StartTimer();
            var wantsToMove = _control.AllPopulation.Where(i => i.Wants.IsRequesting(PopContainerType.Fulfillment)).ToList();
            CheckTimer(5);
            yield return null;

            StartTimer();
            if (wantsToMove.Any())
            {
                for (int i = 0; i < wantsToMove.Count; i++)
                {
                    var person = wantsToMove[i];
                    var needs = person.Wants.GetRequested(PopContainerType.Fulfillment) as FulfillmentWant;
                    if(needs == null)
                        throw new UnityException("Person want handler passing around bad data");

                    foreach (var need in needs.UnfulfilledNeeds)
                    {
                        if (need.Type == PersonNeedsType.Rest && !string.IsNullOrEmpty(person.Home))
                            SendPersonHome(person);

                        else if (TryFulfillNeed(need, person))
                            break;
                    }
                    yield return null;
                }
            }
            CheckTimer(6);

            var elapsed = Time.time - startTime;
            if(elapsed > _tickTime)
                throw new UnityException("PeopleMover took longer to move people than its given tickTime, problems coming.");

            UberDebug.LogChannel(LogChannels.Performance, string.Format("PeopleMoverUpdate Total: {0}", elapsed));
        }

        private PopContainerDetails _details = new PopContainerDetails();
        private readonly StringBuilder _stringBuilder = new StringBuilder(255);

        private void MovePerson(PopContainer newContainer, Person person)
        {
            RemovePersonFromCurrentLocation(person);

            newContainer.AddPerson(person);
            person.CurrentlyOccupying = newContainer.Name;
            _stringBuilder.Length = 0;
            _stringBuilder.Append(newContainer.ActivityPrefix);
            _stringBuilder.Append(" at ");
            _stringBuilder.Append(newContainer.PlaceableName);
            person.CurrentActivity = _stringBuilder.ToString();

            _details.Name = newContainer.Name;
            _details.PlaceableName = newContainer.PlaceableName;
            _details.Type = newContainer.Type;
            _details.Affectors = newContainer.Affectors;

            person.HandleLocationChange(_details);
        }

        private void RemovePersonFromCurrentLocation(Person person)
        {
            if (!string.IsNullOrEmpty(person.CurrentlyOccupying) &&
                _containersByName.ContainsKey(person.CurrentlyOccupying))
            {
                _containersByName[person.CurrentlyOccupying].RemovePerson(person);
            }

            person.CurrentlyOccupying = string.Empty;
            person.CurrentActivity = string.Empty;
        }

        private void SendPersonHome(Person person)
        {
            // make sure the home placeable exists
            if(!_containersByPlaceableName.ContainsKey(person.Home))
                throw new UnityException(string.Format("Home container {0} not found", person.Home));

            // and that it has a Fulfillment container
            var container = _containersByPlaceableName[person.Home].FirstOrDefault(i => i.Type == PopContainerType.Fulfillment);
            if(container == null)
                throw new UnityException(string.Format("No registered Fulfillment container for PopHousing {0}", person.Home));

            // if found, remove person from current location and send them there
            MovePerson(container, person);
        }

        // see if there are any containers to fulfill the person's need
        private bool TryFulfillNeed(NeedsValue need, Person person)
        {
            if (!_containersByNeed.ContainsKey(need.Type))
            {
                UberDebug.LogChannel(LogChannels.Population, string.Format("PeopleMover does not have a container for need: {0}", need.Type));
                return false;
            }

            foreach (var needsContainer in _containersByNeed[need.Type])
            {
                if(!needsContainer.PopContainer.HasRoom)
                    continue;

                MovePerson(needsContainer.PopContainer, person);
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
                    if(!_containersByPlaceableName.ContainsKey(container.PlaceableName))
                        _containersByPlaceableName.Add(container.PlaceableName, new List<PopContainer>());

                    _containersByPlaceableName[container.PlaceableName].Add(container);

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
                            if (affector.Value > 0)
                                _containersByNeed[affector.Type].Add(new NeedsContainerEntry
                                {
                                    PopContainer = container,
                                    Type = affector.Type,
                                    Value = affector.Value
                                });
                        }
                    }

                    // check for any deserialized people that need placement
                    if (_deserialized.Any() && _deserialized.ContainsKey(container.Name))
                    {
                        foreach (var person in _deserialized[container.Name])
                        {
                            container.ResumePerson(person);
                        }
                        _deserialized.Remove(container.Name);
                    }
                }
            }

            // order services by their value
            foreach (var pair in _containersByNeed)
            {
                var list = pair.Value.ToList();
                pair.Value.Clear();
                pair.Value.AddRange(list.OrderByDescending(i => i.Value));
            }
        }

        private void ResetNeedsContainerTable()
        {
            _containersByNeed.Clear();
            foreach (PersonNeedsType value in Enum.GetValues(typeof(PersonNeedsType)))
            {
                _containersByNeed.Add(value, new List<NeedsContainerEntry>());
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

        private readonly Dictionary<string, List<Person>> _deserialized = new Dictionary<string, List<Person>>();

        public void HandleDeserialization(List<Person> people)
        {
            for (int i = 0; i < people.Count; i++)
            {
                if (!string.IsNullOrEmpty(people[i].CurrentlyOccupying))
                {
                    if (!_deserialized.ContainsKey(people[i].CurrentlyOccupying))
                    {
                        _deserialized.Add(people[i].CurrentlyOccupying, new List<Person>());
                    }
                    _deserialized[people[i].CurrentlyOccupying].Add(people[i]);
                }
            }
        }
    }
}