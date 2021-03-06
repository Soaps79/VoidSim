﻿using System;
using Assets.Placeables.Nodes;
using Assets.Scripts;
using Assets.Station.Population;
using UIWidgets;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
#pragma warning disable 649

namespace Assets.Placeables.UI
{
    /// <summary>
    /// This class represents a physical spot that cen be occupied by a person,
    /// uses different icons to represent the possible states
    /// </summary>
    [RequireComponent(typeof(Image))]
    [RequireComponent(typeof(BoundTooltipTrigger))]
    public class OccupantViewModel : ListViewItem, IViewData<Occupancy>
    {
        [SerializeField] private Sprite _spriteFilled;
        [SerializeField] private Sprite _spriteOutlineFilled;
        [SerializeField] private Sprite _spriteOutlineEmpty;
        [SerializeField] private Color _residentColor;
        private Occupancy _occupancy;

        void Start()
        {
            var trigger = GetComponent<BoundTooltipTrigger>();
            trigger.OnHoverActivate += HandleTooltipActivate;

            onSelect.AddListener(HandleSelected);
        }

        private void HandleSelected(ListViewItem item)
        {
            if (!_occupancy.IsOccupied) return;
            Locator.InfoPanelManager.AddPanel(_occupancy.OccupiedBy, transform.position);
            Locator.MessageHub.QueueMessage(PersonSelectedMessageArgs.MessageName, 
                new PersonSelectedMessageArgs { Person = _occupancy.OccupiedBy, IsSelected = true });
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

            var text = _occupancy.OccupiedBy.FirstName + " " + _occupancy.OccupiedBy.LastName + " - " + (_occupancy.OccupiedBy.IsMale ? "Male" : "Female");
            trigger.text = text;
        }

        public void SetData(Occupancy occupancy)
        {
            _occupancy = occupancy;
            _occupancy.OnUpdate += HandleOccupancyUpdate;
            UpdateSprite();
        }
    }
}