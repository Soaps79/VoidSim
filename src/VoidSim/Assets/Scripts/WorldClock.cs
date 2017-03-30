using System.Collections.Generic;
using UnityEngine;
using System;

using QGame;
using Messaging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

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

public class WorldClock : SingletonBehavior<WorldClock>, IMessageListener
{
    public float RealSecondsToGameHour;
    public int MinutesPerHour;
    public int HoursPerDay;
    public int DaysPerWeek;
    public int WeeksPerMonth;
    public int MonthsPerYear;

    private const string PAUSE_NAME = "Pause";

    // TODO: Serialize game speeds to json
    [SerializeField]
    private GameSpeed[] _initialGameSpeeds;
    public string InitialGameSpeedName;
    private Dictionary<string, float> _gameSpeeds = new Dictionary<string, float>();
    
    public static float CurrentTimeScale;
    private string _currentSpeedName;
    private string _prePauseSpeedName;

    public static EventHandler OnHourUp;
    public static EventHandler OnDayUp;
    public static EventHandler OnWeekUp;
    public static EventHandler OnMonthUp;
    public static EventHandler OnYearUp;

    public WorldTime CurrentTime;
    private float _elapsedMS;

    void Awake()
    {
        // editor should have value, if not, this
        if (CurrentTime == null)
            CurrentTime = new WorldTime();

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
        KeyValueDisplay.Instance.Add("GameTime", () => CurrentTime.TimeAsString);
        KeyValueDisplay.Instance.Add("GameSpeed", () => CurrentTimeScale);
        MessageHub.Instance.AddListener(this, GameMessages.GameSpeedChange);
    }

    private void CheckForKeypress(float value)
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
        MessageHub.Instance.QueueMessage(GameMessages.GameSpeedChange, args);
    }

    private void UpdateClock(float delta)
    {
        _elapsedMS += delta;
        var realSecondsToGameMinute = (60 / RealSecondsToGameHour) * CurrentTimeScale;
        AddMinutes(realSecondsToGameMinute * delta);
    }

    #region Time Increments
    private void AddMinutes(float minutes)
    {
        CurrentTime.Minute += minutes;
        while (CurrentTime.Minute >= MinutesPerHour)
        {
            AddHours(1);
            CurrentTime.Minute -= MinutesPerHour;
        }
    }

    private void AddHours(int hours)
    {
        CurrentTime.Hour += hours;
        if (OnHourUp != null)
            OnHourUp(this, null);

        while (CurrentTime.Hour >= HoursPerDay)
        {
            AddDays(1);
            CurrentTime.Hour -= HoursPerDay;
        }
    }

    private void AddDays(int days)
    {
        CurrentTime.Day += days;
        if (OnDayUp != null)
            OnDayUp(this, null);
        while (CurrentTime.Day >= DaysPerWeek)
        {
            AddWeeks(1);
            CurrentTime.Day -= DaysPerWeek;
        }
    }

    private void AddWeeks(int weeks)
    {
        CurrentTime.Week += weeks;
        if (OnWeekUp != null)
            OnWeekUp(this, null);
        while (CurrentTime.Week >= WeeksPerMonth)
        {
            AddMonths(1);
            CurrentTime.Week -= WeeksPerMonth;
        }
    }

    private void AddMonths(int months)
    {
        CurrentTime.Month += months;
        if (OnMonthUp != null)
            OnMonthUp(this, null);
        while (CurrentTime.Month >= MonthsPerYear)
        {
            AddYears(1);
            CurrentTime.Month -= MonthsPerYear;
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
        get {  return RealSecondsToGameHour; }
    }

    public float SecondsPerDay
    {
        get { return RealSecondsToGameHour * HoursPerDay; }
    }

    public float SecondsPerWeek
    {
        get { return RealSecondsToGameHour * HoursPerDay * DaysPerWeek; }
    }

    public float SecondsPerMonth
    {
        get { return RealSecondsToGameHour * HoursPerDay * DaysPerWeek * WeeksPerMonth; }
    }

    public float SecondsPerYear
    {
        get { return RealSecondsToGameHour * HoursPerDay * DaysPerWeek * WeeksPerMonth * MonthsPerYear; }
    }

    public float GetSeconds(TimeLength timeLength)
    {
        switch (timeLength.TimeUnit)
        {
            case TimeUnit.Hour:
                return RealSecondsToGameHour * timeLength.Length;
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
}
