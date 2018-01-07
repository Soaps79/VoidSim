using UnityEngine;
using TMPro;
using System;

namespace UIWidgets.TMProSupport
{
	/// <summary>
	/// CalendarDayOfWeek TMPro.
	/// Display day of week.
	/// </summary>
	public class CalendarDayOfWeekTMPro : CalendarDayOfWeekBase
	{
		[SerializeField]
		TextMeshProUGUI Day;

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

