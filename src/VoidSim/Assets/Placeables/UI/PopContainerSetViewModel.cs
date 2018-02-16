using System.Collections.Generic;
using Assets.Placeables.Nodes;
using QGame;
using UnityEngine;

namespace Assets.Placeables.UI
{
    /// <summary>
    /// Represents the PopContainerSet placeable node. Refreshes entire array on changes
    /// </summary>
    public class PopContainerSetViewModel : QScript
    {
        [SerializeField] private PopContainerViewModel _containerPrefab;
        [SerializeField] private Vector2 _offset;
        [SerializeField] private Transform _containerParent;
        
        private PopContainerSet _containerSet;
        private readonly List<PopContainerViewModel> _containers = new List<PopContainerViewModel>();

        public void Initialize(PopContainerSet containerSet)
        {
            _containerSet = containerSet;
            _containerSet.OnContainersUpdated += UpdateContainers;
            transform.position = new Vector3 
                    { 
                       x = containerSet.transform.position.x + _offset.x,
                       y = containerSet.transform.position.y + _offset.y,
                       z = transform.position.z
                    };
            UpdateContainers(containerSet.Containers);
        }

        private void UpdateContainers(List<PopContainer> containers)
        {
            // written assuming that containers are created once and remain for the duration of the placeable
            // will need updating to handle further changes
            if (containers.Count != _containers.Count)
                RedrawContainers(containers);
        }

        private void RedrawContainers(List<PopContainer> containers)
        {
            ClearExisting();
            foreach (var popContainer in containers)
            {
                var viewmodel = Instantiate(_containerPrefab, _containerParent, false);
                viewmodel.name = popContainer.Name;
                viewmodel.Initialize(popContainer);
            }
        }

        private void ClearExisting()
        {
            _containers.ForEach(i => Destroy(i.gameObject));
            _containers.Clear();

            foreach (Transform child in _containerParent)
            {
                Destroy(child.gameObject);
            }
        }
    }
}