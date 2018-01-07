﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UIWidgets
{
	/// <summary>
	/// Notify sequence.
	/// </summary>
	public enum NotifySequence
	{
		/// <summary>
		/// Display notification right now, without adding to sequence.
		/// </summary>
		None = 0,

		/// <summary>
		/// Add notification to start of sequence.
		/// </summary>
		First = 1,

		/// <summary>
		/// Add notification to end of sequence.
		/// </summary>
		Last = 2,
	}

	/// <summary>
	/// Notify sequence manager.
	/// </summary>
	public class NotifySequenceManager : MonoBehaviour
	{
		static Notify currentNotify;
		static List<Notify> notifySequence = new List<Notify>();

		/// <summary>
		/// Clear notifications sequence.
		/// </summary>
		public void Clear()
		{
			if (currentNotify!=null)
			{
				currentNotify.Return();
				currentNotify = null;
			}
			notifySequence.ForEach(ReturnNotify);
			notifySequence.Clear();
		}

		void ReturnNotify(Notify notify)
		{
			notify.Return();
		}

		/// <summary>
		/// Add the specified notification to sequence.
		/// </summary>
		/// <param name="notification">Notification.</param>
		/// <param name="type">Type.</param>
		public virtual void Add(Notify notification, NotifySequence type)
		{
			if (type==NotifySequence.Last)
			{
				notifySequence.Add(notification);
			}
			else
			{
				notifySequence.Insert(0, notification);
			}
		}

		/// <summary>
		/// Display next notification in sequence if possible.
		/// </summary>
		protected virtual void Update()
		{
			if (currentNotify!=null)
			{
				return ;
			}
			if (notifySequence.Count==0)
			{
				return ;
			}
			currentNotify = notifySequence[0];
			notifySequence.RemoveAt(0);
			currentNotify.Display(NotifyDelay);
		}

		IEnumerator nextDelay;

		void NotifyDelay()
		{
			if (nextDelay!=null)
			{
				StopCoroutine(nextDelay);
			}

			if ((notifySequence.Count > 0) && (notifySequence[0].SequenceDelay > 0))
			{
				nextDelay = NextDelay();
				StartCoroutine(nextDelay);
			}
			else
			{
				currentNotify = null;
			}
		}

		IEnumerator NextDelay()
		{
			yield return new WaitForSeconds(notifySequence[0].SequenceDelay);
			currentNotify = null;
		}
	}
}