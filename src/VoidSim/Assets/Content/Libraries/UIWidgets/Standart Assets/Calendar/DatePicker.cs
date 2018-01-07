using UnityEngine;
using System;

namespace UIWidgets
{
	/// <summary>
	/// DatePicker.
	/// </summary>
	[AddComponentMenu("UI/UIWidgets/DatePicker")]
	public class DatePicker : Picker<DateTime,DatePicker>
	{
		/// <summary>
		/// ListView.
		/// </summary>
		[SerializeField]
		public CalendarBase Calendar;

		/// <summary>
		/// Prepare picker to open.
		/// </summary>
		/// <param name="defaultValue">Default value.</param>
		public override void BeforeOpen(DateTime defaultValue)
		{
			Calendar.Date = defaultValue;

			Calendar.OnDateChanged.AddListener(DateSelected);
		}

		void DateSelected(DateTime date)
		{
			Selected(date);
		}

		/// <summary>
		/// Prepare picker to close.
		/// </summary>
		public override void BeforeClose()
		{
			Calendar.OnDateChanged.RemoveListener(DateSelected);
		}
	}
}