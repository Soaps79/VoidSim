using UnityEngine;
using System.Collections.Generic;

namespace UIWidgets
{
	/// <summary>
	/// Helper for ObjectSliding.
	/// Set allowed positions to current, current + width(ObjectsOnLeft), current - width(ObjectsOnRight)
	/// </summary>
	[RequireComponent(typeof(ObjectSliding))]
	public class ObjectSlidingHorizontalHelper : MonoBehaviour
	{
		/// <summary>
		/// Objects on left.
		/// </summary>
		[SerializeField]
		protected List<RectTransform> ObjectsOnLeft = new List<RectTransform>();

		/// <summary>
		/// Objects on right.
		/// </summary>
		[SerializeField]
		protected List<RectTransform> ObjectsOnRight = new List<RectTransform>();

		/// <summary>
		/// Current ObjectSliding.
		/// </summary>
		[HideInInspector]
		protected ObjectSliding Sliding;

		/// <summary>
		/// Start this instance.
		/// </summary>
		public virtual void Start()
		{
			Init();
		}

		bool isInited;

		/// <summary>
		/// Add listeners and set positions.
		/// </summary>
		public virtual void Init()
		{
			if (isInited)
			{
				return ;
			}
			isInited = true;

			Sliding = GetComponent<ObjectSliding>();

			Sliding.Direction = ObjectSlidingDirection.Horizontal;

			AddListeners();

			CalculatePositions();
		}

		/// <summary>
		/// Add listener.
		/// </summary>
		/// <param name="rect">RectTransform</param>
		protected virtual void AddListener(RectTransform rect)
		{
			var rl = Utilites.GetOrAddComponent<ResizeListener>(rect);
			rl.OnResize.AddListener(CalculatePositions);
		}

		/// <summary>
		/// Add listeners.
		/// </summary>
		protected virtual void AddListeners()
		{
			ObjectsOnLeft.ForEach(AddListener);
			ObjectsOnRight.ForEach(AddListener);
		}

		/// <summary>
		/// Remove listener.
		/// </summary>
		/// <param name="rect">RectTransform.</param>
		protected virtual void RemoveListener(RectTransform rect)
		{
			var rl = rect.GetComponent<ResizeListener>();
			if (rl != null)
			{
				rl.OnResize.RemoveListener(CalculatePositions);
			}
		}

		/// <summary>
		/// Remove listener.
		/// </summary>
		protected virtual void RemoveListeners()
		{
			ObjectsOnLeft.ForEach(RemoveListener);
			ObjectsOnRight.ForEach(RemoveListener);
		}

		/// <summary>
		/// Calculate positions.
		/// </summary>
		protected virtual void CalculatePositions()
		{
			var pos = (Sliding.transform as RectTransform).anchoredPosition.x;
			var left = pos + ObjectsOnLeft.SumFloat(x => x.rect.width);
			var right = pos - ObjectsOnRight.SumFloat(x => x.rect.width);

			Sliding.Positions.Clear();
			Sliding.Positions.Add(pos);
			Sliding.Positions.Add(left);
			Sliding.Positions.Add(right);
		}

		/// <summary>
		/// Remove listeners on destroy.
		/// </summary>
		protected virtual void OnDestroy()
		{
			RemoveListeners();
		}
	}
}