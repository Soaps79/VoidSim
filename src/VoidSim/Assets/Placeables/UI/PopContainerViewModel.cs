using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Placeables.Nodes;
using Assets.WorldMaterials.Population;
using QGame;
using UIWidgets;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Placeables.UI
{
    /// <summary>
    /// Represents the pop container placeable node.
    /// </summary>
    public class PopContainerViewModel : TileView<OccupantViewModel, Occupancy>
    {
        private PopContainer _container;
        private int _lastReserveCount;
        private readonly List<OccupantViewModel> _occupants = new List<OccupantViewModel>();

        public void Initialize(PopContainer popContainer)
        {
            DataSource = popContainer.CurrentOccupancy.ToObservableList();
            _container = popContainer;
            popContainer.OnUpdate += UpdateContainer;

            UpdateContainer();
        }

        private void RedrawAll()
        {
            ClearOccupants();

            foreach (var occupancy in _container.CurrentOccupancy)
            {
                AddOccupantAvatar(occupancy);
            }
        }

        void Update()
        {
            int i = 9;
        }

        private void ClearOccupants()
        {
            if (_occupants.Count == 0)
                return;

            for (int i = 0; i < _occupants.Count; i++)
            {
                Destroy(_occupants[i].gameObject);
            }
            _occupants.Clear();
        }

        private void AddOccupantAvatar(Occupancy occupancy)
        {
            //var avatar = Instantiate(_personPrefab, transform, false);
            //avatar.Initialize(occupancy);
            //_occupants.Add(avatar);
        }

        private void UpdateContainer()
        {
            DataSource = _container.CurrentOccupancy.ToObservableList();
            var rows = DataSource.Count / Layout.GridConstraintCount;
            var rect = GetComponent<RectTransform>();
            var width = Layout.GridConstraintCount * (itemWidth + Layout.Spacing.x) + Layout.GetMarginLeft() + Layout.GetMarginRight();
            //width -= Layout.Spacing.x;
            var height = rows * (itemHeight + Layout.Spacing.y) + Layout.GetMarginTop() + Layout.GetMarginBottom();
            //height -= Layout.Spacing.y;
            rect.sizeDelta = new Vector2(width, height);
            
            //if (_container.MaxCapacity != _occupants.Count)
            //    RedrawAll();
            //else if (_lastReserveCount != _container.Reserved)
            //    UpdateAll();
        }

        //private void UpdateAll()
        //{
        //    var count = _container.CurrentOccupants.Count;
        //    for (int i = 0; i < _occupants.Count; i++)
        //    {
        //        _occupants[i].UpdateValues();
        //    }
        //}
    }
}