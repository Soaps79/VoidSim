using System;
using QGame;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Controllers.GUI
{
	public class SelectionHelper : QScript, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
	{
		public Action OnSelected;
		private GameObject _view;
		[SerializeField] private bool _isClicked;
		private GameObject _selectable;

		public void Bind(GameObject view, GameObject selectable = null)
		{
			_selectable = selectable ?? view;
			_view = view;
			var closeButton = _view.GetComponent<ClosePanelButton>();
			if (closeButton != null)
				closeButton.OnClose += HandleCloseByButton;
		}

		private void HandleCloseByButton()
		{
			_isClicked = false;
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			_view.SetActive(true);
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			if(!_isClicked)
				_view.SetActive(false);
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			_isClicked = true;
		}
	}
}