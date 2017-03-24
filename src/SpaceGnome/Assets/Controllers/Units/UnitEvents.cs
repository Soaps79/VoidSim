using Assets.Views.Units;
using QGame;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Controllers.Units
{
    public class UnitEvents : OrderedEventBehavior, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, ISelectHandler, IDeselectHandler
    {
        private UnitViewDetails _viewDetails;
        private SpriteRenderer _renderer;

        protected override void OnStart()
        {
            base.OnStart();
            var behavior = GetComponent<UnitBehavior>();
            _viewDetails = behavior.ViewDetails;
            _renderer = GetComponent<SpriteRenderer>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            // set the sprite renderer material to Highlighted
            _renderer.material = _viewDetails.HighlightedMaterial;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            // set selected or default material, based on selection
            var material = _viewDetails.DefaultMaterial;
            var selected = EventSystem.current.currentSelectedGameObject;
            if (selected != null && _renderer.gameObject.name == selected.name)
            {
                material = _viewDetails.SelectedMaterial;
            }

            _renderer.material = material;
        }

        public void OnSelect(BaseEventData eventData)
        {
            Debug.Log(string.Format("OnSelect: {0}", eventData.selectedObject.name));
            _renderer.material = _viewDetails.SelectedMaterial;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            // set this as the selected object
            EventSystem.current.SetSelectedGameObject(eventData.pointerPress);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            _renderer.material = _viewDetails.DefaultMaterial;
        }
    }
}
