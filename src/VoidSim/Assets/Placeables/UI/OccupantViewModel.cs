using Assets.Placeables.Nodes;
using Assets.WorldMaterials.Population;
using QGame;
using UIWidgets;
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
    public class OccupantViewModel : QScript //ListViewItem, IViewData<Person>
    {
        [SerializeField] private Sprite _spriteFilled;
        [SerializeField] private Sprite _spriteOutlineFilled;
        [SerializeField] private Sprite _spriteOutlineEmpty;
        [SerializeField] private Color _residentColor;
        private Occupancy _occupancy;

        public void Initialize(Occupancy occupancy)
        {
            _occupancy = occupancy;
            _occupancy.OnUpdate += HandleOccupancyUpdate;

            var trigger = GetComponent<BoundTooltipTrigger>();
            trigger.OnHoverActivate += HandleTooltipActivate;

            UpdateSprite();
        }

        private void HandleOccupancyUpdate(Occupancy obj)
        {
            UpdateSprite();
        }

        private void UpdateSprite()
        {
            var img = GetComponent<Image>();
            img.color = _residentColor;
            if (_occupancy.IsOccupied)
                img.sprite = _spriteFilled;
            else if (_occupancy.IsReserved)
                img.sprite = _spriteOutlineFilled;
            else
                img.sprite = _spriteOutlineEmpty;
        }

        private void HandleTooltipActivate(BoundTooltipTrigger trigger)
        {
            if (!_occupancy.IsOccupied)
            {
                trigger.text = _occupancy.IsReserved ? "Reserved" : "Open";
                return;
            }

            var text = _occupancy.Person.FirstName + " " + _occupancy.Person.LastName + " - " + (_occupancy.Person.IsMale ? "Male" : "Female");
            trigger.text = text;
        }
    }
}