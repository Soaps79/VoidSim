using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Behaviors
{
	/// <summary>
	/// Abstract class enforces Drawable2D parent for derived classes
	/// </summary>
	abstract public class Behavior2D : Behavior
	{
		protected Transform _parentTransform;
		protected SpriteRenderer _parentSprite;
		//protected Drawable2D parentDrawable;

		public Behavior2D(Transform parentTransform, SpriteRenderer parentSprite, float lifetime = 0)
			:base(lifetime)
		{
			_parentTransform = parentTransform;
			_parentSprite = parentSprite;
		}
	}

	/// <summary>
	/// Manipulates the opacity of a Drawable2D. Will interpolate from the object's current
	/// opacity to the target opacity over the specified time. Most fades will not specify the 
	/// last parameter, in favor of fading from their current Opacity to the target.
	/// </summary>
	//class A2DFade : Behavior2D
	//{
	//	public override string Name { get { return "A2DFade"; } }
	//	public float StartOpacity { get; set; }
	//	public float TargetOpacity { get; set; }
		
	//	/// <param name="startOpacity">If left as default, startOpacity will be object's current opacity</param>
	//	public A2DFade(Transform parent, float targetOpacity, int lifetime, float startOpacity = 1.1f)
	//		: base(parent, lifetime)
	//	{
	//		this.StartOpacity = startOpacity;
	//		this.TargetOpacity = targetOpacity;
	//	}

	//	public A2DFade(Transform parent, int lifetime, params string[] parameters)
	//		: base(parent, lifetime)
	//	{
	//		this.StartOpacity = (float) Convert.ToDouble(parameters[0]);
	//		this.TargetOpacity = (float) Convert.ToDouble(parameters[1]);
	//	}

	//	public override void OnInitialize()
	//	{
	//		// if no start opacity was requested, start the fade from current opacity
	//		if (StartOpacity == 1.1f)
	//		{

	//			this.StartOpacity = _parentTransform. parentDrawable.Transform.Opacity;
	//		}
	//		// otherwise, start MUST BE a 0 to 1 value
	//		else if (StartOpacity < 0.0f || StartOpacity > 1.0f)
	//		{
	//			throw new InvalidOperationException("A2DFade - Opacity must be between 0 and 1");
	//		}
	//	}

	//	public override void OnUpdate(Microsoft.Xna.Framework.GameTime gameTime)
	//	{
	//		parentDrawable.Transform.Opacity = 
	//			MathHelper.SmoothStep(StartOpacity, TargetOpacity, LifetimeAsZeroToOne);
	//	}
	//}

	public class B2DColorTransition : Behavior2D
	{
		private Color _startColor;
		private Color _targetColor;

		public override string Name
		{
			get { return "Behavior2D"; }
		}
		
		public B2DColorTransition(SpriteRenderer parentSprite, Color targetColor, float lifetime) 
			: base(null, parentSprite, lifetime)
		{
			_targetColor = targetColor;
			OnEveryUpdate += LinearTransition;
		}

		public override void OnInitialize()
		{
			_startColor = _parentSprite.color;
			AliveChanged += (b) =>
				                {
									if (!b.IsAlive)
									{
										_parentSprite.color = _targetColor;
									}
				                };
		}

		public void LinearTransition(float delta)
		{
			_parentSprite.color = Color.Lerp(_startColor, _targetColor, LifetimeAsZeroToOne);
		}
	}

	public class B2DScaleTween : Behavior2D
	{
		public override string Name { get { return "B2DScaleTween"; } }

		private Vector3 _startScale;
		private Vector3 _targetScale;

		private bool _isSmooth;

		public B2DScaleTween(Transform parentTransform, Vector3 targetScale, float lifetime, bool smooth = true) 
			: base(parentTransform, null, lifetime)
		{
			_targetScale = targetScale;
			_isSmooth = smooth;
		}

		public override void OnInitialize()
		{
			_startScale = _parentTransform.localScale;
			if (_isSmooth)
			{
				OnEveryUpdate += SmoothTween;
			}
			else
			{
				OnEveryUpdate += LinearTween;
			}
		}

		public void LinearTween(float delta)
		{
			_parentTransform.localScale = new Vector3(
				Mathf.Lerp(_startScale.x, _targetScale.x, LifetimeAsZeroToOne),
				Mathf.Lerp(_startScale.y, _targetScale.y, LifetimeAsZeroToOne), 0);
		}

		public void SmoothTween(float delta)
		{
			_parentTransform.localScale = new Vector3(
				Mathf.SmoothStep(_startScale.x, _targetScale.x, LifetimeAsZeroToOne),
				Mathf.SmoothStep(_startScale.y, _targetScale.y, LifetimeAsZeroToOne), 0);
		}
	}

	/// <summary>
	/// Translates parent from its current position to target position
	/// in specified amount of time.
	/// </summary>
	public class B2DTranslate : Behavior2D
	{
		public override string Name { get { return "B2DTranslate"; } }
		Vector2 _startPosition;
		Vector2 _targetPosition;
		readonly bool _useSmooth;

		public B2DTranslate(Transform parentTransform, Vector2 targetPosition, SpriteRenderer parentSprite = null, float lifetime = 0, bool smooth = true)
			: base(parentTransform, parentSprite, lifetime)
		{
			_targetPosition = targetPosition;
			_useSmooth = smooth;
		}

		//public B2DTranslate(Transform parentTransform, Sprite parentSprite, int lifetime, params string[] parameters)
		//	: base(parentTransform, parentSprite, lifetime)
		//{
		//	this._targetPosition = InterfaceLoader.GetVector2(parameters[0]);
		//	this._useSmooth = true;
		//}

		public override void OnInitialize()
		{
			_startPosition = _parentTransform.position;
			if (_useSmooth)
			{
				OnEveryUpdate += SmoothTranslate;
			}
			else
			{
				OnEveryUpdate += LinearTranslate;
			}
		}

		private void LinearTranslate(float delta)
		{
			Vector2 newVec = Vector2.zero;
			newVec.x = Mathf.Lerp(_startPosition.x, _targetPosition.x, LifetimeAsZeroToOne);
			newVec.y = Mathf.Lerp(_startPosition.y, _targetPosition.y, LifetimeAsZeroToOne);
			_parentTransform.position = newVec;
		}

		private void SmoothTranslate(float delta)
		{
			Vector2 newVec = Vector2.zero;
			newVec.x = Mathf.SmoothStep(_startPosition.x, _targetPosition.x, LifetimeAsZeroToOne);
			newVec.y = Mathf.SmoothStep(_startPosition.y, _targetPosition.y, LifetimeAsZeroToOne);
			_parentTransform.position = newVec;
		}
	}

	/// <summary>
	/// Repeatedly raises and lowers the Opacity of parent Drawable2D
	/// </summary>
	//class A2DBlink : Behavior2D
	//{
	//	public override string Name { get { return "A2DBlink"; } }

	//	public int FadeInLength, FadeOutLength;
	//	public int FadeBothLength
	//	{
	//		set
	//		{
	//			if (value > 0)
	//			{
	//				FadeInLength = value;
	//				FadeOutLength = value;
	//			}
	//		}
	//	}
	//	float maxOpacity, minOpacity;
	//	bool isFadingIn;
	//	A2DFade fadeInAction, fadeOutAction;
	//	int delayBeforeStart;
	//	public int DelayBetweenBlinks;

	//	/// <summary>
	//	/// 
	//	/// </summary>
	//	/// <param name="amplitude">Peak deviation from center</param>
	//	/// <param name="frequencyInSeconds">Time in seconds for full cycle</param>
	//	/// <param name="center">Opacity halfway through blink cycle</param>
	//	public A2DBlink(Drawable2D parent, int fadeInTime, int fadeOutTime, float maxOpacity = 1.0f, float minOpacity = 0.0f, int lifetime = 0)
	//		: base(parent, lifetime)
	//	{
	//		this.FadeInLength = fadeInTime;
	//		this.FadeOutLength = fadeOutTime;
	//		this.maxOpacity = maxOpacity;
	//		this.minOpacity = minOpacity;
	//	}

	//	public A2DBlink(Drawable2D parent, params string[] parameters)
	//		: base(parent)
	//	{
	//		if (parameters.Length < 5)
	//		{
	//			throw new ArgumentException("[A2DBlink]: Invalid amount of parameters.");
	//		}

	//		int fadeInTime = Convert.ToInt32(parameters[0]);
	//		int fadeOutTime = Convert.ToInt32(parameters[1]);
	//		float maxOp = (float) Convert.ToDouble(parameters[2]);
	//		float minOp = (float) Convert.ToDouble(parameters[3]);
	//		delayBeforeStart = Convert.ToInt32(parameters[4]);

	//		FadeInLength = fadeInTime;
	//		FadeOutLength = fadeOutTime;
	//		maxOpacity = maxOp;
	//		minOpacity = minOp;
	//	}

	//	public override void OnInitialize(GameTime gameTime)
	//	{
	//		fadeInAction = new A2DFade(this.parentDrawable, maxOpacity, FadeInLength, minOpacity);
	//		fadeInAction.AliveChanged += HandleFadeActionDeath;
	//		fadeOutAction = new A2DFade(this.parentDrawable, minOpacity, FadeOutLength, maxOpacity);
	//		fadeOutAction.AliveChanged += HandleFadeActionDeath;

	//		if (delayBeforeStart > 0)
	//		{
	//			Action wait = new WaitAction(delayBeforeStart);
	//			wait.SetNext(fadeOutAction);
	//			parentDrawable.Holder.Attach(wait);
	//		}
	//		else
	//		{
	//			parentDrawable.Holder.Attach(fadeOutAction);
	//		}
	//		isFadingIn = false;
	//	}

	//	public void Restart()
	//	{
	//		isFadingIn = true;
	//		this.ReverseDirection();
	//	}

	//	private void ReverseDirection()
	//	{

	//		if (isFadingIn)
	//		{
	//			fadeOutAction.Reset(FadeOutLength);
	//			parentDrawable.Holder.Attach(fadeOutAction);
	//		}
	//		else if(this.IsAlive)
	//		{
	//			fadeInAction.Reset(FadeInLength);
	//			if (DelayBetweenBlinks > 0)
	//			{
	//				Action wait = new WaitAction(DelayBetweenBlinks);
	//				wait.SetNext(fadeInAction);
	//				parentDrawable.Holder.Attach(wait);
	//			}
	//			else
	//			{
	//				parentDrawable.Holder.Attach(fadeInAction);
	//			}
	//		}

	//		isFadingIn = !isFadingIn;
	//	}

	//	private void HandleFadeActionDeath(Action action)
	//	{
	//		if (!action.IsAlive)
	//		{
	//			ReverseDirection();
	//		}
	//	}
	//}

	//class A2DHealthMonitor : Behavior2D
	//{
	//	public override string Name { get { return "A2DHealthMonitor"; } }

	//	ICombatant monitoredCombatant;
	//	A2DBlink blinkAction;
		
	//	private float beginBlinkingAt = 0.4f;
	//	private int beginBlinkFadeLifetime = 800;
	//	private int finalBlinkFadeLifetime = 200;

	//	private float stopDelayingBlinkAt = 0.2f;
	//	private int beginDelayBetweenBlinks = 800;
	//	private int finishDelayBetweenBlinks = 20;

	//	bool isSparking;
	//	private const float startSparkingAt = 0.3f;
	//	private const int minFrequency = 200;
	//	private const int startFrequencyOffset = 1200;
	//	private const int endFrequencyOffset = 0;

	//	public A2DHealthMonitor(ICombatant combatant, Drawable2D display)
	//		: base(display)
	//	{
	//		monitoredCombatant = combatant;
	//	}

	//	public override void OnInitialize(GameTime gameTime)
	//	{
	//		blinkAction = new A2DBlink(parentDrawable, beginBlinkFadeLifetime, beginBlinkFadeLifetime);
	//		blinkAction.Initialize(gameTime);
	//		blinkAction.IsAlive = false;
	//		monitoredCombatant.Combat().OnHealthChanged += HandleHealthChange;

	//		OnEveryUpdate += CheckForSparkChange;
	//		EnableStopWatch();
	//		stopWatch.AddNode("Spark", startFrequencyOffset);

	//		KeyValueDisplay.Instance.Add("Sparks: ", () => { return timesSparked as Object; });
	//	}

	//	public override void OnUpdate(GameTime gameTime)
	//	{
			
	//	}

	//	private void CheckForSparkChange(GameTime gameTime)
	//	{
	//		if (!isSparking && startSparkingAt > ServiceLocator.ActivePlayer.Combat().RemainingLifeAsZeroToOneValue)
	//		{
	//			isSparking = true;
	//			ResetSparkTimer();
	//			OnEveryUpdate += UpdateActiveSparking;
	//		}
	//		else if (isSparking && startSparkingAt < ServiceLocator.ActivePlayer.Combat().RemainingLifeAsZeroToOneValue)
	//		{
	//			isSparking = false;
	//			OnEveryUpdate -= UpdateActiveSparking;
	//		}
	//	}

	//	static int timesSparked;

	//	private void UpdateActiveSparking(GameTime gameTime)
	//	{
	//		if (stopWatch["Spark"].HasTicked)
	//		{
	//			int numberOfSparks = Game1.random.Next(1, 4);

	//			int currentWait = 0;
	//			for (int i = 0; i < numberOfSparks; i++)
	//			{
	//				Action a = new WaitAction(currentWait);
	//				a.AliveChanged += action =>
	//					{
	//						if (!action.IsAlive)
	//						{
	//							ParticleComponent comp = ParticleFactory.Instance.RequestParticleSystem(
	//									ParticleFactory.NameSmallSpark, ServiceLocator.ActivePlayer);

	//							comp.ParticleSystem.Emitter.PositionData.Position = GetSparkPosition();
	//						}
	//					};
	//				parentDrawable.Holder.Attach(a);

	//				currentWait += 100;
	//				timesSparked++;
	//			}
				
	//			ResetSparkTimer();
	//		}

	//	}

	//	private Vector3 GetSparkPosition()
	//	{
	//		Vector3 position = ServiceLocator.ActivePlayer.Transform.Position;
	//		float x = Game1.RandomFloat(15, 25);
	//		float y = Game1.RandomFloat(15, 25);
	//		float z = Game1.RandomFloat(15, 25);
	//		if (Game1.random.Next(1, 3) == 1)
	//		{
	//			x *= -1;
	//		}
	//		else if (Game1.random.Next(1, 3) == 1)
	//		{
	//			z *= -1;
	//		}

	//		position += new Vector3(x, y, z);

	//		return position;
	//	}

	//	private void ResetSparkTimer()
	//	{
	//		int frequencyOffset = (int)MathHelper.Lerp(endFrequencyOffset, startFrequencyOffset,
	//				ServiceLocator.ActivePlayer.Combat().RemainingLifeAsZeroToOneValue / startSparkingAt);
	//		stopWatch["Spark"].ChangeLifetime( minFrequency + frequencyOffset);
	//		stopWatch["Spark"].Reset();
	//	}

	//	private void HandleHealthChange(Object sender, EventArgs args)
	//	{
	//		CombatStats combat = sender as CombatStats;
	//		if (combat != null)
	//		{
	//			if (beginBlinkingAt >= combat.RemainingLifeAsZeroToOneValue && blinkAction.IsAlive == false)
	//			{
	//				blinkAction.IsAlive = true;
	//				parentDrawable.Holder.Attach(blinkAction);
	//				blinkAction.Restart();
	//			}
	//			else if (blinkAction.IsAlive == true && beginBlinkingAt < combat.RemainingLifeAsZeroToOneValue)
	//			{
	//				blinkAction.IsAlive = false;
	//				parentDrawable.Transform.Opacity = 0.0f;
	//			}

	//			else if (blinkAction.IsAlive == true)
	//			{
	//				blinkAction.FadeBothLength = (int)MathHelper.Lerp(beginBlinkFadeLifetime, finalBlinkFadeLifetime, 1 - (beginBlinkingAt * combat.RemainingLifeAsZeroToOneValue)); // (1.0f - beginBlinkingAt));
	//				//float f = stopDelayingBlinkAt / beginBlinkingAt;
	//				//if(f < 0.5f)
	//				//{
	//				//    int i = 9;
	//				//    i += 8;
	//				//}
	//				//blinkAction.DelayBetweenBlinks = (int)MathHelper.Lerp(finishDelayBetweenBlinks, beginDelayBetweenBlinks, f);
	//			}
	//		}
	//	}
	//}
}
