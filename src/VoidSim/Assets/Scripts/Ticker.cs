﻿namespace Assets.Scripts
{
	public class TickerData
	{
		public float TotalTicks;
		public float ElapsedTicks;
	}

	/// <summary>
	/// Useful for any value that will be manually incremented
	/// </summary>
	public class Ticker
	{
		public float TotalTicks;
		public float ElapsedTicks;

		public Ticker() { }

		public Ticker(TickerData data)
		{
			TotalTicks = data.TotalTicks;
			ElapsedTicks = data.ElapsedTicks;
		}

		public float TimeRemainingAsZeroToOne
		{
			get { return TotalTicks > 0 ? ElapsedTicks / TotalTicks : 1; }
		}

		public bool IsComplete { get { return ElapsedTicks >= TotalTicks; } }

		public void Reset(float newTotal)
		{
			ElapsedTicks = 0;
            TotalTicks = newTotal;
		}

	    public void Reset()
	    {
			ElapsedTicks = 0;
        }

        public TickerData GetData()
		{
			return new TickerData
			{
				TotalTicks = TotalTicks,
				ElapsedTicks = ElapsedTicks
			};
		}
	}
}