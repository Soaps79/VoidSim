using UnityEngine;
using System.Collections.Generic;

namespace UIWidgets
{
	/// <summary>
	/// Helper for ObjectSliding.
	/// Set allowed positions to current, current + height(ObjectsOnLeft), current - height(ObjectsOnRight)
	/// </summary>
	[RequireComponent(typeof(ObjectSliding))]
	public class ObjectSlidingVerticalHelper : MonoBehaviour
	{
		/// <summary>
		/// Objects on top.
		/// </summary>
		[SerializeField]
		protected List<RectTransform> ObjectsOnTop = new List<RectTransform>();

		/// <summary>
		/// Objects on bottom.
		/// </summary>
		[SerializeField]
		protected List<RectTransform> ObjectsOnBottom = new List<RectTransform>();

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
		/// Add listeners and calculate positions.
		/// </summary>
		public virtual void Init()
		{
			if (isInited)
			{
				return ;
			}
			isInited = true;

			Sliding = GetComponent<ObjectSliding>();

			Sliding.Direction = ObjectSlidingDirection.Vertical;

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
			ObjectsOnTop.ForEach(AddListener);
			ObjectsOnBottom.ForEach(AddListener);
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
			ObjectsOnTop.ForEach(RemoveListener);
			ObjectsOnBottom.ForEach(RemoveListener);
		}

		/// <summary>
		/// Calculate positions.
		/// </summary>
		protected virtual void CalculatePositions()
		{
			var pos = (Sliding.transform as RectTransform).anchoredPosition.y;
			var top = pos - ObjectsOnTop.SumFloat(x => x.rect.height);
			var bottom = pos + ObjectsOnBottom.SumFloat(x => x.rect.height);

			Sliding.Positions.Clear();
			Sliding.Positions.Add(pos);
			Sliding.Positions.Add(top);
			Sliding.Positions.Add(bottom);
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