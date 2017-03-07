using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Qualis_Game
{
	public class SampleAction1 : Action
	{
		public override string Name { get { return "SampleAction1"; } }
		public SampleAction1(int lifetime)
			: base(lifetime)
		{
			if (lifetime == 0)
			{
				this.Kill();
			}

			// functions with the signature void Function(GameTime)
			// can be set for either a single Update or every one
			//this.OnEveryUpdate += OutputRemaining;
			//this.OnNextUpdate += SayHi;
		}

		public override void OnUpdate(GameTime gameTime)
		{
			// this override is called every cycle
			// serves as Update() as we have been using it
		}

		private void OutputRemaining(GameTime gameTime)
		{
			Console.WriteLine(this.RemainingLifetime.ToString());
		}

		private void SayHi(GameTime gameTime)
		{
			Console.WriteLine("Hi");
		}
	}

	/// <summary>
	/// Used to attach multiple Actions when one dies.
	/// All Actions passed into AddAction() will be attached to
	/// the parent holder the cycle after this object is attached
	/// </summary>
	public class SampleAction2 : Action
	{
		public override string Name { get { return "SampleAction2"; } }
		Queue<Action> toAttach = new Queue<Action>();

		public SampleAction2()
			: base(1)
		{
			// the public delegates and events can either
			// have functions hooked into them:
			//this.AliveChanged += AttachPack;

			// or lambdas
			this.AliveChanged += (Action a) =>
			{
				while (toAttach.Count > 0)
				{
					ParentHolder.Attach(toAttach.Dequeue());
				}
			};
		}

		public void AddAction(Action action)
		{
			toAttach.Enqueue(action);
		}

		private void AttachPack(Object sender, EventArgs args)
		{
			while (toAttach.Count > 0)
			{
				ParentHolder.Attach(toAttach.Dequeue());
			}
		}


	}
}
