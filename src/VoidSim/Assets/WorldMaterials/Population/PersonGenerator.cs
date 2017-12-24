using System.Collections.Generic;
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
        private const string _lastIdName = "PersonGenerator";

        public void Initialize(GenerationParams genParams)
        {
            _minNames = genParams.MinNamesLoaded;
            _maxNames = genParams.MaxNamesLoaded;

            PopulateNameLists();
        }

        #region Name Management

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
            var isMale = Random.value > .5f;
            return new Person
            {
                Id = Locator.LastId.GetNext(_lastIdName),
                FirstName = isMale ? Pop(_maleNames) : Pop(_femaleNames),
                LastName = Pop(_lastNames),
                IsMale = isMale
            };
        }

        private string Pop(List<string> names)
        {
            var name = names[names.Count - 1];
            names.RemoveAt(names.Count - 1);
            return name;
        }

        #endregion
    }
}