using System;
using Behaviors;

namespace QGame
{
    public delegate void VoidVoidCallback();
    public delegate void VoidFloatCallback(float value);
    public delegate void VoidQScriptCallback(OrderedEventBehavior script);

    public delegate object ObjectCallback();

    public delegate void VoidILivingCallback(ILiving iLiving);

    public abstract class QScript : OrderedEventBehavior, ILiving
    {
        private bool _isAlive = true;
        public Action<ILiving> AliveChanged { get; set; }

        private BehaviorHolder _holder;
        public BehaviorHolder Holder { get { return _holder; } }

        public QScript()
        {
            _holder = new BehaviorHolder(this);
            EnableStopWatch();
        }

        protected StopWatch StopWatch;
        public void EnableStopWatch()
        {
            if (StopWatch == null)
            {
                StopWatch = new StopWatch();
            }
        }

        public bool IsAlive
        {
            get { return _isAlive; }
            set { _isAlive = TrySet(_isAlive, value, AliveChanged); }
        }

        public override void ClearAllDelegates()
        {
            base.ClearAllDelegates();

            AliveChanged = null;
        }

        /// <summary>
        /// Only fire event if the value is changed.
        /// </summary>
        /// <param name="currentValue">Current value</param>
        /// <param name="newValue">Value to set</param>
        /// <param name="onChangedCallback">Callback to fire if value is changed</param>
        /// <returns>The value to set.</returns>
        protected bool TrySet(bool currentValue, bool newValue, Action<ILiving> onChangedCallback)
        {
            if (currentValue != newValue)
            {
                currentValue = newValue;
                if (onChangedCallback != null)
                {
                    onChangedCallback(this);
                }
            }

            return currentValue;
        }
        
        protected override void OnUpdateStart(float delta)
        {
            if (StopWatch != null)
            {
                StopWatch.UpdateNodes(delta);
            }
            base.OnUpdateStart(delta);
        }

        protected override void OnUpdate(float delta)
        {
            _holder.Update(delta);

            base.OnUpdate(delta);
        }

        public virtual void Initialize() { }
    }
}