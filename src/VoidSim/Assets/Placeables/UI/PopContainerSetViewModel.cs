using System.Collections.Generic;
using Assets.Placeables.Nodes;
using QGame;
using UnityEngine;

namespace Assets.Placeables.UI
{
    /// <summary>
    /// Represents the PopContainerSet placeable node. Refreshes entier array on changes
    /// </summary>
    public class PopContainerSetViewModel : QScript
    {
        [SerializeField] private PopContainerViewModel _containerPrefab;
        [SerializeField] private Vector2 _offset;
        [SerializeField] private Transform _containerParent;

        private PopContainerSet _containerSet;
        private readonly List<PopContainerViewModel> _containers = new List<PopContainerViewModel>();
        //private Canvas _canvas;

        public void Initialize(PopContainerSet containerSet)
        {
            _containerSet = containerSet;
            _containerSet.OnContainersUpdated += HandleContainerUpdate;
            //_canvas = transform.parent.GetComponent<Canvas>();
            transform.position = new Vector3 
                    { 
                       x = containerSet.transform.position.x + _offset.x,
                       y = containerSet.transform.position.y + _offset.y,
                       z = transform.position.z
                    };

            containerSet.OnRemove += set => Destroy(gameObject);
            //if(_canvas == null)
            //    throw new UnityException("PopContainerSetViewModel has parent that is not a canvas");
        }

        private void HandleContainerUpdate(List<PopContainer> containers)
        {
            _containers.ForEach(i => Destroy(i.gameObject));
            _containers.Clear();

            foreach (Transform child in _containerParent)
            {
                Destroy(child.gameObject);
            }

            foreach (var popContainer in containers)
            {
                var viewmodel = Instantiate(_containerPrefab, _containerParent, false);
                viewmodel.Initialize(popContainer);
            }
        }
    }
}