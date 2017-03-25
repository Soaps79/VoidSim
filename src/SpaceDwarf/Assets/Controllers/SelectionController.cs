using System;
using System.Collections.Generic;
using QGame;
using UnityEngine;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

namespace Assets.Controllers
{
    public class SelectionController : SingletonBehavior<SelectionController>
    {
        public Action<GameObject, GameObject> OnSelectedChanged;

        public List<string> SelectionLayers;
        private Dictionary<string, bool> _layerDictionary;

        private GameObject _selectedObject;

        public GameObject SelectedObject
        {
            get
            {
                return _selectedObject;
            }
            private set
            {
                if (HasChanged(_selectedObject, value))
                {
                    var prev = _selectedObject;
                    _selectedObject = value;
                    if (OnSelectedChanged != null)
                    {
                        OnSelectedChanged(prev, _selectedObject);
                    }
                }
            }
        }

        protected override void OnStart()
        {
            base.OnStart();

            // create dictionary of activated layers, default to all to false
            _layerDictionary = CreateLayerDictionary();
        }

        private Dictionary<string, bool> CreateLayerDictionary()
        {
            var layerDictionary = new Dictionary<string, bool>();
            for (var i = 0; i < SelectionLayers.Count; i++)
            {
                layerDictionary.Add(SelectionLayers[i], false);
            }
            return layerDictionary;
        }

        public bool CanSelect(string layer)
        {
            return _layerDictionary[layer];
        }

        public void SetCanSelect(string layer, bool value)
        {
            _layerDictionary[layer] = value;
        }

        public void SetSelected(GameObject selected)
        {
            SelectedObject = selected;
            EventSystem.current.SetSelectedGameObject(selected);
        }

        public void Clear()
        {
            _selectedObject = null;
        }

        private static bool HasChanged(Object prev, Object next)
        {
            return (prev != null && next == null)
                   || (prev == null && next != null)
                   || (prev != null && prev.name != next.name);
        }
    }
}
