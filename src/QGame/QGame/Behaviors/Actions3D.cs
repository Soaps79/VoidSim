using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Qualis_Game
{
	/// <summary>
	/// Abstract class enforces Drawable3D parent for derived classes
	/// </summary>
	public abstract class Action3D : Action
	{
		protected Drawable3D parentDrawable;

		public Action3D(Drawable3D parent, int lifetime = 0)
			: base(lifetime)
		{
			this.parentDrawable = parent;
		}
	}

	/// <summary>
	/// Smoothly interpolates position from current to target across lifetime
	/// </summary>
	public class A3DTranslate : Action3D
	{
		public override string Name { get { return "A3DTranslate"; } }
		Vector3 startPosition, targetPosition;

		public A3DTranslate(Drawable3D parent, Vector3 targetPosition, int lifetime)
			: base(parent, lifetime)
		{
			this.targetPosition = targetPosition;
		}

		public override void OnInitialize(GameTime gameTime)
		{
			startPosition = parentDrawable.Transform.Position;
			AliveChanged += (Action a) =>
				{
					parentDrawable.Transform.SetTranslation(targetPosition);
				};
		}

		public override void OnUpdate(GameTime gameTime)
		{
			parentDrawable.Transform.SetTranslation(Vector3.SmoothStep(startPosition, targetPosition, this.LifetimeAsZeroToOne));
		}
	}
	
	/// <summary>
	/// Changes mesh tint color for lifetime
	/// Then switches back to original color
	/// </summary>
	class A3DMeshTemporaryTint : Action3D
	{
		public override string Name { get { return "A3DMeshTemporaryTint"; } }
		protected Color holdColor;
		protected Color tintColor;
		public Color TintColor
		{
			get
			{
				return tintColor;
			}
		}
		protected StaticMesh parentMesh;

		public A3DMeshTemporaryTint(Drawable3D parent, StaticMesh parentMesh, int lifetime, Color color)
			: base(parent, lifetime)
		{
			this.parentMesh = parentMesh;
			this.tintColor = color;
		}

		public override void OnInitialize(GameTime gameTime)
		{
			// If mesh is already this color, die
			if (parentMesh.Tint == this.tintColor)
			{
				this.Kill();
				return;
			}

			holdColor = parentMesh.Tint;
			parentMesh.Tint = this.tintColor;
			this.AliveChanged += (Action a) =>
			{
				parentMesh.Tint = holdColor;
			};

		}
	}

	/// <summary>
	/// Tints a mesh for specified lifetime when it takes damage
	/// </summary>
	class A3DMeshTintOnDamage : Action3D
	{
		public override string Name { get { return "A3DMeshTintOnDamage"; } }
		private int tintDuration;
		private Color colorToTint;
		private StaticMesh parentMesh;
		ICombatant parentCombatant;

		public A3DMeshTintOnDamage(ICombatant parent, StaticMesh parentMesh, int tintDuration, Color colorToTint)
			: base(parent as Drawable3D, 1000)
		{
			this.tintDuration = tintDuration;
			this.colorToTint = colorToTint;
			this.parentMesh = parentMesh;
			parentCombatant = parent;
		}

		public override void OnInitialize(GameTime gameTime)
		{
			parentCombatant.Combat().OnHealthChanged += (Object sender, EventArgs args) =>
			{
				Action tint = new A3DMeshTemporaryTint(parentDrawable, parentMesh, tintDuration, colorToTint);
				parentDrawable.Actions.Attach(tint);
			};
		}
	}

	/// <summary>
	/// Causes an object to scroll at the same speed as the terrain
	/// </summary>
	class A3DScrollWithTerrain : Action3D
	{
		public override string Name { get { return "A3DScrollWithTerrain"; } }
		public A3DScrollWithTerrain(Drawable3D parent)
			: base(parent)
		{

		}

		public override void OnUpdate(GameTime gameTime)
		{
			parentDrawable.Transform.MoveOnWorldZ(-Locator.Stats.Speed * gameTime.ElapsedGameTime.Milliseconds);
		} 
	}

	/// <summary>
	/// Causes an object to spin at specified speeds
	/// </summary>
	class A3DSpin : Action3D
	{
		public override string Name { get { return "A3DSpin"; } }
		Vector3 rotation;

		public A3DSpin(Drawable3D parent, Vector3 speed, int lifetime = 0)
			: base(parent, lifetime)
		{
			this.rotation = speed;
		}

		public A3DSpin(Drawable3D parent, float min, float max)
			: base(parent)
		{
			this.rotation = new Vector3(Game1.RandomFloat(min, max), Game1.RandomFloat(min, max), Game1.RandomFloat(min, max));
		}

		public override void OnUpdate(GameTime gameTime)
		{
			parentDrawable.Transform.AdjustYawPitchRoll(this.rotation * gameTime.ElapsedGameTime.Milliseconds);
		}
	}

	/// <summary>
	/// Moves object along its Forward using FlightStats
	/// </summary>
	public class A3DFlyStraight : Action3D
	{
		public override string Name { get { return "A3DFlyStraight"; } }
		FlightStats flightStats;
		bool resetSpeedBeforeApplying;

		public A3DFlyStraight(Drawable3D parent, FlightStats stats, bool resetSpeed, int lifetime = 0)
			: base(parent, lifetime)
		{
			this.flightStats = stats;
			this.resetSpeedBeforeApplying = resetSpeed;
		}

		public override void OnInitialize(GameTime gameTime)
		{
			if (resetSpeedBeforeApplying)
			{
				flightStats.BeginFlight();
			}
		}

		public override void OnUpdate(GameTime gameTime)
		{
			parentDrawable.Transform.MoveForward(flightStats.GetUpdatedSpeed(gameTime)
				* gameTime.ElapsedGameTime.Milliseconds);
		}
	}

	/// <summary>
	/// Dies when object passes 90% bounds height while moving down
	/// </summary>
	public class A3DFlyIntoView : A3DFlyStraight
	{
		public override string Name { get { return "A3DFlyIntoView"; } }

		int flyPercentage;
		public A3DFlyIntoView(Drawable3D parent, FlightStats stats, int fly = 90)
			: base(parent, stats, false)
		{
			flyPercentage = fly;
		}

		public override void OnUpdate(GameTime gameTime)
		{
			base.OnUpdate(gameTime);

			if (ScreenBounds.IsPositionPastPercentage(LocationCheckType.Down, flyPercentage, parentDrawable.Transform.Position))
			{
				this.Kill();
			}
		}
	}

	/// <summary>
	/// Kills parent when position leaves screen bounds
	/// </summary>
	public class A3DKillSelfOffScreen : Action3D
	{
		public override string Name { get { return "A3DKillSelfOffScreen"; } }
		public A3DKillSelfOffScreen(Drawable3D parent)
			: base(parent, 0)
		{

		}

		public override void OnUpdate(GameTime gameTime)
		{
			if(!ScreenBounds.IsOnScreen(parentDrawable.Transform.Position))
			{
                parentDrawable.Alive = false;
			}
		}
	}

	/// <summary>
	/// Dies when parent translates past percentage in given direction
	/// </summary>
	public class A3DPositionPercentageCheck : Action3D
	{
		public override string Name { get { return "A3DPositionPercentageCheck"; } }
		public A3DPositionPercentageCheck(Drawable3D parent, LocationCheckType type, float percentage)
			: base(parent)
		{
			this.OnEveryUpdate += (GameTime g) =>
				{
					if (ScreenBounds.IsPositionPastPercentage(type, percentage, parentDrawable.Transform.Position))
					{
						this.Kill();
					}
				};
		}
	}

	/// <summary>
	/// Interpolates pitch to rotate 360 degrees
	/// </summary>
	public class A3DLoopAround : Action3D
	{
		public override string Name { get { return "A3DLoopAround"; } }
		float incrementPitch;
		float elapsedTurn;

		public A3DLoopAround(Drawable3D parent, FlightStats stats, bool goingUp, int lifetime)
			: base(parent, lifetime)
		{
			incrementPitch = 360.0f / (float)lifetime;
			if (!goingUp)
			{
				incrementPitch *= -1;
			}
		}

		public override void OnInitialize(GameTime gameTime)
		{
			this.OnEveryUpdate = IncrementLoop;

			this.AliveChanged += (Action a) =>
			{
				if (!a.IsAlive)
				{
					parentDrawable.Transform.FlattenPitch();
					this.OnEveryUpdate = null;
				}
			};
		}

		public void IncrementLoop(GameTime gameTime)
		{
			parentDrawable.Transform.AdjustPitch(incrementPitch * gameTime.ElapsedGameTime.Milliseconds);
			elapsedTurn += incrementPitch * gameTime.ElapsedGameTime.Milliseconds;
		}
	}

	/// <summary>
	/// Interpolates parent's yaw and roll
	/// </summary>
	public class A3DTurnWithLean : Action3D
	{
		public override string Name { get { return "A3DTurn"; } }
		float incrementRotation;
		float incrementRoll;

		float maxRotation;
		float maxRoll;

		float halfwayDone;
		bool isHalfwayDone;

		public A3DTurnWithLean(Drawable3D parent, int lifetime, float rotationAmount, float rollAmount)
			:	base(parent, lifetime)
		{
			incrementRotation = rotationAmount / lifetime;
			incrementRoll = rollAmount / lifetime;
			maxRotation = rotationAmount;
			maxRoll = rollAmount;
			halfwayDone = lifetime / 2;
		}

		public override void OnUpdate(GameTime gameTime)
		{
			float elapsed = (float)gameTime.ElapsedGameTime.Milliseconds;

			parentDrawable.Transform.AdjustRoll(incrementRoll * elapsed);
			parentDrawable.Transform.RotateOnWorldY(incrementRotation * elapsed);

			if (!isHalfwayDone && this.RemainingLifetime < halfwayDone)
			{
				incrementRoll *= -1;
				isHalfwayDone = true;
			}
		}
	}

	/// <summary>
	/// Smoothly interpolates up and down the world Y
	/// </summary>
    public class A3DHover : Action3D
    {
		public override string Name { get { return "A3DHover"; } }
        float maxHover;
		float currentAmount;
		float hoverIncrement;
		bool goingUp;

        public A3DHover(Drawable3D parent, int lifetime, float distance, int cycleMS)
			:	base(parent, lifetime)
		{
			maxHover = distance;
			hoverIncrement = (distance * 2) / (float)cycleMS;
		}

		public override void OnUpdate(GameTime gameTime)
		{
			parentDrawable.Transform.MoveOnWorldY((float)Math.Sin(gameTime.TotalGameTime.TotalMilliseconds / 500) * maxHover);//(hoverIncrement * gameTime.ElapsedGameTime.Milliseconds));

			base.Update(gameTime);
		}
    }

	/// <summary>
	/// Snaps parent to look at target
	/// </summary>
    public class A3DTurnToTarget : Action3D
    {
		public override string Name { get { return "A3DTurnToTarget"; } }
        float maxTurn;
        float maxAmount;
        float currentTurn = 0;

        Drawable3D drawTarget;
        public Drawable3D DrawTarget
        {
            get
            {
                return drawTarget;
            }
            set
            {
                drawTarget = value;
            }
        }

        public A3DTurnToTarget(Drawable3D parent, Drawable3D target, int lifetime = 0)
            : base(parent, lifetime)
        {
            drawTarget = target;            
        }

        public override void OnUpdate(GameTime gameTime)
        {
            //maxAmount = angleToTarget(parentDrawable.Transform.Position, drawTarget.Transform.Position);
            //if (maxAmount > 0)
            //{
            //    parentDrawable.Transform.AdjustYaw(maxAmount, AngleMeasureType.Radians);
            //}

            parentDrawable.Transform.LookAt(drawTarget.Transform.Position, Vector3.Up);
        }

        private float angleToTarget(Vector3 source, Vector3 target)
        {
            float dX = (target.X - source.X);
            float dZ = (target.Z - source.Z);

            return (float)Math.Atan2(dZ, dX);
        }
    }

	/// <summary>
	/// Adds specified grow rate to parent on each update
	/// </summary>
    public class A3DGrow : Action3D
    {
		public override string Name { get { return "A3DGrow"; } }
        Vector3 scaleRate;

        public A3DGrow(Drawable3D parent, float rate, int lifetime = 0)
            : base(parent, lifetime)
        {
            scaleRate = new Vector3(rate, rate, rate);
        }

        public override void OnUpdate(GameTime gameTime)
        {
			parentDrawable.Transform.AdjustScale(scaleRate);       
		}
    }

	/// <summary>
	/// TODO: Tim, describe me
	/// </summary>
    public class A3DFollow : Action3D
    {
		public override string Name { get { return "A3DFollow"; } }
        Enemy target;
        A3DTurnToTarget turnToTargetAction;
        int maxTargets;
        int currentTarget = 0;

        public A3DFollow(Drawable3D parent, FlightStats flightstats, int numtargets, int lifetime = 0)
            : base(parent, lifetime)
        {
            parentDrawable.Actions.Attach( new A3DFlyStraight(parent, flightstats, false));
			turnToTargetAction = new A3DTurnToTarget(parent, target);
            parentDrawable.Actions.Attach( turnToTargetAction );
            maxTargets = numtargets;

            GetNextTarget();

            //KeyValueDisplay.Instance.Add("Target",
            //    delegate()
            //    {
            //        return (Object)enemy.Name;
            //    }
            //);
        }

        public override void OnUpdate(GameTime gameTime)
        {
        }

        private void GetNextTarget()
        {
            currentTarget++;
            if (currentTarget > maxTargets)
            {
                parentDrawable.Alive = false;
                return;
            }

			target = Locator.EnemyTracker.GetClosestEnemy(parentDrawable.Transform.Position);
			if (target == null || !ScreenBounds.IsOnScreen(target.Transform.Position))
            {
                parentDrawable.Alive = false;
                return;
            }
			turnToTargetAction.DrawTarget = target;

			if (target.EnemyType == EnemyType.Boss)
			{
				int bonusDamage = maxTargets - currentTarget;
				currentTarget = maxTargets;
				WeaponProjectile temp = parentDrawable as WeaponProjectile;
				if(temp != null)
				{
					temp.Combat().Damage = 10 * bonusDamage;
					temp.Combat().TakeDamage(temp.Combat().CurrentHealth - 1);
				}

				return;
			}

			target.AliveChanged += (Entity e) =>
			{
				this.OnNextUpdate += (GameTime g) =>
				{
					GetNextTarget();
				}; 
			};
        }
    }

	/// <summary>
	/// TODO: Tim, describe me
	/// </summary>
    public class A3DTarget : Action3D
    {
		public override string Name { get { return "A3DTarget"; } }
        Drawable3D drawTarget;
		FlightStats flightstats;

        public A3DTarget(Drawable3D parent, FlightStats fs, Drawable3D target, int lifetime = 0)
            : base(parent, lifetime)
        {
            drawTarget = target;
			flightstats = fs;
        }

        public override void OnInitialize(GameTime gameTime)
        {
            parentDrawable.Actions.Attach(new A3DFlyStraight(parentDrawable, flightstats, false));

            parentDrawable.Actions.Attach(new A3DTurnToTarget(parentDrawable, drawTarget));

            drawTarget.AliveChanged += (Entity e) =>
            {
                if (!drawTarget.Alive)
                {
                    parentDrawable.Alive = false;
                }
            };
        }
        public override void OnUpdate(GameTime gameTime)
        {
        }
    }

	/// <summary>
	/// TODO: Tim, describe me
	/// </summary>
	public class A3DWaitForTarget : Action3D
	{
		public override string Name { get { return "A3DWaitForTarget"; } }
		FlightStats flightstats;
		Enemy target;

		public A3DWaitForTarget(Drawable3D parent, FlightStats fs, int lifetime = 0)
			: base(parent, lifetime)
		{
			flightstats = fs;
		}

		public override void OnUpdate(GameTime gameTime)
		{
			FindAvailablEnemy();
		}

		private void FindAvailablEnemy()
		{
			target = Locator.EnemyTracker.GetFirstTargettableEnemy();

			if(target != null)
			{
				this.SetNext(new A3DTarget(parentDrawable, flightstats, target));
				target.AliveChanged += (Entity e) =>
					{
						if (!e.Alive)
						{
							parentDrawable.Actions.ClearAllButThis(new A3DWaitForTarget(parentDrawable, flightstats));
						}
					};

				parentDrawable.AliveChanged += (Entity e) =>
					{
						if (!e.Alive)
						{
							target.IsTargetted = false;
						}
					};

				this.Kill();
			}
		}
	}

	public class A3DAttachToBone : Action3D
	{
		public override string Name { get { return "A3DAttachToBone"; } }

		ICollidable targetDrawable;
		String boneName;
		ModelBone bone;

		Vector3 scale;
		Vector3 position;
		Quaternion rotation;
		Vector3 rot;
		public A3DAttachToBone(Drawable3D parent, ICollidable target, String bonename, int lifetime = 0)
			: base(parent, lifetime)
		{
			targetDrawable = target;
			boneName = bonename;

			int count = targetDrawable.Mesh().MainModel.Bones.Count;
			for (int i = 0; i < count; i++)
			{
				if (targetDrawable.Mesh().MainModel.Bones[i].Name == boneName)
				{
					bone = targetDrawable.Mesh().MainModel.Bones[i];
				}
			}
		}

		//public override void OnInitialize(GameTime gameTime)
		//{

		//}

		public override void OnUpdate(GameTime gameTime)
		{
			bone.Transform.Decompose(out scale, out rotation, out position);
			////rotation.Normalize();
			//rot.X = rotation.X;
			//rot.Y = rotation.Y;
			//rot.Z = rotation.Z;
			//parentDrawable.Transform.AdjustYawPitchRoll(rot);
			parentDrawable.Transform.SetTranslation(targetDrawable.Mesh().Transform.Position + position);
		}
	}

	class A3DBarrelRoll : Action3D
	{
		public override string Name { get { return "A3DBarrelRoll"; } }
		float incrementRoll;
		public A3DBarrelRoll(Drawable3D parent, int fullRotateTime, int numberOfRotations)
			: base(parent, fullRotateTime * numberOfRotations)
		{
			incrementRoll = 360.0f / (float)fullRotateTime;
			this.AliveChanged += (Action a) =>
				{
					if (!a.IsAlive)
					{
						parent.Transform.FlattenRoll();
					}
				};
		}

		public override void OnUpdate(GameTime gameTime)
		{
			parentDrawable.Transform.AdjustRoll(incrementRoll * gameTime.ElapsedGameTime.Milliseconds);
		}
	}

	class A3DAdjustPitch : Action3D
	{
		public override string Name { get { return "A3DLowerPitch"; } }
		float incrementAmount;

		public A3DAdjustPitch(Drawable3D parent, int lifetime, float amount)
			: base(parent, lifetime)
		{
			incrementAmount = amount / (float)lifetime;
		}

		public override void OnUpdate(GameTime gameTime)
		{
			parentDrawable.Transform.AdjustPitch(incrementAmount * gameTime.ElapsedGameTime.Milliseconds);
		}
	}

	class A3DPopup : Action3D
	{
		public override string Name { get { return "A3DPopup"; } }
		float startHeight, targetHeight;


		public A3DPopup(Drawable3D parent, int targetHeight, int lifetime, int startHeight = -9999)
			: base(parent, lifetime)
		{
			this.targetHeight = targetHeight;
			this.startHeight = startHeight;
		}

		public override void OnInitialize(GameTime gameTime)
		{
			if (startHeight == -9999)
			{
				startHeight = parentDrawable.Transform.Position.Y;
			}
		}

		public override void OnUpdate(GameTime gameTime)
		{
			Vector3 vec = parentDrawable.Transform.Position;
			vec.Y = Game1.GlobalHeight + (float)(Math.Sin(this.LifetimeAsZeroToOne * MathHelper.Pi) * targetHeight);
			parentDrawable.Transform.SetTranslation(vec);
		}
	}

	/// <summary>
	/// Moves object along its Up using FlightStats
	/// </summary>
	public class A3DFlyUp : Action3D
	{
		public override string Name { get { return "A3DFlyUp"; } }
		FlightStats flightStats;
		bool resetSpeedBeforeApplying;

		public A3DFlyUp(Drawable3D parent, FlightStats stats, bool resetSpeed, int lifetime = 0)
			: base(parent, lifetime)
		{
			this.flightStats = stats;
			this.resetSpeedBeforeApplying = resetSpeed;
		}

		public override void OnInitialize(GameTime gameTime)
		{
			if (resetSpeedBeforeApplying)
			{
				flightStats.BeginFlight();
			}
		}

		public override void OnUpdate(GameTime gameTime)
		{
			parentDrawable.Transform.MoveUp(flightStats.GetUpdatedSpeed(gameTime)
				* gameTime.ElapsedGameTime.Milliseconds);
		}
	}

	public class A3DFlyDown : Action3D
	{
		public override string Name { get { return "A3DFlyDown"; } }
		FlightStats flightStats;
		bool resetSpeedBeforeApplying;

		public A3DFlyDown(Drawable3D parent, FlightStats stats, bool resetSpeed, int lifetime = 0)
			: base(parent, lifetime)
		{
			this.flightStats = stats;
			this.resetSpeedBeforeApplying = resetSpeed;
		}

		public override void OnInitialize(GameTime gameTime)
		{
			if (resetSpeedBeforeApplying)
			{
				flightStats.BeginFlight();
			}
		}

		public override void OnUpdate(GameTime gameTime)
		{
			parentDrawable.Transform.MoveUp((flightStats.GetUpdatedSpeed(gameTime)
				* gameTime.ElapsedGameTime.Milliseconds) * -1);
		}
	}

	public class A3DKillBelowTerrain : Action3D
	{
		public override string Name { get { return "A3DKillBelowTerrain"; } }

		public A3DKillBelowTerrain(Drawable3D parent)
			: base(parent, 0)
		{
		}

		public override void OnUpdate(GameTime gameTime)
		{
			if (parentDrawable.Transform.Position.Y < 0)
			{
				parentDrawable.Alive = false;
			}
		}
	}
}
