using UnityEngine;
using UnityEngine.EventSystems;
using System;

namespace UIWidgets
{
	/// <summary>
	/// Base class for Calendar date.
	/// </summary>
	public abstract class CalendarDateBase : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
	{
		/// <summary>
		/// Current date to display.
		/// </summary>
		protected DateTime CurrentDate;

		/// <summary>
		/// Date belongs to this calendar.
		/// </summary>
		[HideInInspector]
		public CalendarBase Calendar;

		/// <summary>
		/// Set current date.
		/// </summary>
		/// <param name="currentDate">Current date.</param>
		public virtual void SetDate(DateTime currentDate)
		{
			CurrentDate = currentDate;

			DateChanged();
		}

		/// <summary>
		/// Update displayed date.
		/// </summary>
		public abstract void DateChanged();

		#region IPointerDownHandler implementation
		/// <summary>
		/// OnPoiterDown event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnPointerDown(PointerEventData eventData)
		{
		}
		#endregion

		#region IPointerUpHandler implementation
		/// <summary>
		/// OnPointerUp event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnPointerUp(PointerEventData eventData)
		{
		}
		#endregion

		#region IPointerClickHandler implementation
		/// <summary>
		/// PointerClick event.
		/// Change calendar date to clicked date.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnPointerClick(PointerEventData eventData)
		{
			Calendar.Date = CurrentDate;
		}
		#endregion
	}
}

