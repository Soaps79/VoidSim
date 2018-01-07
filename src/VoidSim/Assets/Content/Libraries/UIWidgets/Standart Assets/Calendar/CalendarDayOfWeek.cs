using UnityEngine;
using UnityEngine.UI;
using System;

namespace UIWidgets
{
	/// <summary>
	/// CalendarDayOfWeek.
	/// Display day of week.
	/// </summary>
	public class CalendarDayOfWeek : CalendarDayOfWeekBase
	{
		/// <summary>
		/// Text component to display day of week.
		/// </summary>
		[SerializeField]
		protected Text Day;

		/// <summary>
		/// Set current date.
		/// </summary>
		/// <param name="currentDate">Current date.</param>
		public override void SetDate(DateTime currentDate)
		{
			Day.text = currentDate.ToString("ddd", Calendar.Culture);
		}
	}
}

