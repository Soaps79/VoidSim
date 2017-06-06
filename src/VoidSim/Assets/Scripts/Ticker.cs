namespace Assets.Scripts
{
	/// <summary>
	/// Useful for any value that will be manually incremented
	/// </summary>
	public class Ticker
	{
		public float TotalTicks;
		public float ElapsedTicks;

		public float TimeRemainingAsZeroToOne
		{
			get { return TotalTicks > 0 ? ElapsedTicks / TotalTicks : 1; }
		}

		public bool IsComplete { get { return ElapsedTicks >= TotalTicks; } }

		public void Reset(float newTotal = 0)
		{
			ElapsedTicks = 0;
			if (newTotal != 0)
				TotalTicks = newTotal;
		}
	}
}