﻿using System;
using UIWidgets;
using UnityEngine;
using UnityEngine.Events;

namespace UIWidgetsSamples
{
	/// <summary>
	/// TimeRangeSlider event.
	/// </summary>
	[Serializable]
	public class TimeRangeSliderEvent : UnityEvent<DateTime,DateTime>
	{
		
	}

	/// <summary>
	/// TimeRangeSlider.
	/// </summary>
	[RequireComponent(typeof(RangeSlider))]
	public class TimeRangeSlider : MonoBehaviour
	{
		[SerializeField]
		DateTime minTime = new DateTime(2017, 1, 1, 0, 0, 1);

		/// <summary>
		/// Min time.
		/// </summary>
		public DateTime MinTime {
			get {
				return minTime;
			}
			set {
				minTime = value;

				Init();
			}
		}

		[SerializeField]
		DateTime maxTime = new DateTime(2017, 1, 1, 23, 50, 0);

		/// <summary>
		/// Max time.
		/// </summary>
		public DateTime MaxTime {
			get {
				return maxTime;
			}
			set {
				maxTime = value;

				Init();
			}
		}

		[SerializeField]
		[Tooltip("Minutes")]
		int interval = 10;

		/// <summary>
		/// Interval.
		/// </summary>
		public int Interval {
			get {
				return Mathf.Max(1, interval);
			}
			set {
				interval = Mathf.Max(1, value);

				Init();
			}
		}

		/// <summary>
		/// Start time.
		/// </summary>
		public DateTime StartTime {
			get {
				return Value2Time(CurrentRangeSlider.ValueMin);
			}
			set {
				CurrentRangeSlider.ValueMin = Time2Value(value);
			}
		}

		/// <summary>
		/// End time.
		/// </summary>
		public DateTime EndTime {
			get {
				return Value2Time(CurrentRangeSlider.ValueMax);
			}
			set {
				CurrentRangeSlider.ValueMax = Time2Value(value);
			}
		}

		RangeSlider currentRangeSlider;

		/// <summary>
		/// Current RangeSlider.
		/// </summary>
		public RangeSlider CurrentRangeSlider {
			get {
				if (currentRangeSlider==null)
				{
					currentRangeSlider = GetComponent<RangeSlider>();
				}
				return currentRangeSlider;
			}
		}

		/// <summary>
		/// Change time event.
		/// </summary>
		public TimeRangeSliderEvent OnChange = new TimeRangeSliderEvent();

		bool isStarted;

		/// <summary>
		/// Start this instance.
		/// </summary>
		public virtual void Start()
		{
			Init();
		}

		/// <summary>
		/// Remove listeners.
		/// </summary>
		protected virtual void OnDestroy()
		{
			if (currentRangeSlider!=null)
			{
				currentRangeSlider.OnValuesChange.RemoveListener(SliderChanged);
			}
		}

		/// <summary>
		/// Handle slider values changed event.
		/// </summary>
		/// <param name="min">Min value.</param>
		/// <param name="max">Max value.</param>
		protected virtual void SliderChanged(int min, int max)
		{
			OnChange.Invoke(StartTime, EndTime);
		}

		/// <summary>
		/// Init.
		/// </summary>
		protected void Init()
		{
			CurrentRangeSlider.WholeNumberOfSteps = true;
			CurrentRangeSlider.LimitMin = Time2Value(MinTime);
			CurrentRangeSlider.LimitMax = Time2Value(MaxTime);
			CurrentRangeSlider.Step = Interval;

			if (!isStarted)
			{
				CurrentRangeSlider.OnValuesChange.AddListener(SliderChanged);

				CurrentRangeSlider.ValueMin = CurrentRangeSlider.LimitMin;
				CurrentRangeSlider.ValueMax = CurrentRangeSlider.LimitMax;

				isStarted = true;
			}
		}

		DateTime Value2Time(int value)
		{
			value -= Time2Value(MinTime);
			return MinTime.AddMinutes(value);
		}

		static int Time2Value(DateTime time)
		{
			return time.Minute + time.Hour * 60;
		}
	}
}

