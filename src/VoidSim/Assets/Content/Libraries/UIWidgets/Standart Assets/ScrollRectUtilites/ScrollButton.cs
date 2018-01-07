﻿using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UIWidgets
{
	/// <summary>
	/// Scroll button.
	/// </summary>
	public class ScrollButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
	{
		/// <summary>
		/// OnClick event.
		/// </summary>
		public UnityEvent OnClick = new UnityEvent();

		/// <summary>
		/// OnDown.
		/// </summary>
		public UnityEvent OnDown = new UnityEvent();

		/// <summary>
		/// OnUp.
		/// </summary>
		public UnityEvent OnUp = new UnityEvent();

		/// <summary>
		/// Raises the pointer click event.
		/// </summary>
		/// <param name="eventData">Current event data.</param>
		public void OnPointerClick(PointerEventData eventData)
		{
			OnClick.Invoke();
		}

		/// <summary>
		/// Raises the pointer down event.
		/// </summary>
		/// <param name="eventData">Current event data.</param>
		public virtual void OnPointerDown(PointerEventData eventData)
		{
			OnDown.Invoke();
		}

		/// <summary>
		/// Raises the pointer up event.
		/// </summary>
		/// <param name="eventData">Current event data.</param>
		public virtual void OnPointerUp(PointerEventData eventData)
		{
			OnUp.Invoke();
		}
	}
}