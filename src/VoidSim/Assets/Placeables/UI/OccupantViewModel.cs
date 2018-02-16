using Assets.WorldMaterials.Population;
using QGame;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace Assets.Placeables.UI
{
    /// <summary>
    /// This class represents a physical spot that cen be occupied by a person,
    /// uses different icons to represent the possible states
    /// </summary>
    [RequireComponent(typeof(Image))]
    [RequireComponent(typeof(BoundTooltipTrigger))]
    public class OccupantViewModel : QScript
    {
        [SerializeField] private Sprite _spriteFilled;
        [SerializeField] private Sprite _spriteOutlineFilled;
        [SerializeField] private Sprite _spriteOutlineEmpty;
        [SerializeField] private Color _residentColor;
        private Person _person;
        private bool _isReserved;

        public void Initialize(bool isReserved, Person person)
        {
            _person = person;
            _isReserved = isReserved;

            var trigger = GetComponent<BoundTooltipTrigger>();
            trigger.OnHoverActivate += HandleTooltipActivate;

            UpdateSprite();
        }

        private void UpdateSprite()
        {
            var img = GetComponent<Image>();
            img.color = _residentColor;
            if (_person != null)
                img.sprite = _spriteFilled;
            else if (_isReserved)
                img.sprite = _spriteOutlineFilled;
            else
                img.sprite = _spriteOutlineEmpty;
        }

        private void HandleTooltipActivate(BoundTooltipTrigger trigger)
        {
            if (_person == null)
            {
                trigger.text = _isReserved ? "Reserved" : "Open";
                return;
            }

            var text = _person.FirstName + " " + _person.LastName + " - " + (_person.IsMale ? "Male" : "Female");
            trigger.text = text;
        }

        public void UpdateValues(Person person, bool value)
        {
            if (_person == person && _isReserved == value)
                return;

            _person = person;
            _isReserved = value;
            UpdateSprite();
        }
    }
}