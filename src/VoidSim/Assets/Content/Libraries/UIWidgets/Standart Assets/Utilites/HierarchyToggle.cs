﻿using UnityEngine;

namespace UIWidgets
{
	/// <summary>
	/// Move object in hierarchy and keeps original parent, position and relative size.
	/// </summary>
	public class HierarchyToggle : MonoBehaviour
	{
		/// <summary>
		/// Original parent.
		/// </summary>
		protected Transform Parent;
		
		/// <summary>
		/// Original position.
		/// </summary>
		protected Vector2 AnchoredPosition;
		
		/// <summary>
		/// Original size delta.
		/// </summary>
		protected Vector2 SizeDelta;
		
		/// <summary>
		/// Set specified object as new parent.
		/// </summary>
		/// <param name="newParent">New parent.</param>
		public void SetParent(Transform newParent)
		{
			Parent = transform.parent;
			AnchoredPosition = (transform as RectTransform).anchoredPosition;
			SizeDelta = (transform as RectTransform).sizeDelta;

			transform.SetParent(newParent);
		}

		/// <summary>
		/// Set specified object as parent.
		/// </summary>
		/// <param name="newParent">New parent.</param>
		public void SetParent(GameObject newParent)
		{
			SetParent(newParent.transform);
		}

		/// <summary>
		/// Restore original parent, position and relative size.
		/// </summary>
		public void Restore()
		{
			if (Parent==null)
			{
				return ;
			}
			transform.SetParent(Parent);
			
			(transform as RectTransform).anchoredPosition = AnchoredPosition;
			(transform as RectTransform).sizeDelta = SizeDelta;
		}
	}
}