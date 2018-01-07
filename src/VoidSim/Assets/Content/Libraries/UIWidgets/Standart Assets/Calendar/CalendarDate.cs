using UnityEngine;
using UnityEngine.UI;

namespace UIWidgets
{
	/// <summary>
	/// CalendarDate.
	/// Display date.
	/// </summary>
	public class CalendarDate : CalendarDateBase
	{
		/// <summary>
		/// Text component to display day.
		/// </summary>
		[SerializeField]
		protected Text Day;

		/// <summary>
		/// Image component to display day background.
		/// </summary>
		[SerializeField]
		protected Image DayImage;

		/// <summary>
		/// Selected date background.
		/// </summary>
		[SerializeField]
		public Sprite SelectedDayBackground;

		/// <summary>
		/// Selected date color.
		/// </summary>
		[SerializeField]
		public Color SelectedDay = Color.white;

		/// <summary>
		/// Default date background.
		/// </summary>
		[SerializeField]
		public Sprite DefaultDayBackground;

		/// <summary>
		/// Color for date in current month.
		/// </summary>
		[SerializeField]
		public Color CurrentMonth = Color.white;

		/// <summary>
		/// Weekend date color.
		/// </summary>
		[SerializeField]
		public Color Weekend = Color.red;

		/// <summary>
		/// Color for date not in current month.
		/// </summary>
		[SerializeField]
		public Color OtherMonth = Color.gray;

		/// <summary>
		/// Update displayed date.
		/// </summary>
		public override void DateChanged()
		{
			Day.text = CurrentDate.ToString("dd", Calendar.Culture);

			if (Calendar.IsSameDay(Calendar.Date, CurrentDate))
			{
				Day.color = SelectedDay;
				DayImage.sprite = SelectedDayBackground;
			}
			else
			{
				DayImage.sprite = DefaultDayBackground;

				if (Calendar.IsSameMonth(Calendar.DateDisplay, CurrentDate))
				{
					if ((Calendar.IsWeekend(CurrentDate)) ||
						(Calendar.IsHoliday(CurrentDate)))
					{
						Day.color = Weekend;
					}
					else
					{
						Day.color = CurrentMonth;
					}
				}
				else
				{
					if ((Calendar.IsWeekend(CurrentDate)) ||
						(Calendar.IsHoliday(CurrentDate)))
					{
						Day.color = Weekend * OtherMonth;
					}
					else
					{
						Day.color = OtherMonth;
					}
				}
			}
		}
	}
}

