using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections.Generic;

namespace EasyLayout
{
	/// <summary>
	/// LayoutElementInfo.
	/// Correctly works with multiple resizes during one frame.
	/// </summary>
	public class LayoutElementInfo
	{
		/// <summary>
		/// RectTransform.
		/// </summary>
		public RectTransform Rect {
			get;
			protected set;
		}

		/// <summary>
		/// Width.
		/// </summary>
		public float Width {
			get;
			protected set;
		}

		/// <summary>
		/// Height.
		/// </summary>
		public float Height {
			get;
			protected set;
		}

		/// <summary>
		/// Size.
		/// </summary>
		public float Size {
			get {
				return Layout.Stacking==Stackings.Horizontal ? Width : Height;
			}
		}

		/// <summary>
		/// Minimal width.
		/// </summary>
		public float MinWidth {
			get;
			protected set;
		}

		/// <summary>
		/// Minimal height.
		/// </summary>
		public float MinHeight {
			get;
			protected set;
		}

		/// <summary>
		/// Preferred width.
		/// </summary>
		public float PreferredWidth {
			get;
			protected set;
		}

		/// <summary>
		/// Preferred height.
		/// </summary>
		public float PreferredHeight {
			get;
			protected set;
		}

		/// <summary>
		/// Flexible width.
		/// </summary>
		public float FlexibleWidth {
			get;
			protected set;
		}

		/// <summary>
		/// Flexible height.
		/// </summary>
		public float FlexibleHeight {
			get;
			protected set;
		}

		/// <summary>
		/// Resizer.
		/// </summary>
		protected EasyLayoutResizer Resizer;

		/// <summary>
		/// Current layout.
		/// </summary>
		protected EasyLayout Layout;

		/// <summary>
		/// Scale.
		/// </summary>
		protected Vector3 Scale;

		/// <summary>
		/// Is width changed?
		/// </summary>
		protected bool ChangedWidth;

		/// <summary>
		/// Is height changed?
		/// </summary>
		protected bool ChangedHeight;

		/// <summary>
		/// New width.
		/// </summary>
		protected float newWidth;

		/// <summary>
		/// New width.
		/// </summary>
		public float NewWidth {
			get {
				return newWidth;
			}
			set {
				newWidth = value;
				Width = value * Scale.x;
				ChangedWidth = true;
			}
		}

		/// <summary>
		/// New height.
		/// </summary>
		protected float newHeight;

		/// <summary>
		/// New height.
		/// </summary>
		public float NewHeight {
			get {
				return newHeight;
			}
			set {
				newHeight = value;
				Height = value * Scale.y;
				ChangedHeight = true;
			}
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="rectTransform">RectTransform.</param>
		/// <param name="resizer">Resizer.</param>
		/// <param name="layout">Current layout.</param>
		public LayoutElementInfo(RectTransform rectTransform, EasyLayoutResizer resizer, EasyLayout layout)
		{
			Rect = rectTransform;
			Resizer = resizer;
			Layout = layout;

			Scale = rectTransform.localScale;
			Width = rectTransform.rect.width * Scale.x;
			Height = rectTransform.rect.height * Scale.y;

			if (Layout.ChildrenWidth!=ChildrenSize.DoNothing)
			{
				MinWidth = GetMinWidth(rectTransform);
				PreferredWidth = GetPreferredWidth(rectTransform);
				FlexibleWidth = GetFlexibleWidth(rectTransform);
			}

			if (Layout.ChildrenHeight!=ChildrenSize.DoNothing)
			{
				MinHeight = GetMinHeight(rectTransform);
				PreferredHeight = GetPreferredHeight(rectTransform);
				FlexibleHeight = GetFlexibleHeight(rectTransform);
			}
		}

		/// <summary>
		/// Set size.
		/// </summary>
		/// <param name="axis">Axis.</param>
		/// <param name="size">New size.</param>
		public void SetSize(RectTransform.Axis axis, float size)
		{
			if (axis==RectTransform.Axis.Horizontal)
			{
				NewWidth = size;
			}
			else
			{
				NewHeight = size;
			}
		}

		/// <summary>
		/// Actual resize.
		/// </summary>
		public void ApplyResize()
		{
			DrivenTransformProperties driven_properties = DrivenTransformProperties.AnchoredPosition | DrivenTransformProperties.AnchoredPositionZ;

			if (ChangedWidth)
			{
				driven_properties |= DrivenTransformProperties.SizeDeltaX;
				Rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, NewWidth);
			}
			if (ChangedHeight)
			{
				driven_properties |= DrivenTransformProperties.SizeDeltaY;
				Rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, NewHeight);
			}

			if (ChangedWidth || ChangedHeight)
			{
				Resizer.PropertiesTracker.Add(Layout, Rect, driven_properties);
			}
		}

		/// <summary>
		/// Gets the preferred width of the RectTransform.
		/// </summary>
		/// <returns>The preferred width.</returns>
		/// <param name="rect">Rect.</param>
		static float GetPreferredWidth(RectTransform rect)
		{
			if (rect==null)
			{
				return 0f;
			}
			if (rect.gameObject.activeInHierarchy)
			{
				return Mathf.Max(0f, LayoutUtility.GetPreferredWidth(rect));
			}
			else
			{
				return GetLayoutParameter(rect, x => Mathf.Max(x.preferredWidth, x.minWidth));
			}
		}

		/// <summary>
		/// Gets the min width of the RectTransform.
		/// </summary>
		/// <returns>The preferred width.</returns>
		/// <param name="rect">Rect.</param>
		static float GetMinWidth(RectTransform rect)
		{
			if (rect==null)
			{
				return 0f;
			}
			if (rect.gameObject.activeInHierarchy)
			{
				return Mathf.Max(0f, LayoutUtility.GetMinWidth(rect));
			}
			else
			{
				return GetLayoutParameter(rect, x => x.minWidth);
			}
		}

		/// <summary>
		/// Gets the preferred height of the RectTransform.
		/// </summary>
		/// <returns>The preferred height.</returns>
		/// <param name="rect">Rect.</param>
		static float GetPreferredHeight(RectTransform rect)
		{
			if (rect==null)
			{
				return 0f;
			}
			if (rect.gameObject.activeInHierarchy)
			{
				return Mathf.Max(0f, LayoutUtility.GetPreferredHeight(rect));
			}
			else
			{
				return GetLayoutParameter(rect, x => Mathf.Max(x.preferredHeight, x.minHeight));
			}
		}

		/// <summary>
		/// Gets the min height of the RectTransform.
		/// </summary>
		/// <returns>The min height.</returns>
		/// <param name="rect">Rect.</param>
		static float GetMinHeight(RectTransform rect)
		{
			if (rect==null)
			{
				return 0f;
			}
			if (rect.gameObject.activeInHierarchy)
			{
				return Mathf.Max(0f, LayoutUtility.GetMinHeight(rect));
			}
			else
			{
				return GetLayoutParameter(rect, x => x.minHeight);
			}
		}

		/// <summary>
		/// Gets the flexible width of the RectTransform.
		/// </summary>
		/// <returns>The flexible width.</returns>
		/// <param name="rect">Rect.</param>
		static float GetFlexibleWidth(RectTransform rect)
		{
			if (rect==null)
			{
				return 0f;
			}
			if (rect.gameObject.activeInHierarchy)
			{
				return Mathf.Max(0f, LayoutUtility.GetFlexibleWidth(rect));
			}
			else
			{
				return GetLayoutParameter(rect, x => x.flexibleWidth);
			}
		}

		/// <summary>
		/// Gets the flexible height of the RectTransform.
		/// </summary>
		/// <returns>The flexible height.</returns>
		/// <param name="rect">Rect.</param>
		static float GetFlexibleHeight(RectTransform rect)
		{
			if (rect==null)
			{
				return 0f;
			}
			if (rect.gameObject.activeInHierarchy)
			{
				return Mathf.Max(0f, LayoutUtility.GetFlexibleHeight(rect));
			}
			else
			{
				return GetLayoutParameter(rect, x => x.flexibleHeight);
			}
		}

		static float GetLayoutParameter(RectTransform rect, Func<ILayoutElement,float> func)
		{
			float result = 0f;
			#if UNITY_4_6 || UNITY_4_7
			var elements = rect.GetComponents<Component>().OfType<ILayoutElement>();
			#else
			var elements = rect.GetComponents<ILayoutElement>();
			#endif
			var max_priority = elements.Max(x => x.layoutPriority);
			foreach (var elem in elements)
			{
				if (elem.layoutPriority==max_priority)
				{
					result = Mathf.Max(result, func(elem));
				}
			}
			return result;
		}

	}
}

