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

            for (int i = 0; i < _container.CurrentOccupants.Count; i++)
            {
                AddOccupantAvatar(_container.Reserved > i, _container.CurrentOccupants[i]);
            }

            _lastReserveCount = _container.Reserved;

            if (_occupants.Count < _container.Reserved)
            {
                var reservedToDisplay = _container.Reserved - _occupants.Count;
                for (int i = 0; i < reservedToDisplay; i++)
                {
                    AddOccupantAvatar(true);
                }
            }

            if (_occupants.Count < _container.MaxCapacity)
            {
                var emptyCount = _container.MaxCapacity - _occupants.Count;
                for (int i = 0; i < emptyCount; i++)
                {
                    AddOccupantAvatar(false);
                }
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

        private void AddOccupantAvatar(bool isReserved, Person person = null)
        {
            var avatar = Instantiate(_personPrefab, transform, false);
            avatar.Initialize(isReserved, person);
            _occupants.Add(avatar);
        }

        private void HandleContainerUpdate()
        {
            if(_container.MaxCapacity != _occupants.Count)
                RedrawAll();
            else if (_lastReserveCount != _container.Reserved)
                UpdateAll();
        }

        private void UpdateAll()
        {
            var count = _container.CurrentOccupants.Count;
            for (int i = 0; i < _occupants.Count; i++)
            {
                _occupants[i].UpdateValues(
                    i < count ? _container.CurrentOccupants[i] : null, i <= _container.Reserved);
            }
        }
    }
}