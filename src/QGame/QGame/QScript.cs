using System;
using Behaviors;
using UnityEngine;

namespace QGame
{
    public delegate void VoidVoidCallback();
    public delegate void VoidFloatCallback(float value);
    public delegate void VoidQScriptCallback(OrderedEventBehavior script);

    public delegate object ObjectCallback();

    public delegate void VoidILivingCallback(ILiving iLiving);

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