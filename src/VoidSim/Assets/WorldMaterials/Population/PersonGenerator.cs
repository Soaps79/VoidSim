using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Assets.Scripts;
using UnityEngine;

namespace Assets.WorldMaterials.Population
{
    public class PersonGenerator
    {
        private int _maxNames;
        private int _minNames;

        private readonly List<string> _maleNames = new List<string>();
        private readonly List<string> _femaleNames = new List<string>();
        private readonly List<string> _lastNames = new List<string>();
        private Dictionary<PersonNeedsType, PersonNeedsInfo> _needsTemplate;
        private const string _lastIdName = "PersonGenerator";

        public void Initialize(GenerationParams genParams)
        {
            _minNames = genParams.MinNamesLoaded;
            _maxNames = genParams.MaxNamesLoaded;
            _needsTemplate = genParams.ResidentNeeds.ToDictionary(i => i.Type); ;

            PopulateNameLists();
        }

        #region Name Management

        // pulls name lists from their associated text files
        private void PopulateNameLists()
        {
            TextAsset textAsset = null;
            string[] names = null;
            if (_maleNames.Count < _maxNames)
            {
                textAsset = Resources.Load<TextAsset>("names_male");
                names = ExtractNames(textAsset.text);
                PopulateList(_maleNames, names);
            }
            if (_femaleNames.Count < _maxNames)
            {
                textAsset = Resources.Load<TextAsset>("names_female");
                names = ExtractNames(textAsset.text);
                PopulateList(_femaleNames, names);
            }
            if (_lastNames.Count < _maxNames)
            {
                textAsset = Resources.Load<TextAsset>("names_last");
                names = ExtractNames(textAsset.text);
                PopulateList(_lastNames, names);
            }
        }

        private static string[] ExtractNames(string names)
        {
            return Regex.Split(names, "\r\n");
        }

        // randomly pulls names from the greater list
        private void PopulateList(List<string> knownNames, string[] allNames)
        {
            var needed = (float)_maxNames - knownNames.Count;
            var length = allNames.Length;

            // from https://stackoverflow.com/a/48089
            for (int i = 0; i < allNames.Length; i++)
            {
                var rand = Random.value;
                var probability = needed / (length - i);
                if (probability > rand)
                {
                    knownNames.Add(allNames[i]);
                    needed -= 1;
                    if (knownNames.Count >= _maxNames)
                        break;
                }
            }
        }

        private void CheckLists()
        {
            if(_maleNames.Count < _minNames
                || _femaleNames.Count < _minNames
                || _lastNames.Count < _minNames)
                PopulateNameLists();
        }
        #endregion

        #region Making People

        public List<Person> GeneratePeople(int count)
        {
            if (count > _lastNames.Count)
            {
                var message = string.Format("People Generator - more people requested ({0}) than stored ({1}). Raise limit in Pop SO", count, _lastNames.Count);
                UberDebug.LogChannel(LogChannels.Population, message);
            }

            var list = new List<Person>();
            for (int i = 0; i < count; i++)
            {
                list.Add(GeneratePerson());
            }
            CheckLists();

            return list;
        }

        private Person GeneratePerson()
        {
            // generate basic person data
            var isMale = Random.value > .5f;
            var person =new Person
            {
                Id = Locator.LastId.GetNext(_lastIdName),
                FirstName = isMale ? Pop(_maleNames) : Pop(_femaleNames),
                LastName = Pop(_lastNames),
                IsMale = isMale
            };

            // match static data from SO with random values
            var needs = new List<PersonNeeds>();
            foreach (var template in _needsTemplate.Values)
            {
                var value = template.MinInitialValue + 
                    Random.value * (template.MaxInitialValue - template.MinInitialValue);

                needs.Add(new PersonNeeds
                {
                    MinValue = _needsTemplate[template.Type].MinValue,
                    MaxValue = _needsTemplate[template.Type].MaxValue,
                    MinTolerance = _needsTemplate[template.Type].MinTolerance,
                    CurrentValue = value,
                    Type = template.Type,
                    MinFulfillment = template.MinFulfillment,
                    StartWantingToMove = template.StartingWantToMove
                });
            }
            person.SetNeeds(needs);

            return person;
        }

        private string Pop(List<string> names)
        {
            var name = names[names.Count - 1];
            names.RemoveAt(names.Count - 1);
            return name;
        }

        // hydrates Person objects with static data from SO and deserialized data from file
        public List<Person> DeserializePopulation(List<PersonData> personDatas)
        {
            var people = new List<Person>();

            foreach (var personData in personDatas)
            {
                var person = new Person(personData);
                var needs = new List<PersonNeeds>();
                foreach (var need in personData.Needs)
                {
                    needs.Add(new PersonNeeds
                    {
                        MinValue = _needsTemplate[need.Type].MinValue,
                        MaxValue = _needsTemplate[need.Type].MaxValue,
                        CurrentValue = need.CurrentValue,
                        Type = need.Type,
                        MinFulfillment = _needsTemplate[need.Type].MinFulfillment,
                        StartWantingToMove = _needsTemplate[need.Type].StartingWantToMove,
                        MinTolerance = _needsTemplate[need.Type].MinTolerance
                    });
                }
                person.SetNeeds(needs);
                people.Add(person);
            }

            return people;
        }

        #endregion
    }
}