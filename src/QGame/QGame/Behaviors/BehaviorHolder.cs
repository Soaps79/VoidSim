using System;
using System.Collections.Generic;
using System.Linq;
using QGame;

namespace Behaviors
{
	public sealed class BehaviorHolder
	{
		#region Variables
		private LinkedList<Behavior> behaviorList = new LinkedList<Behavior>();
		long timeModifier;
		#endregion

		// Holder needs to clear children when parent dies, or else their 
		// references will cause parents to not be GC'd
		public BehaviorHolder(ILiving parent)
		{
			parent.AliveChanged += OnParentAliveChanged;
		}
		
		public int Count { get { return behaviorList.Count; } }
		public int AliveCount { get { return behaviorList.Count(b => b.IsAlive); } }

		#region Interface
		/// <summary>
		/// Returns whether or not Holder has Behaviors, disregarding IsAlive
		/// </summary>
		public bool HasBehaviors
		{
			get
			{
				return behaviorList.Any();
			}
		}

		/// <summary>
		/// Returns number of Behaviors IsAlive
		/// </summary>
		public bool HasAliveBehaviors
		{
			get
			{
				return behaviorList.Any(i => i.IsAlive);
			}
		}

		/// <summary>
		/// Will modify elapsed times sent to behaviors by divisor, and will revert
		/// back to 0 when lifetime expires
		/// </summary>
		/// TimeModifier system not tested in Unity
		public void SetTimeModifier(int divisor, int lifetime = 0)
		{
			if (divisor == 0)
			{
				timeModifier = 0;
				return;
			}
			else if (timeModifier != 0)
			{
				return;
			}
			else
			{
				timeModifier = divisor;
				Behavior a = new WaitBehavior(lifetime * divisor);
				a.AliveChanged += (Behavior act) =>
				{
					if (!act.IsAlive)
					{
						SetTimeModifier(0);
					}
				};
				Attach(a);
			}
		}

		// iterate through behaviorList
		// Dead items - if they have a child Behavior, attach child Behavior then detach item
		// update item's lifetime
		// update any item not paused
		public void Update(float delta)
		{
			var deltaToPass = delta;

			if (timeModifier != 0)
			{
				deltaToPass /= timeModifier;
			}

			LinkedListNode<Behavior> index = behaviorList.First;
			//LinkedListNode<Behavior> active = index;
			Behavior activeBehavior;

			while (index != null)
			{
				activeBehavior = index.Value;
				index = index.Next;
				
				if (!activeBehavior.IsAlive)
				{
					Behavior next = activeBehavior.GetNext();

					if (next != null)
					{
						Attach(next);
					}

					Detach(activeBehavior);
				}
				else if (!activeBehavior.IsPaused)
				{
					if (!activeBehavior.Initialized)
					{
						activeBehavior.Initialize();
					}

					activeBehavior.Update(deltaToPass);
				}

			}
		}

		public void Attach(Behavior behavior)
		{
			behaviorList.AddFirst(behavior);
			behavior.Attached = true;
			behavior.ParentHolder = this;
		}

		public bool IsBehaviorAttached(string name)
		{
			LinkedListNode<Behavior> index = behaviorList.First;

			while (index != null)
			{
				if (index.Value.Name == name)
				{
					return true;
				}

				index = index.Next;
			}

			return false;
		}

		public Behavior GetAttachedBehavior(string name)
		{
			return behaviorList.FirstOrDefault(i => i.Name == name);
		}

		public void Clear()
		{
			behaviorList.Clear();
			timeModifier = 0;
		}

		public void ClearAllButThis(Behavior a)
		{
			// do we want to Detach all current entities, calling their AliveChanged?
			this.Clear();
			behaviorList.AddLast(a);
		}

		#endregion
        public event EventHandler<EventArgs> DetachedLastBehavior;
		private void Detach(Behavior behavior)
		{
			behaviorList.Remove(behavior);
			behavior.Attached = false;
			behavior.IsAlive = true;

            if (!HasBehaviors)
            {
                if (DetachedLastBehavior != null)
                {
                    DetachedLastBehavior(this, null);
                    DetachedLastBehavior = null;
                }
            }
		}

		//// clear list to release memory
		private void OnParentAliveChanged(ILiving sender)
		{
			if (!sender.IsAlive)
			{
				Clear();
			}
		}

	}
}
