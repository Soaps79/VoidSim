using System;
using Assets.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Controllers
{
    public class SelectionController : SingletonBehavior<SelectionController>
    {
        public Action<GameObject, GameObject> OnSelectedChanged;

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

        public void SetSelected(GameObject selected)
        {
            SelectedObject = selected;
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
