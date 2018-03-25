using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.WorldMaterials.Population
{
    /// <summary>
    /// Handles distribution of Person portraits. Portraits are simply sprites, 
    /// but this class attempt an even distribution of portraits using its GetNext functions
    /// </summary>
    public class PortraitManager
    {
        private class PortraitEntry
        {
            public string Name;
            public Sprite Sprite;
            public int Usages;
        }

        private readonly List<PortraitEntry> _malePortraits = new List<PortraitEntry>();
        private int _currentMaleMin;
        private readonly List<PortraitEntry> _femalePortraits = new List<PortraitEntry>();
        private int _currentFemaleMin;

        public void AddMaleSprites(List<Sprite> sprites)
        {
            foreach (var sprite in sprites)
            {
                _malePortraits.Add(new PortraitEntry
                {
                    Name = sprite.name,
                    Sprite = sprite
                });
            }
        }

        public void AddFemaleSprites(List<Sprite> sprites)
        {
            foreach (var sprite in sprites)
            {
                _femalePortraits.Add(new PortraitEntry
                {
                    Name = sprite.name,
                    Sprite = sprite
                });
            }
        }

        // gets the next male, randomly pulled from the least used portraits
        public Sprite GetNextMale()
        {
            var eligible = _malePortraits.Where(i => i.Usages == _currentMaleMin);
            if(!eligible.Any())
                throw new UnityException("PortraitManager male min not kept current");

            var chosen = eligible.Count() == 1 ? eligible.First() : eligible.ElementAt(Random.Range(0, eligible.Count()));
            chosen.Usages++;
            UpdateMinimums();
            return chosen.Sprite;
        }

        // gets the next female, randomly pulled from the least used portraits
        public Sprite GetNextFemale()
        {
            var eligible = _femalePortraits.Where(i => i.Usages == _currentFemaleMin);
            if (!eligible.Any())
                throw new UnityException("PortraitManager male min not kept current");

            var chosen = eligible.Count() == 1 ? eligible.First() : eligible.ElementAt(Random.Range(0, eligible.Count()));
            chosen.Usages++;
            UpdateMinimums();
            return chosen.Sprite;
        }

        // Will return sprite of a given name, will update usage unless told not to
        public Sprite GetSprite(string name, bool ignoreUsage = false)
        {
            var entry = _malePortraits.FirstOrDefault(i => i.Name == name);
            if(entry == null)
                entry = _femalePortraits.FirstOrDefault(i => i.Name == name);
            if(entry == null)
                throw new UnityException("PortraitManager asked for sprite it does not know");

            if (!ignoreUsage)
            {
                entry.Usages++;
                UpdateMinimums();
            }

            return entry.Sprite;
        }

        private void UpdateMinimums()
        {
            _currentMaleMin = _malePortraits.Min(i => i.Usages);
            _currentFemaleMin = _femalePortraits.Min(i => i.Usages);
        }
    }
}