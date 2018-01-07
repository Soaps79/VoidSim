using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System;

namespace UIWidgets
{
	/// <summary>
	/// AccordionDirection.
	/// </summary>
	public enum AccordionDirection
	{
		/// <summary>
		/// Horizontal direction.
		/// </summary>
		Horizontal = 0,

		/// <summary>
		/// Vertical direction.
		/// </summary>
		Vertical = 1,
	}

	/// <summary>
	/// Accordion item resize methods.
	/// </summary>
	public enum ResizeMethods
	{
		/// <summary>
		/// Change width or height.
		/// </summary>
		Size = 0,

		/// <summary>
		/// Change LayoutElement flexibleWidth or flexibleHeight.
		/// </summary>
		Flexible = 1,
	}

	/// <summary>
	/// Accordion.
	/// </summary>
	[AddComponentMenu("UI/UIWidgets/Accordion")]
	public class Accordion : MonoBehaviour
	{
		/// <summary>
		/// Items.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("Items")]
		protected List<AccordionItem> items = new List<AccordionItem>();

		/// <summary>
		/// Items.
		/// </summary>
		protected ObservableList<AccordionItem> dataSource;

		/// <summary>
		/// Accordion items.
		/// </summary>
		/// <value>Accordion items.</value>
		[Obsolete("Use DataSource instead.")]
		public List<AccordionItem> Items {
			get {
				return DataSource.ToList();
			}
			set {
				DataSource = new ObservableList<AccordionItem>(value);
			}
		}

		/// <summary>
		/// Accordion items.
		/// </summary>
		/// <value>Accordion items.</value>
		public virtual ObservableList<AccordionItem> DataSource {
			get {
				if (dataSource==null)
				{
					#pragma warning disable 0618
					dataSource = new ObservableList<AccordionItem>(items);
					dataSource.OnChange += UpdateItems;
					items = null;
					#pragma warning restore 0618
				}
				return dataSource;
			}
			set {
				if (dataSource!=null)
				{
					dataSource.OnChange -= UpdateItems;
				}
				dataSource = value;
				if (dataSource!=null)
				{
					dataSource.OnChange += UpdateItems;
				}
				UpdateItems();
			}
		}
		
		/// <summary>
		/// Only one item can be opened.
		/// </summary>
		[SerializeField]
		public bool OnlyOneOpen = true;
		
		/// <summary>
		/// Animate open and close.
		/// </summary>
		[SerializeField]
		public bool Animate = true;

		/// <summary>
		/// The duration of the animation.
		/// </summary>
		[SerializeField]
		public float AnimationDuration = 0.5f;

		/// <summary>
		/// Use unscaled time.
		/// </summary>
		[SerializeField]
		public bool UnscaledTime = false;

		/// <summary>
		/// The direction.
		/// </summary>
		[SerializeField]
		public AccordionDirection Direction = AccordionDirection.Vertical;

		/// <summary>
		/// The item resize method.
		/// </summary>
		[SerializeField]
		public ResizeMethods ResizeMethod = ResizeMethods.Size;

		/// <summary>
		/// Disable closed items.
		/// </summary>
		[SerializeField]
		public bool DisableClosed = true;

		/// <summary>
		/// OnToggleItem event.
		/// </summary>
		[SerializeField]
		public AccordionEvent OnToggleItem = new AccordionEvent();

		/// <summary>
		/// The components.
		/// </summary>
		protected List<AccordionItemComponent> Components = new List<AccordionItemComponent>();

		/// <summary>
		/// The callbacks.
		/// </summary>
		protected List<UnityAction> Callbacks = new List<UnityAction>();

		/// <summary>
		/// Start this instance.
		/// </summary>
		protected virtual void Start()
		{
			UpdateItems();
		}

		/// <summary>
		/// Updates the items.
		/// </summary>
		protected virtual void UpdateItems()
		{
			RemoveCallbacks();

			AddCallbacks();
			UpdateLayout();
		}

		/// <summary>
		/// Adds the callback.
		/// </summary>
		/// <param name="item">Item.</param>
		protected virtual void AddCallback(AccordionItem item)
		{
			if (item.Open)
			{
				Open(item, false);
			}
			else
			{
				Close(item, false);
			}
			UnityAction callback = () => ToggleItem(item);

			var component = Utilites.GetOrAddComponent<AccordionItemComponent>(item.ToggleObject);
			component.OnClick.AddListener(callback);

			item.ContentObjectRect = item.ContentObject.transform as RectTransform;
			item.ContentLayoutElement = Utilites.GetOrAddComponent<LayoutElement>(item.ContentObject);
			item.ContentObjectHeight = item.ContentObjectRect.rect.height;
			if (item.ContentObjectHeight==0f)
			{
				item.ContentObjectHeight = LayoutUtility.GetPreferredHeight(item.ContentObjectRect);
			}
			item.ContentObjectWidth = item.ContentObjectRect.rect.width;
			if (item.ContentObjectWidth==0f)
			{
				item.ContentObjectWidth = LayoutUtility.GetPreferredWidth(item.ContentObjectRect);
			}

			Components.Add(component);
			Callbacks.Add(callback);
		}

		/// <summary>
		/// Adds the callbacks.
		/// </summary>
		protected virtual void AddCallbacks()
		{
			DataSource.ForEach(AddCallback);
		}

		/// <summary>
		/// Removes the callback.
		/// </summary>
		/// <param name="component">Component.</param>
		/// <param name="index">Index.</param>
		protected virtual void RemoveCallback(AccordionItemComponent component, int index)
		{
			if (component==null)
			{
				return ;
			}

			if (index < Callbacks.Count)
			{
				component.OnClick.RemoveListener(Callbacks[index]);
			}
		}

		/// <summary>
		/// Removes the callbacks.
		/// </summary>
		protected virtual void RemoveCallbacks()
		{
			Components.ForEach(RemoveCallback);
			Components.Clear();
			Callbacks.Clear();
		}

		/// <summary>
		/// This function is called when the MonoBehaviour will be destroyed.
		/// </summary>
		protected virtual void OnDestroy()
		{
			RemoveCallbacks();
		}

		/// <summary>
		/// Toggles the item.
		/// </summary>
		/// <param name="item">Item.</param>
		public virtual void ToggleItem(AccordionItem item)
		{
			if (item.Open)
			{
				if (!OnlyOneOpen)
				{
					Close(item);
				}
			}
			else
			{
				if (OnlyOneOpen)
				{
					DataSource.Where(IsOpen).ForEach(Close);
				}

				Open(item);
			}
		}

		/// <summary>
		/// Open the specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		public virtual void Open(AccordionItem item)
		{
			if (item.Open)
			{
				return ;
			}
			if (OnlyOneOpen)
			{
				DataSource.Where(IsOpen).ForEach(Close);
			}
			Open(item, Animate);
		}

		/// <summary>
		/// Close the specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		public virtual void Close(AccordionItem item)
		{
			if (!item.Open)
			{
				return ;
			}
			Close(item, Animate);
		}

		/// <summary>
		/// Determines whether this instance is open the specified item.
		/// </summary>
		/// <returns><c>true</c> if this instance is open the specified item; otherwise, <c>false</c>.</returns>
		/// <param name="item">Item.</param>
		protected bool IsOpen(AccordionItem item)
		{
			return item.Open;
		}

		/// <summary>
		/// Determines whether this instance is horizontal.
		/// </summary>
		/// <returns><c>true</c> if this instance is horizontal; otherwise, <c>false</c>.</returns>
		protected bool IsHorizontal()
		{
			return Direction==AccordionDirection.Horizontal;
		}

		/// <summary>
		/// Open the specified item and animate.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <param name="animate">If set to <c>true</c> animate.</param>
		protected virtual void Open(AccordionItem item, bool animate)
		{
			if (item.CurrentCoroutine != null)
			{
				StopCoroutine(item.CurrentCoroutine);

				if (IsHorizontal())
				{
					item.ContentObjectRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, item.ContentObjectWidth);
					if (ResizeMethod == ResizeMethods.Size)
					{
						item.ContentLayoutElement.preferredWidth = -1f;
					}
					else if (ResizeMethod == ResizeMethods.Flexible)
					{
						item.ContentLayoutElement.flexibleWidth = 1f;
					}
				}
				else
				{
					item.ContentObjectRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, item.ContentObjectHeight);
					if (ResizeMethod == ResizeMethods.Size)
					{
						item.ContentLayoutElement.preferredHeight = -1f;
					}
					else if (ResizeMethod == ResizeMethods.Flexible)
					{
						item.ContentLayoutElement.flexibleHeight = 1f;
					}
				}
				item.ContentObject.SetActive(false);
			}

			if (animate)
			{
				item.CurrentCoroutine = StartCoroutine(OpenCoroutine(item));
			}
			else
			{
				item.ContentObject.SetActive(true);
				OnToggleItem.Invoke(item);
			}

			item.ContentObject.SetActive(true);
			item.Open = true;
		}

		/// <summary>
		/// Close the specified item and animate.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <param name="animate">If set to <c>true</c> animate.</param>
		protected virtual void Close(AccordionItem item, bool animate)
		{
			if (item.CurrentCoroutine != null)
			{
				StopCoroutine(item.CurrentCoroutine);

				item.ContentObject.SetActive(true);
				if (IsHorizontal())
				{
					if (item.ContentObjectRect != null)
					{
						item.ContentObjectRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, item.ContentObjectWidth);
					}
					if (ResizeMethod == ResizeMethods.Size)
					{
						item.ContentLayoutElement.preferredWidth = item.ContentObjectWidth;
					}
					else if (ResizeMethod == ResizeMethods.Flexible)
					{
						item.ContentLayoutElement.flexibleWidth = 0f;
					}
				}
				else
				{
					if (item.ContentObjectRect != null)
					{
						item.ContentObjectRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, item.ContentObjectHeight);
					}
					if (ResizeMethod == ResizeMethods.Size)
					{
						item.ContentLayoutElement.preferredHeight = item.ContentObjectHeight;
					}
					else if (ResizeMethod == ResizeMethods.Flexible)
					{
						item.ContentLayoutElement.flexibleHeight = 0f;
					}
				}
			}

			if (item.ContentObjectRect!=null)
			{
				item.ContentObjectHeight = item.ContentObjectRect.rect.height;
				item.ContentObjectWidth = item.ContentObjectRect.rect.width;
			}

			if (animate)
			{
				item.CurrentCoroutine = StartCoroutine(HideCoroutine(item));
			}
			else
			{
				item.ContentObject.SetActive(false);
				item.Open = false;
				OnToggleItem.Invoke(item);
			}

		}

		/// <summary>
		/// Open coroutine.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <returns>The coroutine.</returns>
		protected virtual IEnumerator OpenCoroutine(AccordionItem item)
		{
			item.ContentObject.SetActive(true);
			item.Open = true;

			if (ResizeMethod==ResizeMethods.Size)
			{
				yield return StartCoroutine(Animations.Open(item.ContentObjectRect, AnimationDuration, IsHorizontal(), UnscaledTime, () => {
					if (IsHorizontal())
					{
						item.ContentLayoutElement.preferredWidth = -1;
					}
					else
					{
						item.ContentLayoutElement.preferredHeight = -1;
					}
					LayoutUtilites.UpdateLayout(item.ContentObjectRect.parent.GetComponent<LayoutGroup>());
					item.ContentObjectWidth = item.ContentObjectRect.rect.width;
					item.ContentObjectHeight = item.ContentObjectRect.rect.height;
				}));
			}
			else if (ResizeMethod==ResizeMethods.Flexible)
			{
				yield return StartCoroutine(Animations.OpenFlexible(item.ContentObjectRect, AnimationDuration, IsHorizontal()));
			}

			UpdateLayout();

			OnToggleItem.Invoke(item);
		}

		/// <summary>
		/// Updates the layout.
		/// </summary>
		protected void UpdateLayout()
		{
			LayoutUtilites.UpdateLayout(GetComponent<LayoutGroup>());
		}

		/// <summary>
		/// Hide coroutine.
		/// </summary>
		/// <returns>The coroutine.</returns>
		/// <param name="item">Item.</param>
		protected virtual IEnumerator HideCoroutine(AccordionItem item)
		{
			if (ResizeMethod==ResizeMethods.Size)
			{
					yield return StartCoroutine(Animations.Collapse(item.ContentObjectRect, AnimationDuration, IsHorizontal(), UnscaledTime, () => {
					if (IsHorizontal())
					{
						item.ContentLayoutElement.preferredWidth = -1;
					}
					else
					{
						item.ContentLayoutElement.preferredHeight = -1;
					}
				}));
			}
			else if (ResizeMethod==ResizeMethods.Flexible)
			{
				yield return StartCoroutine(Animations.CollapseFlexible(item.ContentObjectRect, AnimationDuration, IsHorizontal()));
			}

			item.Open = false;
			if (DisableClosed)
			{
				item.ContentObject.SetActive(false);
			}
			else
			{
				var axis = IsHorizontal() ? RectTransform.Axis.Horizontal : RectTransform.Axis.Vertical;
				item.ContentObjectRect.SetSizeWithCurrentAnchors(axis, 0f);
			}

			UpdateLayout();

			OnToggleItem.Invoke(item);
		}

		/// <summary>
		/// Close all opened items.
		/// </summary>
		public virtual void CloseAll()
		{
			DataSource.ForEach(Close);
		}

		#if UNITY_EDITOR
		/// <summary>
		/// Disable closed items in Editor if DisableClosed enabled.
		/// </summary>
		protected virtual void OnValidate()
		{
			if (!DisableClosed)
			{
				return ;
			}
			if (items!=null)
			{
				items.ForEach(x => x.ContentObject.SetActive(x.Open));
			}
		}
		#endif
	}
}