using UnityEngine;
using System;

namespace UIWidgets
{
	/// <summary>
	/// Base class for Calendar day of week.
	/// </summary>
	public abstract class CalendarDayOfWeekBase : MonoBehaviour
	{
		/// <summary>
		/// Date belongs to this calendar.
		/// </summary>
		[HideInInspector]
		public CalendarBase Calendar;

		/// <summary>
		/// Set current date.
		/// </summary>
		/// <param name="currentDate">Current date.</param>
		public abstract void SetDate(DateTime currentDate);
	}
}