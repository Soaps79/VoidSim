using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Behaviors
{
	#region Utility Behaviors
	/// <summary>
	/// Use to delay, usage includes attaching a NextBehavior or hooking in to its AliveChanged
	/// </summary>
	public class WaitBehavior : Behavior
	{
		public override string Name { get { return "WaitBehavior"; } }

		public WaitBehavior(float lifetime)
			: base(lifetime)
		{
			if (lifetime == 0)
			{
				this.Kill();
			}
		}
	}

	/// <summary>
	/// Used to attach multiple Behaviors when one dies.
	/// All Behaviors passed into AddBehavior() will be attached to
	/// the parent holder the cycle after this object is attached
	/// </summary>
	public class BehaviorPack : Behavior
	{
		public override string Name { get { return "BehaviorPack"; } }
		Queue<Behavior> toAttach = new Queue<Behavior>();

		public BehaviorPack(float lifetime = -1)
			: base(lifetime)
		{
			AliveChanged += (behavior) =>
			{
				while (toAttach.Count > 0)
				{
					ParentHolder.Attach(toAttach.Dequeue());
				}
			};
		}

		public BehaviorPack(IEnumerable<Behavior> behaviors, float lifetime = -1)
			: this(lifetime)
		{
			foreach (var behavior in behaviors)
			{
				AddBehavior(behavior);
			}
		}

		public void AddBehavior(Behavior Behavior)
		{
			toAttach.Enqueue(Behavior);
		}
	}
	#endregion

	#region Audio Holder
	///// <summary>
	///// Interpolates music volume start to end across lifetime
	///// </summary>
	//public class MusicFadeAction : Behavior
	//{
	//	public override string Name { get { return "MusicFadeAction"; } }
	//	float startVolume, endVolume;

	//	public MusicFadeAction(float startVolume, float endVolume, int lifetime)
	//		: base(lifetime)
	//	{
	//		this.startVolume = startVolume;
	//		this.endVolume = endVolume;
	//	}

	//	public override void OnUpdate(GameTime gameTime)
	//	{
	//		AudioManager.Instance.MusicVolume = MathHelper.Lerp(startVolume, endVolume, this.LifetimeAsZeroToOne);
	//	}
	//}
	#endregion

	#region BillBoard Holder
	///// <summary>
	///// Enforces a parent billboard for derived classes
	///// </summary>
	//abstract public class ActionBB : Behavior
	//{
	//	protected Billboard parentBillboard;

	//	public ActionBB(Billboard parent, int lifetime = 0)
	//		: base(lifetime)
	//	{
	//		parentBillboard = parent;
	//	}
	//}
	
	///// <summary>
	///// Smoothly interpolates BillboardText opacity from current to target across lifetime
	///// </summary>
	//public class ABBFade : ActionBB
	//{
	//	public override string Name { get { return "ABBFade"; } }
	//	float startOpacity;
	//	float targetOpacity;

	//	public ABBFade(Billboard parent, int lifetime, float targetOpacity)
	//		: base(parent, lifetime)
	//	{
	//		this.targetOpacity = targetOpacity;
	//	}

	//	public override void OnInitialize(GameTime gameTime)
	//	{
	//		this.startOpacity = this.parentBillboard.Opacity;
	//	}

	//	public override void OnUpdate(Microsoft.Xna.Framework.GameTime gameTime)
	//	{
	//		parentBillboard.Opacity =
	//			MathHelper.SmoothStep(startOpacity, targetOpacity, LifetimeAsZeroToOne);
	//	}
	//}

	///// <summary>
	///// Smoothly interpolates BillboardText TextSize from current to target across lifetime
	///// </summary>
	//public class ABBTextSizeChange : ActionBB
	//{
	//	public override string Name { get { return "ABTSizeChange"; } }
	//	float startSize, targetSize;
	//	BillboardText parentText;


	//	public ABBTextSizeChange(BillboardText parent, float targetSize, int lifetime)
	//		: base(parent, lifetime)
	//	{
	//		parentText = parent;
	//		this.targetSize = targetSize;
	//	}

	//	public override void OnInitialize(GameTime gameTime)
	//	{
	//		this.startSize = parentText.TextSize;
	//		AliveChanged += (Behavior a) =>
	//			{
	//				parentText.TextSize = targetSize;
	//			};
	//	}

	//	public override void OnUpdate(GameTime gameTime)
	//	{
	//		parentText.TextSize = MathHelper.SmoothStep(startSize, targetSize, this.LifetimeAsZeroToOne);
	//	}
	//}
	#endregion

	//public class ICombatantHealthFill : Behavior
	//{
	//	public override string Name { get { return "ICombatantHealthFill"; } }

	//	float totalAmount;
	//	ICombatant parentCombatant;
	//	float startAmount;

	//	public ICombatantHealthFill(ICombatant parent, int max, int lifetime = 0)
	//		: base(lifetime)
	//	{
	//		totalAmount = max;
	//		parentCombatant = parent;
	//	}

	//	public override void OnInitialize(GameTime gameTime)
	//	{
	//		startAmount = parentCombatant.Combat().CurrentHealth;
	//		this.AliveChanged += (Behavior act) =>
	//		{
	//			if (!act.IsAlive)
	//			{
	//				parentCombatant.Combat().CurrentHealth = (int)totalAmount;
	//			}
	//		};
	//	}

	//	public override void OnUpdate(GameTime gameTime)
	//	{
	//		parentCombatant.Combat().CurrentHealth = (int)MathHelper.SmoothStep(startAmount, totalAmount, this.LifetimeAsZeroToOne);
	//		//parentEnemy.Combat().TakeDamage((int)-(amountPer * gameTime.ElapsedGameTime.Milliseconds));
	//	}
	//}

	//public class ExplodeMineAtGlobalHeight : Action3D
	//{
	//	public override string Name { get { return "ExplodeMineAtGlobalHeight"; } }
	//	private float explodeAtY;
	//	public ExplodeMineAtGlobalHeight(Drawable3D mine, float offsetToGlobal = 0)
	//		: base(mine)
	//	{
	//		explodeAtY = Game1.GlobalHeight + offsetToGlobal;
	//		parentDrawable.AliveChanged += MineHitPlayerOnDescent;
	//	}

	//	public override void OnInitialize(GameTime gameTime)
	//	{
		
	//	}

	//	public override void OnUpdate(GameTime gameTime)
	//	{
	//		if (parentDrawable.Transform.Position.Y < explodeAtY)
	//		{
	//			parentDrawable.IsAlive = false;
	//			ParticleComponent component = ParticleFactory.Instance.RequestParticleSystem(
	//				ParticleFactory.NameFieryExplosion, parentDrawable);
	//			AudioManager.Instance.PlaySound("ExplosionMine");
	//		}
	//	}

	//	private void MineHitPlayerOnDescent(Entity e)
	//	{
	//		if (!e.IsAlive)
	//		{
	//			ParticleComponent component = ParticleFactory.Instance.RequestParticleSystem(
	//				ParticleFactory.NameFieryExplosion, parentDrawable);
	//			AudioManager.Instance.PlaySound("ExplosionMine");
	//		}
	//	}

	//}
}
