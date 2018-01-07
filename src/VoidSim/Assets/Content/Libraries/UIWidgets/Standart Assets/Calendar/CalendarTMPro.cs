using UnityEngine;
using TMPro;

namespace UIWidgets.TMProSupport
{
	/// <summary>
	/// Calendar TMPro.
	/// </summary>
	public class CalendarTMPro : CalendarBase
	{
		[SerializeField]
		TextMeshProUGUI dateText;

		/// <summary>
		/// Text to display current date.
		/// </summary>
		public TextMeshProUGUI DateText {
			get {
				return dateText;
			}
			set {
				dateText = value;

				UpdateDate();
			}
		}

		[SerializeField]
		TextMeshProUGUI monthText;

		/// <summary>
		/// Text to display current month.
		/// </summary>
		public TextMeshProUGUI MonthText {
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