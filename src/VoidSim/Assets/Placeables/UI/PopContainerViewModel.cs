using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Placeables.Nodes;
using Assets.WorldMaterials.Population;
using QGame;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Placeables.UI
{
    /// <summary>
    /// Represents the pop container placeable node.
    /// </summary>
    public class PopContainerViewModel : QScript
    {
        [SerializeField] private OccupantViewModel _personPrefab;
        
        private PopContainer _container;
        private int _lastReserveCount;
        private readonly List<OccupantViewModel> _occupants = new List<OccupantViewModel>();

        public void Initialize(PopContainer popContainer)
        {
            _container = popContainer;
            popContainer.OnUpdate += HandleContainerUpdate;

            if (_personPrefab == null)
                throw new UnityException("PopContainerViewModel missing static data");

            RedrawAll();
        }

        private void RedrawAll()
        {
            ClearOccupants();

            foreach (var occupancy in _container.CurrentOccupancy)
            {
                AddOccupantAvatar(occupancy);
            }
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
            var avatar = Instantiate(_personPrefab, transform, false);
            avatar.Initialize(occupancy);
            _occupants.Add(avatar);
        }

        private void HandleContainerUpdate()
        {
            if(_container.MaxCapacity != _occupants.Count)
                RedrawAll();
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