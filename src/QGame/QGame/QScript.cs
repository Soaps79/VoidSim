using System;
using Behaviors;
using UnityEngine;

namespace QGame
{
    /// <summary>
    /// Wraps a MonoBehavior, adding in time modification functionality, update callbacks and a maintained StopWatch
    /// </summary>
    public abstract class QScript : OrderedEventBehavior
    {
        protected readonly StopWatch StopWatch = new StopWatch();

        internal override void UpdateInternals()
        {
            if (StopWatch.IsRunning())
            {
                StopWatch.UpdateNodes(UseTimeModifier ? Time.deltaTime * TimeModifier : Time.deltaTime);
            }
        }
    }
}