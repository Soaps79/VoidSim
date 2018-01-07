using UnityEngine;
using UnityEngine.UI;

namespace UIWidgets
{
	/// <summary>
	/// Calendar.
	/// </summary>
	public class Calendar : CalendarBase
	{
		[SerializeField]
		Text dateText;

		/// <summary>
		/// Text to display current date.
		/// </summary>
		public Text DateText {
			get {
				return dateText;
			}
			set {
				dateText = value;

				UpdateDate();
			}
		}

		[SerializeField]
		Text monthText;

		/// <summary>
		/// Text to display current month.
		/// </summary>
		public Text MonthText {
			get {
				return monthText;
			}
			set {
				monthText = value;

				UpdateCalendar();
			}
		}

		/// <summary>
		/// Update displayed date and month.
		/// </summary>
		protected override void UpdateDate()
		{
			if (dateText!=null)
			{
				dateText.text = Date.ToString("D", Culture);
			}

			if (monthText!=null)
			{
				monthText.text = DateDisplay.ToString("Y", Culture);
			}
		}
	}
}