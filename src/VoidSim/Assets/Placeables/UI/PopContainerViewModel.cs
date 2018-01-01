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
        
        private int _childCount;

        public void Initialize(PopContainer popContainer)
        {
            if(_personPrefab == null)
                throw new UnityException("PopContainerViewModel missing static data");

            for (int i = 0; i < popContainer.CurrentOccupants.Count; i++)
            {
                AddOccupantAvatar(popContainer.Reserved > i, popContainer.CurrentOccupants[i]);
            }

            if (_childCount < popContainer.Reserved)
            {
                var reservedToDisplay = popContainer.Reserved - _childCount;
                for (int i = 0; i < reservedToDisplay; i++)
                {
                    AddOccupantAvatar(true);
                }
            }

            if (_childCount < popContainer.MaxCapacity)
            {
                var emptyCount = popContainer.MaxCapacity - _childCount;
                for (int i = 0; i < emptyCount; i++)
                {
                    AddOccupantAvatar(false);
                }
            }
        }

        private void AddOccupantAvatar(bool isReserved, Person person = null)
        {
            var avatar = Instantiate(_personPrefab, transform, false);
            avatar.Initialize(isReserved, person);
            _childCount++;
        }
    }
}