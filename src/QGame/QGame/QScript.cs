using UnityEngine;

using Behaviors;
using Messaging;

using System.Collections.Generic;


namespace QGame
{
	public delegate void VoidVoidCallback();
	public delegate void VoidFloatCallback(float value);
	public delegate void VoidQScriptCallback(QScript script);

	public delegate object ObjectCallback();

	public delegate void VoidILivingCallback(ILiving iLiving);

	public interface ILiving
	{
		event VoidILivingCallback AliveChanged;
		bool IsAlive { get; set; }
	}

	public abstract class QScript : MonoBehaviour, ILiving
	{
		private bool _isIsEnabled = true;
		public bool IsEnabled
		{
			get
			{
				return _isIsEnabled;
			}
			set
			{
				if (_isIsEnabled != value)
				{
					_isIsEnabled = value;
					if (EnableChanged != null)
					{
						EnableChanged(this);
					}
				}
			}
		}
		public VoidQScriptCallback EnableChanged;

		private bool _isAlive = true;
		public bool IsAlive
		{
			get { return _isAlive; }
			set
			{
				if (_isAlive != value)
				{
					_isAlive = value;
					if (AliveChanged != null)
					{
						AliveChanged(this);
					}
					if (!_isAlive && OnDeath != null)
					{
						OnDeath(this);
					}
				}
			}
		}
		public event VoidILivingCallback AliveChanged;
		public event VoidILivingCallback OnDeath;

		//public readonly string KillAllTrigger;
		//public void Die()
		//{
		//	IsAlive = false;
		//	OnUpdate(1);
		//}

		private BehaviorHolder _holder;
		public BehaviorHolder Holder
		{
			get
			{
				return _holder;
			}
		}

		public QScript()
		{
			_holder = new BehaviorHolder(this);
			//KillAllTrigger = "Die";
		}

		//protected StopWatch stopWatch;
		//public void EnableStopWatch()
		//{
		//	if (stopWatch == null)
		//	{
		//		stopWatch = new StopWatch();
		//	}
		//}

		public void ClearAllDelegates()
		{
			this.OnNextUpdate = null;
			this.OnEveryUpdate = null;
			this.EnableChanged = null;
			this.AliveChanged = null;
		}

		#region Updating
		public void Update()
		{
			Update(Time.deltaTime);
		}

		// Derived classes do not override Update()
		// OnUpdate will hold their individual update logic.
		public VoidFloatCallback OnEveryUpdate;
		public VoidFloatCallback OnNextUpdate;
		private void Update(float delta)
		{
			//if (stopWatch != null)
			//{
			//	stopWatch.Update(gameTime);
			//}

			Holder.Update(delta);

			if (OnNextUpdate != null)
			{
				OnNextUpdate(delta);
				OnNextUpdate = null;
			}

			if (OnEveryUpdate != null)
			{
				OnEveryUpdate(delta);
			}

			this.OnUpdate(delta);
		}
		public virtual void OnUpdate(float delta) { }
		public virtual void Initialize() { }
		#endregion
	}
}