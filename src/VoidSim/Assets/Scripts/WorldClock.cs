using System;
using System.Collections.Generic;
using Messaging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using QGame;
using UnityEngine;

namespace Assets.Scripts
{
	[Serializable]
	public class GameSpeed
	{
		public string Name;
		public float TimeScale;
	}

	public class GameSpeedMessageArgs : MessageArgs
	{
		public float PreviousSpeedTimeScale;
		public string PreviousSpeedName;
		public float NewSpeedTimeScale;
		public string NewSpeedName;
	}

	public enum TimeUnit
	{
		Hour, Minute, Day, Week, Month, Year
	}

	[Serializable]
	public class TimeLength
	{
		[JsonConverter(typeof(StringEnumConverter))]
		public TimeUnit TimeUnit;
		public int Length;
	}

	[Serializable]
	public class WorldTime
	{
		public int Hour;
		public float Minute;
		public int Day;
		public int Week;
		public int Month;
		public int Year;
		public string TimeAsString
		{
			get
			{
				return string.Format("y{0} : m{1} : w{2} : d{3} : h{4}",
					Year, Month, Week, Day, Hour);
			}
		}
	}

	public interface IWorldClock
	{
		float GetSeconds(TimeLength timeLength);
		void RegisterCallback(TimeUnit unit, EventHandler callback);
		WorldTime CurrentTime { get; }

		EventHandler OnHourUp { get; set; }
		EventHandler OnDayUp { get; set; }
		EventHandler OnWeekUp { get; set; }
		EventHandler OnMonthUp { get; set; }
		EventHandler OnYearUp { get; set; }

		float RealSecondsToGameHour { get; }
		int MinutesPerHour { get; }
		int HoursPerDay { get; }
		int DaysPerWeek { get; }
		int WeeksPerMonth { get; }
		int MonthsPerYear { get; }
	}

	public class WorldClock : SingletonBehavior<WorldClock>, IMessageListener, IWorldClock
	{
		[SerializeField] private float _realSecondsToGameHour;
		[SerializeField] private int _minutesPerHour;
		[SerializeField] private int _hoursPerDay;
		[SerializeField] private int _daysPerWeek;
		[SerializeField] private int _weeksPerMonth;
		[SerializeField] private int _monthsPerYear;

		public float RealSecondsToGameHour { get { return _realSecondsToGameHour; } }
		public int MinutesPerHour { get { return _minutesPerHour; } }
		public int HoursPerDay { get { return _hoursPerDay; } }
		public int DaysPerWeek { get { return _daysPerWeek; } }
		public int WeeksPerMonth { get { return _weeksPerMonth; } }
		public int MonthsPerYear { get { return _monthsPerYear; } }

		private const string PAUSE_NAME = "Pause";

		// TODO: Serialize game speeds to json
		[SerializeField]
		private GameSpeed[] _initialGameSpeeds;
		public string InitialGameSpeedName;
		private Dictionary<string, float> _gameSpeeds = new Dictionary<string, float>();
    
		public static float CurrentTimeScale;
		private string _currentSpeedName;
		private string _prePauseSpeedName;

		public EventHandler OnHourUp { get; set; }
		public EventHandler OnDayUp { get; set; }
		public EventHandler OnWeekUp { get; set; }
		public EventHandler OnMonthUp { get; set; }
		public EventHandler OnYearUp { get; set; }

		[SerializeField] private WorldTime _initialTime;

		public WorldTime CurrentTime { get; private set; }
		private float _elapsedMS;

		void Awake()
		{
			// editor should have value, if not, this
			CurrentTime = _initialTime ?? new WorldTime();

			// pull speeds from editor
			foreach (var initialGameSpeed in _initialGameSpeeds)
			{
				_gameSpeeds.Add(initialGameSpeed.Name, initialGameSpeed.TimeScale);
			}

			if (!_gameSpeeds.ContainsKey(InitialGameSpeedName))
			{
				throw new UnityException("Initial speed name has no matching value");
			}
			_gameSpeeds.Add(PAUSE_NAME, 0);
			UseTimeModifier = false;
		}

		void Start()
		{
			// messages and KVD
			RegisterWithServices();

			// start looking for keypresses
			OnEveryUpdate += CheckForKeypress;
			OnEveryUpdate += UpdateClock;


			// tell the world
			PostSpeedChange(InitialGameSpeedName, _gameSpeeds[InitialGameSpeedName]);
		}

		private void RegisterWithServices()
		{
			KeyValueDisplay.Instance.Add("GameSpeed", () => CurrentTimeScale);
			Locator.MessageHub.AddListener(this, GameMessages.GameSpeedChange);
		}

		private void CheckForKeypress()
		{
			// passing pause here will toggle between pause and the last speed
			if (Input.GetKeyDown(KeyCode.Space))
			{
				ChangeGameSpeed(PAUSE_NAME);
				return;
			}
            
			// number keys index into speeds in order defined by editor
			var keyCode = KeyCode.Alpha1;
			foreach (var speed in _initialGameSpeeds)
			{
				if(Input.GetKeyDown(keyCode) )
					ChangeGameSpeed(speed.Name);

				keyCode++;
			}
		}

		public void ChangeGameSpeed(string speed)
		{
			if (CheckForUnpause(speed))
				return;

			if (speed != _currentSpeedName && _gameSpeeds.ContainsKey(speed))
			{
				PostSpeedChange(speed, _gameSpeeds[speed]);
			}
		}

		private bool CheckForUnpause(string speed)
		{
			if (speed == PAUSE_NAME && CurrentTimeScale == 0)
			{
				ChangeGameSpeed(_prePauseSpeedName);
				return true;
			}
			return false;
		}

		void PostSpeedChange(string speed, float value)
		{
			var args = new GameSpeedMessageArgs()
			{
				PreviousSpeedTimeScale = CurrentTimeScale,
				PreviousSpeedName = _currentSpeedName,
				NewSpeedTimeScale = value,
				NewSpeedName = speed
			};
			Locator.MessageHub.QueueMessage(GameMessages.GameSpeedChange, args);
		}

		private void UpdateClock()
		{
		    var delta = Time.deltaTime;
			_elapsedMS += delta;
			var realSecondsToGameMinute = (60 / _realSecondsToGameHour) * CurrentTimeScale;
			AddMinutes(realSecondsToGameMinute * delta);
		}

		#region Time Increments
		private void AddMinutes(float minutes)
		{
			CurrentTime.Minute += minutes;
			while (CurrentTime.Minute >= _minutesPerHour)
			{
				AddHours(1);
				CurrentTime.Minute -= _minutesPerHour;
			}
		}

		private void AddHours(int hours)
		{
			CurrentTime.Hour += hours;
			if (OnHourUp != null)
				OnHourUp(this, null);

			while (CurrentTime.Hour >= _hoursPerDay)
			{
				AddDays(1);
				CurrentTime.Hour -= _hoursPerDay;
			}
		}

		private void AddDays(int days)
		{
			CurrentTime.Day += days;
			if (OnDayUp != null)
				OnDayUp(this, null);
			while (CurrentTime.Day >= _daysPerWeek)
			{
				AddWeeks(1);
				CurrentTime.Day -= _daysPerWeek;
			}
		}

		private void AddWeeks(int weeks)
		{
			CurrentTime.Week += weeks;
			if (OnWeekUp != null)
				OnWeekUp(this, null);
			while (CurrentTime.Week >= _weeksPerMonth)
			{
				AddMonths(1);
				CurrentTime.Week -= _weeksPerMonth;
			}
		}

		private void AddMonths(int months)
		{
			CurrentTime.Month += months;
			if (OnMonthUp != null)
				OnMonthUp(this, null);
			while (CurrentTime.Month >= _monthsPerYear)
			{
				AddYears(1);
				CurrentTime.Month -= _monthsPerYear;
			}
		}

		private void AddYears(int years)
		{
			if (OnYearUp != null)
				OnYearUp(this, null);
			CurrentTime.Year += years;
		}
		#endregion

		#region IMessageListener implementation

		public void HandleMessage(string type, MessageArgs args)
		{
			if (type == GameMessages.GameSpeedChange && args is GameSpeedMessageArgs)
			{
				HandleSpeedChange(args as GameSpeedMessageArgs);
			}
		}

		void HandleSpeedChange(GameSpeedMessageArgs args)
		{
			if (args.NewSpeedName == PAUSE_NAME)
			{
				_prePauseSpeedName = _currentSpeedName;
			}

			CurrentTimeScale = args.NewSpeedTimeScale;
			_currentSpeedName = args.NewSpeedName;
		}

		public string Name { get { return "WorldClock"; } }

		#endregion


		// would be nice to make these appear statically
		public float SecondsPerHour
		{
			get {  return _realSecondsToGameHour; }
		}

		public float SecondsPerDay
		{
			get { return _realSecondsToGameHour * _hoursPerDay; }
		}

		public float SecondsPerWeek
		{
			get { return _realSecondsToGameHour * _hoursPerDay * _daysPerWeek; }
		}

		public float SecondsPerMonth
		{
			get { return _realSecondsToGameHour * _hoursPerDay * _daysPerWeek * _weeksPerMonth; }
		}

		public float SecondsPerYear
		{
			get { return _realSecondsToGameHour * _hoursPerDay * _daysPerWeek * _weeksPerMonth * _monthsPerYear; }
		}

		public float GetSeconds(TimeLength timeLength)
		{
			switch (timeLength.TimeUnit)
			{
				case TimeUnit.Hour:
					return _realSecondsToGameHour * timeLength.Length;
				case TimeUnit.Day:
					return SecondsPerDay* timeLength.Length;
				case TimeUnit.Week:
					return SecondsPerWeek* timeLength.Length;
				case TimeUnit.Month:
					return SecondsPerMonth* timeLength.Length;
				case TimeUnit.Year:
					return SecondsPerYear* timeLength.Length;
				default:
					throw new ArgumentOutOfRangeException("timeLength", timeLength.TimeUnit, "Unsupported Unit in GetSeconds");
			}
		}

		public void RegisterCallback(TimeUnit unit, EventHandler callback)
		{
			switch (unit)
			{
				case TimeUnit.Hour:
					OnHourUp += callback;
					break;
				case TimeUnit.Day:
					OnDayUp += callback;
					break;
				case TimeUnit.Week:
					OnWeekUp += callback;
					break;
				case TimeUnit.Month:
					OnMonthUp += callback;
					break;
				case TimeUnit.Year:
					OnYearUp += callback;
					break;
				default:
					throw new ArgumentOutOfRangeException("unit", unit, null);
			}
		}
	}
}