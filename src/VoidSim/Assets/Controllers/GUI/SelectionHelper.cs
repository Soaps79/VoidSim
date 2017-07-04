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
		private bool _isClicked;
		private GameObject _selectable;

		public void Bind(GameObject view, GameObject selectable = null)
		{
			_selectable = selectable ?? view;
			_view = view;
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