using QGame;

namespace Behaviors
{
	public delegate void BehaviorCallback(Behavior sender);

	/// <summary>
	/// An instruction set for a parent object, an individual behavior.
	/// Will update as long as it is attached to an BehaviorHolder and is not paused.
	/// Simple derived classes will add variables and override OnUpdate()
	/// Callbacks are triggered OnNextUpdate, OnEveryUpdate, and AliveChanged
	/// BehaviorHolder will detach an Behavior whose IsAlive value is false, and attach NextBehavior
	/// </summary>
	public abstract class Behavior
	{
		#region Variables

		private bool _isAlive = true;
		private bool _isAttached = false;
		private bool _isInitialized = false;
		private bool _isPaused = false;
		private Behavior _nextBehavior;

		private float _lifetime;
		private float _elapsedLifetime;

		private BehaviorHolder _parentHolder;

		//protected StopWatch stopWatch;
		//public void EnableStopWatch()
		//{
		//	if (stopWatch == null)
		//	{
		//		stopWatch = new StopWatch();
		//	}
		//}

		#endregion;

		#region Properties

		public abstract string Name { get; }

		public BehaviorHolder ParentHolder
		{
			get
			{
				return _parentHolder;
			}
			set
			{
				_parentHolder = value;
			}
		}

		public float RemainingLifetime
		{
			get
			{
				return _lifetime - _elapsedLifetime;
			}
		}
		public float Lifetime
		{
			get
			{
				return _lifetime;
			}
		}
		public float ElapsedLifetime
		{
			get
			{
				return _elapsedLifetime;
			}
		}
		public float LifetimeAsZeroToOne
		{
			get
			{
				return _elapsedLifetime / _lifetime;
			}
		}

		public bool Attached
		{
			get { return _isAttached; }
			set
			{
				_isAttached = value;
				if (!_isAttached)
				{
					ParentHolder = null;
				}
			}
		}


		public bool Initialized
		{
			get { return _isInitialized; }
			set { if (!value) _isInitialized = value; }
		}

		public BehaviorCallback AliveChanged;
		public BehaviorCallback OnDeath;
		public bool IsAlive
		{
			get { return _isAlive; }
			set
			{
				_isAlive = value; 
				if(AliveChanged != null)
				{
					AliveChanged(this);
				}
				if (!_isAlive && OnDeath != null)
				{
					OnDeath(this);
				}
			}
		}

		public Behavior SetNext(Behavior a)
		{
			_nextBehavior = a;
			return this._nextBehavior;
		}
		public Behavior GetNext()
		{
			return _nextBehavior;
		}

		public bool IsPaused
		{
			get { return _isPaused; }
			set { _isPaused = value; }
		}
		#endregion;

		#region Updating
		public VoidFloatCallback OnNextUpdate;
		public VoidFloatCallback OnEveryUpdate;
		/// <summary>
		/// This function processes both OnNextUpdate and OnEveryUpdate
		/// as well as calling the virtual OnUpdate()
		/// </summary>
		public void Update(float delta)
		{
			UpdateTiming(delta);

			//if (stopWatch != null)
			//{
			//	stopWatch.Update(gameTime);
			//}

			if (OnEveryUpdate != null)
			{
				OnEveryUpdate(delta);
			}

			if (OnNextUpdate != null)
			{
				OnNextUpdate(delta);
				OnNextUpdate = null;
			}

			this.OnUpdate(delta);
		}

		public virtual void OnUpdate(float delta) { }
		#endregion

		public Behavior(float lifetime = 0)
		{
			_lifetime = lifetime;
		}

		public void Initialize()
		{
			OnInitialize();
			_isInitialized = true;
		}
		public virtual void OnInitialize() { }

		public void ResetElapsedLifetime() { _elapsedLifetime = 0; }

		public void Reset(float newLifetime)
		{
			_lifetime = newLifetime;
			ResetElapsedLifetime();
			_isInitialized = false;
			IsAlive = true;
		}

		private void UpdateTiming(float elapsed)
		{
			_elapsedLifetime += elapsed;

			if (_lifetime != 0)
			{
				if (_elapsedLifetime > _lifetime)
				{
					Kill();
				}
			}
		}

		public void Kill()
		{
			IsAlive = false;
		}

	}
}
