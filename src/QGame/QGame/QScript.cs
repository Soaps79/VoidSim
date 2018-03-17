using System;
using Behaviors;
using UnityEngine;

namespace QGame
{
    public abstract class QScript : OrderedEventBehavior
    {
        protected readonly StopWatch StopWatch = new StopWatch();

        protected override void OnUpdateStart()
        {
            if (StopWatch.IsRunning())
            {
                StopWatch.UpdateNodes(UseTimeModifier ? Time.deltaTime * TimeModifier : Time.deltaTime);
            }
        }
    }
}