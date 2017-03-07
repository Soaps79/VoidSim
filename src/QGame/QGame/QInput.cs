using System.Collections.Generic;

using UnityEngine;

namespace QGame
{
	public class InputButtonAxis
	{
		public string AxisName { get; internal set; }
		public float CurrentValue { get; internal set; }
		public float LastValue { get; internal set; }

		/// <summary>
		/// Returns true if A was pressed this frame
		/// </summary>
		public bool AIsPressed
		{
			get { return (CurrentValue < 0 && LastValue >= 0); }
		}

		/// <summary>
		/// Returns true if A was released this frame
		/// </summary>
		public bool AIsReleased
		{
			get { return (CurrentValue >= 0 && LastValue < 0); }
		}

		/// <summary>
		/// Returns true if A is down this frame
		/// </summary>
		public bool AIsDown
		{
			get { return CurrentValue < 0; }
		}

		/// <summary>
		/// Returns true if B was pressed this frame
		/// </summary>
		public bool BIsPressed
		{
			get { return (CurrentValue > 0 && LastValue <= 0); }
		}

		/// <summary>
		/// Returns true if B was released this frame
		/// </summary>
		public bool BIsReleased
		{
			get { return (CurrentValue >= 0 && LastValue < 0); }
		}

		/// <summary>
		/// Returns true if B is down this frame
		/// </summary>
		public bool BIsDown
		{
			get { return CurrentValue > 0; }
		}
	}

	public class QInput : QScript
	{
		

	}
}