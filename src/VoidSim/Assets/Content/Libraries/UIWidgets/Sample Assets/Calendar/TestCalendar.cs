using UnityEngine;
using System.Globalization;

namespace UIWidgetsSamples
{
	/// <summary>
	/// Test Calendar.
	/// </summary>
	public class TestCalendar : MonoBehaviour
	{
		/// <summary>
		/// Calendart.
		/// </summary>
		[SerializeField]
		protected UIWidgets.Calendar Calendar;

		/// <summary>
		/// Start this instance.
		/// </summary>
		protected virtual void Start()
		{
			LocaleEn();
		}

		/// <summary>
		/// Set en-US culture.
		/// </summary>
		public void LocaleEn()
		{
			Calendar.Culture = new CultureInfo("en-US");
		}

		/// <summary>
		/// Set ja-JP culture.
		/// </summary>
		public void LocaleJp()
		{
			Calendar.Culture = new CultureInfo("ja-JP");
		}

		/// <summary>
		/// Set fr-FR culture.
		/// </summary>
		public void LocaleFr()
		{
			Calendar.Culture = new CultureInfo("fr-FR");
		}

		/// <summary>
		/// Set de-DE culture.
		/// </summary>
		public void LocaleDe()
		{
			Calendar.Culture = new CultureInfo("de-DE");
		}

		/// <summary>
		/// Set zh-CN culture.
		/// </summary>
		public void LocaleCh()
		{
			Calendar.Culture = new CultureInfo("zh-CN");
		}

		/// <summary>
		/// Set ru-RU culture.
		/// </summary>
		public void LocaleRu()
		{
			Calendar.Culture = new CultureInfo("ru-RU");
		}

		void SetCalendar(Calendar calendar)
		{
			Calendar.Culture.DateTimeFormat.Calendar = calendar;
			Calendar.UpdateCalendar();
		}

		/// <summary>
		/// Set gregorian calendar.
		/// </summary>
		public void GregorianCalendar()
		{
			SetCalendar(new GregorianCalendar());
		}

		/// <summary>
		/// Set hebrew calendar.
		/// </summary>
		public void HebrewCalendar()
		{
			SetCalendar(new HebrewCalendar());
		}

		/// <summary>
		/// Set korean calendar.
		/// </summary>
		public void KoreanCalendar()
		{
			SetCalendar(new KoreanCalendar());
		}

		/// <summary>
		/// Set japanese calendar.
		/// </summary>
		public void JapaneseCalendar()
		{
			SetCalendar(new JapaneseCalendar());
		}

		/// <summary>
		/// Set hijri calendar.
		/// </summary>
		public void HijriCalendar()
		{
			SetCalendar(new HijriCalendar());
		}

		/// <summary>
		/// Set julian calendar.
		/// </summary>
		public void JulianCalendar()
		{
			SetCalendar(new JulianCalendar());
		}

		/// <summary>
		/// Set persian calendar.
		/// </summary>
		public void PersianCalendar()
		{
			SetCalendar(new PersianCalendar());
		}

		/// <summary>
		/// Set taiwan calendar.
		/// </summary>
		public void TaiwanCalendar()
		{
			SetCalendar(new TaiwanCalendar());
		}
	}
}

