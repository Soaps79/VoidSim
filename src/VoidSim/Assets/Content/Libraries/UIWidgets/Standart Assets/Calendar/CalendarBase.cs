using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System;
using System.Globalization;

namespace UIWidgets
{
	/// <summary>
	/// Calendat event.
	/// </summary>
	public class CalendarDateEvent : UnityEvent<DateTime>
	{
		
	}

	/// <summary>
	/// Calendar base class.
	/// </summary>
	public abstract class CalendarBase : MonoBehaviour
	{
		[SerializeField]
		DateTime date = DateTime.Today;

		/// <summary>
		/// Selected date.
		/// </summary>
		public DateTime Date {
			get {
				return date;
			}
			set {
				Init();
				date = value;
				dateDisplay = value;

				UpdateCalendar();

				OnDateChanged.Invoke(date);
			}
		}

		[SerializeField]
		DayOfWeek firstDayOfWeek;

		/// <summary>
		/// First day of week.
		/// </summary>
		public DayOfWeek FirstDayOfWeek {
			get {
				return firstDayOfWeek;
			}
			set {
				firstDayOfWeek = value;

				UpdateCalendar();
			}
		}

		/// <summary>
		/// Container for dates.
		/// </summary>
		[SerializeField]
		public RectTransform Container;

		[SerializeField]
		CalendarDateBase calendarDateTemplate;

		/// <summary>
		/// Date template.
		/// </summary>
		public CalendarDateBase CalendarDateTemplate {
			get {
				return calendarDateTemplate;
			}
			set {
				calendarDateTemplate = value;

				Init();
				UpdateCalendar();
			}
		}

		/// <summary>
		/// Container for day of weeks.
		/// </summary>
		[SerializeField]
		public RectTransform HeaderContainer;

		[SerializeField]
		CalendarDayOfWeekBase calendarDayOfWeekTemplate;

		/// <summary>
		/// Day of week template.
		/// </summary>
		public CalendarDayOfWeekBase CalendarDayOfWeekTemplate {
			get {
				return calendarDayOfWeekTemplate;
			}
			set {
				calendarDayOfWeekTemplate = value;

				InitHeader();

				UpdateCalendar();
			}
		}

		DateTime dateDisplay = DateTime.Today;

		/// <summary>
		/// Displayed month in calendar.
		/// </summary>
		public DateTime DateDisplay {
			get {
				return dateDisplay;
			}
			set {
				dateDisplay = value;

				UpdateCalendar();
			}
		}

		CultureInfo culture = CultureInfo.InvariantCulture;

		/// <summary>
		/// Current culture.
		/// </summary>
		public CultureInfo Culture {
			get {
				return culture;
			}
			set {
				culture = value;

				UpdateCalendar();
			}
		}

		[SerializeField]
		string currentDate = DateTime.Today.ToString("yyyy-MM-dd");

		[SerializeField]
		string format = "yyyy-MM-dd";

		/// <summary>
		/// Date format.
		/// </summary>
		public string Format {
			get {
				return format;
			}
			set {
				format = value;

				UpdateDate();
			}
		}

		/// <summary>
		/// Event called when date changed.
		/// </summary>
		public CalendarDateEvent OnDateChanged = new CalendarDateEvent();

		/// <summary>
		/// Days in week.
		/// </summary>
		protected int DaysInWeek = 7;

		/// <summary>
		/// Daiplayed weeks count.
		/// </summary>
		protected int DisplayedWeeks = 6;

		bool isInited;

		EasyLayout.EasyLayout layout;

		/// <summary>
		/// Container layout.
		/// </summary>
		public EasyLayout.EasyLayout Layout {
			get {
				if (layout==null)
				{
					layout = Container.GetComponent<EasyLayout.EasyLayout>();
				}
				return layout;
			}
		}

		/// <summary>
		/// Cache for instatiated date components.
		/// </summary>
		protected Stack<CalendarDateBase> Cache = new Stack<CalendarDateBase>();

		/// <summary>
		/// Displayed days components.
		/// </summary>
		protected List<CalendarDateBase> Days = new List<CalendarDateBase>();

		/// <summary>
		/// Displayed days of week components.
		/// </summary>
		protected List<CalendarDayOfWeekBase> Header = new List<CalendarDayOfWeekBase>();

		/// <summary>
		/// Start.
		/// </summary>
		public virtual void Start()
		{
			InitFull();
		}

		/// <summary>
		/// Full initialization of this instance.
		/// </summary>
		public virtual void InitFull()
		{
			if (isInited)
			{
				return ;
			}
			isInited = true;

			date = DateTime.ParseExact(currentDate, format, Culture);

			Init();
			InitHeader();

			UpdateCalendar();
		}

		/// <summary>
		/// Init.
		/// </summary>
		protected virtual void Init()
		{
			calendarDateTemplate.gameObject.SetActive(false);

			Layout.LayoutType = EasyLayout.LayoutTypes.Grid;
			Layout.Stacking = EasyLayout.Stackings.Horizontal;
			Layout.GridConstraint = EasyLayout.GridConstraints.FixedColumnCount;
			Layout.GridConstraintCount = DaysInWeek;

			Cache.ForEach(x => Destroy(x.gameObject));
			Cache.Clear();

			Days.ForEach(x => Destroy(x.gameObject));
			Days.Clear();
		}

		/// <summary>
		/// init header.
		/// </summary>
		protected virtual void InitHeader()
		{
			calendarDayOfWeekTemplate.gameObject.SetActive(false);

			Header.ForEach(x => Destroy(x.gameObject));
			Header.Clear();
		}

		/// <summary>
		/// Displayed days count for specified month.
		/// </summary>
		/// <param name="displayedMonth">Displayed month.</param>
		/// <returns>Displayed days count.</returns>
		protected virtual int GetMaxDisplayedDays(DateTime displayedMonth)
		{
			return DaysInWeek * DisplayedWeeks;
		}

		/// <summary>
		/// Updates the calendar.
		/// Should be called only after changing culture settings.
		/// </summary>
		public virtual void UpdateCalendar()
		{
			InitFull();

			UpdateHeader();
			UpdateDays();
			UpdateDate();
		}

		/// <summary>
		/// Update displayed date and month.
		/// </summary>
		protected virtual void UpdateDate()
		{
			
		}

		/// <summary>
		/// Update header.
		/// </summary>
		protected virtual void UpdateHeader()
		{
			if (CalendarDayOfWeekTemplate==null)
			{
				return ;
			}

			GenerateHeader();

			var firstDay = GetFirstBlockDay(dateDisplay);
			for (int i = 0; i < DaysInWeek; i++)
			{
				Header[i].SetDate(firstDay.AddDays(i));
			}
		}

		/// <summary>
		/// Update days.
		/// </summary>
		protected virtual void UpdateDays()
		{
			GenerateDays(dateDisplay);

			var n = GetMaxDisplayedDays(dateDisplay);
			var firstDay = GetFirstBlockDay(dateDisplay);
			for (int i = 0; i < n; i++)
			{
				Days[i].SetDate(firstDay.AddDays(i));
			}
		}

		/// <summary>
		/// Get first day for displayed month.
		/// </summary>
		/// <param name="day">Displayed month.</param>
		/// <returns>First day for displayed month.</returns>
		protected virtual DateTime GetFirstBlockDay(DateTime day)
		{
			var first = day.AddDays(-culture.DateTimeFormat.Calendar.GetDayOfMonth(day) + 1);

			while (culture.DateTimeFormat.Calendar.GetDayOfWeek(first)!=FirstDayOfWeek)
			{
				first = first.AddDays(-1);
			}

			return first;
		}

		/// <summary>
		/// Instantiated date component from template.
		/// </summary>
		/// <returns>Date component.</returns>
		protected virtual CalendarDateBase GenerateDay()
		{
			CalendarDateBase day;

			if (Cache.Count > 0)
			{
				day = Cache.Pop();
			}
			else
			{
				day = Instantiate(CalendarDateTemplate) as CalendarDateBase;
				day.transform.SetParent(Container, false);
				Utilites.FixInstantiated(CalendarDateTemplate, day);

				day.Calendar = this;
			}

			day.gameObject.SetActive(true);
			day.transform.SetAsLastSibling();

			return day;
		}

		/// <summary>
		/// Create header from day of week template.
		/// </summary>
		protected virtual void GenerateHeader()
		{
			if (CalendarDayOfWeekTemplate==null)
			{
				return ;
			}

			for (int i = Header.Count; i < DaysInWeek; i++)
			{
				var day_of_week = Instantiate(CalendarDayOfWeekTemplate) as CalendarDayOfWeekBase;
				day_of_week.transform.SetParent(HeaderContainer, false);
				Utilites.FixInstantiated(CalendarDayOfWeekTemplate, day_of_week);

				day_of_week.Calendar = this;

				day_of_week.gameObject.SetActive(true);

				Header.Add(day_of_week);
			}
		}
		
		/// <summary>
		/// Create days for displayed month.
		/// </summary>
		/// <param name="displayedDate">Displayed date.</param>
		protected virtual void GenerateDays(DateTime displayedDate)
		{
			var n = GetMaxDisplayedDays(displayedDate);

			if (n > Days.Count)
			{
				for (int i = Days.Count; i < n; i++)
				{
					Days.Add(GenerateDay());
				}
			}
			else if (n < Days.Count)
			{
				Days.GetRange(n, Days.Count - n).ForEach(Cache.Push);
				Cache.ForEach(x => x.gameObject.SetActive(false));
				Days.RemoveRange(n, Days.Count - n);
			}
		}

		/// <summary>
		/// Display next month.
		/// </summary>
		public virtual void NextMonth()
		{
			DateDisplay = DateDisplay.AddMonths(1);
		}

		/// <summary>
		/// Display previous month.
		/// </summary>
		public virtual void PrevMonth()
		{
			DateDisplay = DateDisplay.AddMonths(-1);
		}

		/// <summary>
		/// Display next year.
		/// </summary>
		public virtual void NextYear()
		{
			DateDisplay = DateDisplay.AddYears(1);
		}

		/// <summary>
		/// Display previous year.
		/// </summary>
		public virtual void PrevYear()
		{
			DateDisplay = DateDisplay.AddYears(-1);
		}

		/// <summary>
		/// Is specified date is weekend?
		/// </summary>
		/// <param name="displayedDate">Displayed date.</param>
		/// <returns>true if specified date is weekend; otherwise, false.</returns>
		public virtual bool IsWeekend(DateTime displayedDate)
		{
			var day_of_week = culture.DateTimeFormat.Calendar.GetDayOfWeek(displayedDate);
			return day_of_week==DayOfWeek.Saturday || day_of_week==DayOfWeek.Sunday;
		}

		/// <summary>
		/// Is specified date is holiday?
		/// </summary>
		/// <param name="displayedDate">Displayed date.</param>
		/// <returns>true if specified date is holiday; otherwise, false.</returns>
		public virtual bool IsHoliday(DateTime displayedDate)
		{
			return false;
		}

		/// <summary>
		/// Is dates belongs to same month?
		/// </summary>
		/// <param name="date1">First date.</param>
		/// <param name="date2">Second date.</param>
		/// <returns>true if dates belongs to same month; otherwise, false.</returns>
		public virtual bool IsSameMonth(DateTime date1, DateTime date2)
		{
			return culture.DateTimeFormat.Calendar.GetYear(date1)==culture.DateTimeFormat.Calendar.GetYear(date2)
				&& culture.DateTimeFormat.Calendar.GetMonth(date1)==culture.DateTimeFormat.Calendar.GetMonth(date2);
		}

		/// <summary>
		/// Is dates belongs to same day?
		/// </summary>
		/// <param name="date1">First date.</param>
		/// <param name="date2">Second date.</param>
		/// <returns>true if dates is same day; otherwise, false.</returns>
		public virtual bool IsSameDay(DateTime date1, DateTime date2)
		{
			return date1.Date==date2.Date;
		}

		/// <summary>
		/// Set calendar type.
		/// </summary>
		/// <param name="calendar">Calendar type.</param>
		public void SetCalendar(System.Globalization.Calendar calendar)
		{
			culture.DateTimeFormat.Calendar = calendar;
			UpdateCalendar();
		}
	}
}