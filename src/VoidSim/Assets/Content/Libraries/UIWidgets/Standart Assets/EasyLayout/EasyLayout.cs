using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;
using System;
using System.ComponentModel;

namespace EasyLayout
{
	/// <summary>
	/// Grid constraints.
	/// </summary>
	public enum GridConstraints
	{
		/// <summary>
		/// Don't constraint the number of rows or columns.
		/// </summary>
		Flexible = 0,

		/// <summary>
		/// Constraint the number of columns to a specified number.
		/// </summary>
		FixedColumnCount = 1,

		/// <summary>
		/// Constraint the number of rows to a specified number.
		/// </summary>
		FixedRowCount = 2,
	}

	/// <summary>
	/// Compact constraints.
	/// </summary>
	public enum CompactConstraints
	{
		/// <summary>
		/// Don't constraint the number of rows or columns.
		/// </summary>
		Flexible = 0,

		/// <summary>
		/// Constraint the number of columns to a specified number.
		/// </summary>
		MaxColumnCount = 1,

		/// <summary>
		/// Constraint the number of rows to a specified number.
		/// </summary>
		MaxRowCount = 2,
	}

	/// <summary>
	/// Padding.
	/// </summary>
	[Serializable]
	public struct Padding
	{
		/// <summary>
		/// The left padding.
		/// </summary>
		[SerializeField]
		public float Left;

		/// <summary>
		/// The right padding.
		/// </summary>
		[SerializeField]
		public float Right;

		/// <summary>
		/// The top padding.
		/// </summary>
		[SerializeField]
		public float Top;

		/// <summary>
		/// The bottom padding.
		/// </summary>
		[SerializeField]
		public float Bottom;

		/// <summary>
		/// Initializes a new instance of the struct.
		/// </summary>
		/// <param name="left">Left.</param>
		/// <param name="right">Right.</param>
		/// <param name="top">Top.</param>
		/// <param name="bottom">Bottom.</param>
		public Padding(float left, float right, float top, float bottom)
		{
			Left = left;
			Right = right;
			Top = top;
			Bottom = bottom;
		}

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>A string that represents the current object.</returns>
		public override string ToString()
		{
			return String.Format("Padding(left: {0}, right: {1}, top: {2}, bottom: {3})",
				Left,
				Right,
				Top,
				Bottom
			);
		}

		/// <summary>
		/// Determines whether the specified object is equal to the current object.
		/// </summary>
		/// <param name="obj">The object to compare with the current object.</param>
		/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
		public override bool Equals(object obj)
		{
			if (obj is Padding)
			{
				return Equals((Padding)obj);
			}
			return false;
		}

		/// <summary>
		/// Determines whether the specified object is equal to the current object.
		/// </summary>
		/// <param name="other">The object to compare with the current object.</param>
		/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
		public bool Equals(Padding other)
		{
			return Left == other.Left && Right == other.Right && Top == other.Top && Bottom == other.Bottom;
		}

		/// <summary>
		/// Hash function.
		/// </summary>
		/// <returns>A hash code for the current object.</returns>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		/// <summary>
		/// Compare specified paddings.
		/// </summary>
		/// <param name="padding1">First padding.</param>
		/// <param name="padding2">Second padding.</param>
		/// <returns>true if the paddings are equal; otherwise, false.</returns>
		public static bool operator ==(Padding padding1, Padding padding2)
		{
			return Equals(padding1, padding2);
		}

		/// <summary>
		/// Compare specified paddings.
		/// </summary>
		/// <param name="padding1">First padding.</param>
		/// <param name="padding2">Second padding.</param>
		/// <returns>true if the paddings not equal; otherwise, false.</returns>
		public static bool operator !=(Padding padding1, Padding padding2)
		{
			return !Equals(padding1, padding2);
		}
	}

	/// <summary>
	/// Children size.
	/// </summary>
	[Flags]
	public enum ChildrenSize
	{
		/// <summary>
		/// Don't change children sizes.
		/// </summary>
		DoNothing = 0,

		/// <summary>
		/// Set children sizes to preferred.
		/// </summary>
		SetPreferred = 1,

		/// <summary>
		/// Set children size to maximum size of children preferred.
		/// </summary>
		SetMaxFromPreferred = 2,

		/// <summary>
		/// Stretch children size to fit container.
		/// </summary>
		FitContainer = 3,

		/// <summary>
		/// Shrink children size if UI size more than RectTransform size.
		/// </summary>
		ShrinkOnOverflow = 4,
	}

	/// <summary>
	/// Anchors.
	/// </summary>
	[Flags]
	public enum Anchors
	{
		/// <summary>
		/// UpperLeft.
		/// </summary>
		UpperLeft = 0,

		/// <summary>
		/// UpperCenter.
		/// </summary>
		UpperCenter = 1,

		/// <summary>
		/// UpperRight.
		/// </summary>
		UpperRight = 2,

		/// <summary>
		/// MiddleLeft.
		/// </summary>
		MiddleLeft = 3,

		/// <summary>
		/// MiddleCenter.
		/// </summary>
		MiddleCenter = 4,

		/// <summary>
		/// MiddleRight.
		/// </summary>
		MiddleRight = 5,

		/// <summary>
		/// LowerLeft.
		/// </summary>
		LowerLeft = 6,

		/// <summary>
		/// LowerCenter.
		/// </summary>
		LowerCenter = 7,

		/// <summary>
		/// LowerRight.
		/// </summary>
		LowerRight = 8,
	}
	
	/// <summary>
	/// Stackings.
	/// </summary>
	[Flags]
	public enum Stackings
	{
		/// <summary>
		/// Horizontal.
		/// </summary>
		Horizontal = 0,

		/// <summary>
		/// Vertical.
		/// </summary>
		Vertical = 1,
	}

	/// <summary>
	/// Horizontal aligns.
	/// </summary>
	[Flags]
	public enum HorizontalAligns
	{
		/// <summary>
		/// Left.
		/// </summary>
		Left = 0,

		/// <summary>
		/// Center.
		/// </summary>
		Center = 1,

		/// <summary>
		/// Right.
		/// </summary>
		Right = 2,
	}

	/// <summary>
	/// Inner aligns.
	/// </summary>
	[Flags]
	public enum InnerAligns
	{
		/// <summary>
		/// Top.
		/// </summary>
		Top = 0,

		/// <summary>
		/// Middle.
		/// </summary>
		Middle = 1,

		/// <summary>
		/// Bottom.
		/// </summary>
		Bottom = 2,
	}

	/// <summary>
	/// Layout type to use.
	/// </summary>
	[Flags]
	public enum LayoutTypes
	{
		/// <summary>
		/// Compact.
		/// </summary>
		Compact = 0,

		/// <summary>
		/// Grid.
		/// </summary>
		Grid = 1,
	}


	/// <summary>
	/// EasyLayout.
	/// Warning: using RectTransform relative size with positive size delta (like 100% + 10) with ContentSizeFitter can lead to infinite increased size.
	/// </summary>
	[ExecuteInEditMode]
	[RequireComponent(typeof(RectTransform))]
	[AddComponentMenu("UI/UIWidgets/EasyLayout")]
	public class EasyLayout : UnityEngine.UI.LayoutGroup, INotifyPropertyChanged
	{
		/// <summary>
		/// Occurs when a property value changes.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged = (x, y) => { };

		/// <summary>
		/// Occurs when a property value changes.
		/// </summary>
		[SerializeField]
		public UnityEvent SettingsChanged = new UnityEvent();

		[SerializeField]
		[FormerlySerializedAs("GroupPosition")]
		Anchors groupPosition = Anchors.UpperLeft;

		/// <summary>
		/// The group position.
		/// </summary>
		public Anchors GroupPosition {
			get {
				return groupPosition;
			}
			set {
				groupPosition = value;
				Changed("GroupPosition");
			}
		}

		[SerializeField]
		[FormerlySerializedAs("Stacking")]
		Stackings stacking = Stackings.Horizontal;

		/// <summary>
		/// The stacking type.
		/// </summary>
		public Stackings Stacking {
			get {
				return stacking;
			}
			set {
				stacking = value;
				Changed("Stacking");
			}
		}

		[SerializeField]
		[FormerlySerializedAs("LayoutType")]
		LayoutTypes layoutType = LayoutTypes.Compact;

		/// <summary>
		/// The type of the layout.
		/// </summary>
		public LayoutTypes LayoutType {
			get {
				return layoutType;
			}
			set {
				layoutType = value;
				Changed("LayoutType");
			}
		}

		[SerializeField]
		[FormerlySerializedAs("CompactConstraint")]
		CompactConstraints compactConstraint = CompactConstraints.Flexible;

		/// <summary>
		/// Which constraint to use for the Grid layout.
		/// </summary>
		public CompactConstraints CompactConstraint {
			get {
				return compactConstraint;
			}
			set {
				compactConstraint = value;
				Changed("CompactConstraint");
			}
		}

		[SerializeField]
		[FormerlySerializedAs("CompactConstraintCount")]
		int compactConstraintCount = 1;

		/// <summary>
		/// How many elements there should be along the constrained axis.
		/// </summary>
		public int CompactConstraintCount {
			get {
				return Mathf.Max(1, compactConstraintCount);
			}
			set {
				compactConstraintCount = value;
				Changed("CompactConstraintCount");
			}
		}

		[SerializeField]
		[FormerlySerializedAs("GridConstraint")]
		GridConstraints gridConstraint = GridConstraints.Flexible;

		/// <summary>
		/// Which constraint to use for the Grid layout.
		/// </summary>
		public GridConstraints GridConstraint {
			get {
				return gridConstraint;
			}
			set {
				gridConstraint = value;
				Changed("GridConstraint");
			}
		}

		[SerializeField]
		[FormerlySerializedAs("GridConstraintCount")]
		int gridConstraintCount = 1;

		/// <summary>
		/// How many cells there should be along the constrained axis.
		/// </summary>
		public int GridConstraintCount {
			get {
				return Mathf.Max(1, gridConstraintCount);
			}
			set {
				gridConstraintCount = value;
				Changed("GridConstraintCount");
			}
		}

		/// <summary>
		/// Constraint count.
		/// </summary>
		public int ConstraintCount {
			get {
				if (LayoutType==LayoutTypes.Compact)
				{
					return CompactConstraintCount;
				}
				else
				{
					return GridConstraintCount;
				}
			}
		}

		[SerializeField]
		[FormerlySerializedAs("RowAlign")]
		HorizontalAligns rowAlign = HorizontalAligns.Left;

		/// <summary>
		/// The row align.
		/// </summary>
		public HorizontalAligns RowAlign {
			get {
				return rowAlign;
			}
			set {
				rowAlign = value;
				Changed("RowAlign");
			}
		}

		[SerializeField]
		[FormerlySerializedAs("InnerAlign")]
		InnerAligns innerAlign = InnerAligns.Top;

		/// <summary>
		/// The inner align.
		/// </summary>
		public InnerAligns InnerAlign {
			get {
				return innerAlign;
			}
			set {
				innerAlign = value;
				Changed("InnerAlign");
			}
		}

		[SerializeField]
		[FormerlySerializedAs("CellAlign")]
		Anchors cellAlign = Anchors.UpperLeft;

		/// <summary>
		/// The cell align.
		/// </summary>
		public Anchors CellAlign {
			get {
				return cellAlign;
			}
			set {
				cellAlign = value;
				Changed("CellAlign");
			}
		}

		[SerializeField]
		[FormerlySerializedAs("Spacing")]
		Vector2 spacing = new Vector2(5, 5);

		/// <summary>
		/// The spacing.
		/// </summary>
		public Vector2 Spacing {
			get {
				return spacing;
			}
			set {
				spacing = value;
				Changed("Spacing");
			}
		}

		[SerializeField]
		[FormerlySerializedAs("Symmetric")]
		bool symmetric = true;

		/// <summary>
		/// Symmetric margin.
		/// </summary>
		public bool Symmetric {
			get {
				return symmetric;
			}
			set {
				symmetric = value;
				Changed("Symmetric");
			}
		}

		[SerializeField]
		[FormerlySerializedAs("Margin")]
		Vector2 margin = new Vector2(5, 5);

		/// <summary>
		/// The margin.
		/// </summary>
		public Vector2 Margin {
			get {
				return margin;
			}
			set {
				margin = value;
				Changed("Margin");
			}
		}

		[SerializeField]
		[FormerlySerializedAs("PaddingInner")]
		Padding paddingInner = new Padding();

		/// <summary>
		/// The padding.
		/// </summary>
		public Padding PaddingInner {
			get {
				return paddingInner;
			}
			set {
				paddingInner = value;
				Changed("PaddingInner");
			}
		}

		[SerializeField]
		[FormerlySerializedAs("MarginTop")]
		float marginTop = 5f;

		/// <summary>
		/// The margin top.
		/// </summary>
		public float MarginTop {
			get {
				return marginTop;
			}
			set {
				marginTop = value;
				Changed("MarginTop");
			}
		}

		[SerializeField]
		[FormerlySerializedAs("MarginBottom")]
		float marginBottom = 5f;

		/// <summary>
		/// The margin bottom.
		/// </summary>
		public float MarginBottom {
			get {
				return marginBottom;
			}
			set {
				marginBottom = value;
				Changed("MarginBottom");
			}
		}

		[SerializeField]
		[FormerlySerializedAs("MarginLeft")]
		float marginLeft = 5f;

		/// <summary>
		/// The margin left.
		/// </summary>
		public float MarginLeft {
			get {
				return marginLeft;
			}
			set {
				marginLeft = value;
				Changed("MarginLeft");
			}
		}

		[SerializeField]
		[FormerlySerializedAs("MarginRight")]
		float marginRight = 5f;

		/// <summary>
		/// The margin right.
		/// </summary>
		public float MarginRight {
			get {
				return marginRight;
			}
			set {
				marginRight = value;
				Changed("MarginRight");
			}
		}

		[SerializeField]
		[FormerlySerializedAs("RightToLeft")]
		bool rightToLeft = false;

		/// <summary>
		/// The right to left stacking.
		/// </summary>
		public bool RightToLeft {
			get {
				return rightToLeft;
			}
			set {
				rightToLeft = value;
				Changed("RightToLeft");
			}
		}

		[SerializeField]
		[FormerlySerializedAs("TopToBottom")]
		bool topToBottom = true;

		/// <summary>
		/// The top to bottom stacking.
		/// </summary>
		public bool TopToBottom {
			get {
				return topToBottom;
			}
			set {
				topToBottom = value;
				Changed("TopToBottom");
			}
		}

		[SerializeField]
		[FormerlySerializedAs("SkipInactive")]
		bool skipInactive = true;

		/// <summary>
		/// The skip inactive.
		/// </summary>
		public bool SkipInactive {
			get {
				return skipInactive;
			}
			set {
				skipInactive = value;
				Changed("SkipInactive");
			}
		}

		Func<IEnumerable<GameObject>,IEnumerable<GameObject>> filter = null;

		/// <summary>
		/// The filter.
		/// </summary>
		public Func<IEnumerable<GameObject>,IEnumerable<GameObject>> Filter {
			get {
				return filter;
			}
			set {
				filter = value;
				Changed("Filter");
			}
		}

		[SerializeField]
		[FormerlySerializedAs("ChildrenWidth")]
		ChildrenSize childrenWidth;

		/// <summary>
		/// How to control width of the children.
		/// </summary>
		public ChildrenSize ChildrenWidth {
			get {
				return childrenWidth;
			}
			set {
				childrenWidth = value;
				Changed("ChildrenWidth");
			}
		}

		[SerializeField]
		[FormerlySerializedAs("ChildrenHeight")]
		ChildrenSize childrenHeight;

		/// <summary>
		/// How to control height of the children.
		/// </summary>
		public ChildrenSize ChildrenHeight {
			get {
				return childrenHeight;
			}
			set {
				childrenHeight = value;
				Changed("ChildrenHeight");
			}
		}

		/// <summary>
		/// Control width of children.
		/// </summary>
		[SerializeField]
		[Obsolete("Use ChildrenWidth with ChildrenSize.SetPreferred instead.")]
		public bool ControlWidth;

		/// <summary>
		/// Control height of children.
		/// </summary>
		[SerializeField]
		[Obsolete("Use ChildrenHeight with ChildrenSize.SetPreferred instead.")]
		[FormerlySerializedAs("ControlHeight")]
		public bool ControlHeight;

		/// <summary>
		/// Sets width of the chidren to maximum width from them.
		/// </summary>
		[SerializeField]
		[Obsolete("Use ChildrenWidth with ChildrenSize.SetMaxFromPreferred instead.")]
		[FormerlySerializedAs("MaxWidth")]
		public bool MaxWidth;
		
		/// <summary>
		/// Sets height of the chidren to maximum height from them.
		/// </summary>
		[SerializeField]
		[Obsolete("Use ChildrenHeight with ChildrenSize.SetMaxFromPreferred instead.")]
		[FormerlySerializedAs("MaxHeight")]
		public bool MaxHeight;

		Vector2 _blockSize;

		/// <summary>
		/// Gets or sets the size of the inner block.
		/// </summary>
		/// <value>The size of the inner block.</value>
		public Vector2 BlockSize {
			get {
				return _blockSize;
			}
			protected set {
				_blockSize = value;
			}
		}

		Vector2 _uiSize;
		/// <summary>
		/// Gets or sets the UI size.
		/// </summary>
		/// <value>The UI size.</value>
		public Vector2 UISize {
			get {
				return _uiSize;
			}
			protected set {
				_uiSize = value;
			}
		}

		/// <summary>
		/// Gets the minimum height.
		/// </summary>
		/// <value>The minimum height.</value>
		public override float minHeight
		{
			get
			{
				return BlockSize[1];
			}
		}

		/// <summary>
		/// Gets the minimum width.
		/// </summary>
		/// <value>The minimum width.</value>
		public override float minWidth
		{
			get
			{
				return BlockSize[0];
			}
		}

		/// <summary>
		/// Gets the preferred height.
		/// </summary>
		/// <value>The preferred height.</value>
		public override float preferredHeight
		{
			get
			{
				return BlockSize[1];
			}
		}

		/// <summary>
		/// Gets the preferred width.
		/// </summary>
		/// <value>The preferred width.</value>
		public override float preferredWidth
		{
			get
			{
				return BlockSize[0];
			}
		}

		/// <summary>
		/// Property changed.
		/// </summary>
		/// <param name="propertyName">Property name.</param>
		protected void Changed(string propertyName)
		{
			SetDirty();

			PropertyChanged(this, new PropertyChangedEventArgs(propertyName));

			SettingsChanged.Invoke();
		}

		/// <summary>
		/// Raises the disable event.
		/// </summary>
		protected override void OnDisable()
		{
			Resizer.Clear();
			base.OnDisable();
		}

		/// <summary>
		/// Raises the rect transform removed event.
		/// </summary>
		void OnRectTransformRemoved()
		{
			SetDirty();
		}

		/// <summary>
		/// Sets the layout horizontal.
		/// </summary>
		public override void SetLayoutHorizontal()
		{
			RepositionUIElements();
		}

		/// <summary>
		/// Sets the layout vertical.
		/// </summary>
		public override void SetLayoutVertical()
		{
			RepositionUIElements();
		}

		/// <summary>
		/// Calculates the layout input horizontal.
		/// </summary>
		public override void CalculateLayoutInputHorizontal()
		{
			base.CalculateLayoutInputHorizontal();
			CalculateLayoutSize();
		}

		/// <summary>
		/// Calculates the layout input vertical.
		/// </summary>
		public override void CalculateLayoutInputVertical()
		{
			CalculateLayoutSize();
		}

		/// <summary>
		/// Calculates the size.
		/// </summary>
		void CalculateSize()
		{
			if (uiElementsGroup.Count==0)
			{
				UISize = Vector2.zero;
			}
			else
			{
				UISize = new Vector2(GetWidth(), GetHeight());
			}

			if (Symmetric)
			{
				BlockSize = new Vector2(UISize.x + Margin.x * 2, UISize.y + Margin.y * 2);
			}
			else
			{
				BlockSize = new Vector2(UISize.x + MarginLeft + MarginRight, UISize.y + MarginTop + MarginBottom);
			}
		}

		float GetHeight()
		{
			float height = Spacing.y * (uiElementsGroup.Count - 1);
			foreach (var row in uiElementsGroup)
			{
				float row_height = 0f;
				foreach (var elem in row)
				{
					row_height = Mathf.Max(row_height, elem.Height);
				}
				height += row_height;
			}

			return height + PaddingInner.Top + PaddingInner.Bottom;
		}

		float GetWidth()
		{
			var widths = new List<float>();

			foreach (var block in uiElementsGroup)
			{
				for (var j = 0; j < block.Count; j++)
				{
					if (widths.Count==j)
					{
						widths.Add(0);
					}
					widths[j] = Mathf.Max(widths[j], block[j].Width);
				}
			}

			var width = widths.Sum() + widths.Count * Spacing.x - Spacing.x;

			return width + PaddingInner.Left + PaddingInner.Right;
		}

		/// <summary>
		/// Marks layout to update.
		/// </summary>
		public void NeedUpdateLayout()
		{
			UpdateLayout();
		}

		/// <summary>
		/// Calculates the size of the layout.
		/// </summary>
		public void CalculateLayoutSize()
		{
			GroupUIElements();
		}

		/// <summary>
		/// Repositions the user interface elements.
		/// </summary>
		void RepositionUIElements()
		{
			GroupUIElements();

			Positioner.SetPositions(uiElementsGroup);
		}

		/// <summary>
		/// Updates the layout.
		/// </summary>
		public void UpdateLayout()
		{
			CalculateLayoutInputHorizontal();
			SetLayoutHorizontal();
			CalculateLayoutInputVertical();
			SetLayoutVertical();
		}

		EasyLayoutPositioner positioner;

		/// <summary>
		/// EasyLayout Positioner.
		/// </summary>
		protected EasyLayoutPositioner Positioner {
			get {
				if (positioner==null)
				{
					positioner = new EasyLayoutPositioner(this);
				}
				return positioner;
			}
		}

		EasyLayoutResizer resizer;

		/// <summary>
		/// EasyLayout Resizer.
		/// </summary>
		protected EasyLayoutResizer Resizer {
			get {
				if (resizer==null)
				{
					resizer = new EasyLayoutResizer(this);
				}
				return resizer;
			}
		}

		/// <summary>
		/// Is IgnoreLayout enabled?
		/// </summary>
		/// <param name="rect">RectTransform</param>
		/// <returns>true if IgnoreLayout enabled; otherwise, false.</returns>
		static protected bool IsIgnoreLayout(Transform rect)
		{
			#if UNITY_4_6 || UNITY_4_7
			var ignorer = rect.GetComponent(typeof(ILayoutIgnorer)) as ILayoutIgnorer;
			#else
			var ignorer = rect.GetComponent<ILayoutIgnorer>();
			#endif
			return (ignorer!=null) && ignorer.ignoreLayout;
		}

		List<LayoutElementInfo> GetUIElements()
		{
			var elements = rectChildren;

			if (!SkipInactive)
			{
				elements = new List<RectTransform>();
				foreach (Transform child in transform)
				{
					if (!IsIgnoreLayout(child))
					{
						elements.Add(child as RectTransform);
					}
				}
			}

			if (Filter!=null)
			{
				var temp = Filter(elements.Convert<RectTransform,GameObject>(GetGameObject));
				elements = temp.Select<GameObject,RectTransform>(GetRectTransform).ToList();
			}

			var result = elements.Convert<RectTransform,LayoutElementInfo>(CreateLayoutElementInfo);

			Resizer.ResizeElements(result);

			return result;
		}

		/// <summary>
		/// Create LayoutElementInfo for specified RectTrasnform.
		/// </summary>
		/// <param name="rect">RectTransform.</param>
		/// <returns>LayoutElementInfo for specified RectTrasnform.</returns>
		protected virtual LayoutElementInfo CreateLayoutElementInfo(RectTransform rect)
		{
			return new LayoutElementInfo(rect, Resizer, this);
		}

		/// <summary>
		/// Get GameObject for specified RectTransform.
		/// </summary>
		/// <param name="element">RectTransform.</param>
		/// <returns>GameObject for specified RectTransform.</returns>
		static protected GameObject GetGameObject(RectTransform element)
		{
			return element.gameObject;
		}

		/// <summary>
		/// Get RectTransform for specified GameObject.
		/// </summary>
		/// <param name="go">GameObject.</param>
		/// <returns>RectTransform for specified GameObject.</returns>
		static protected RectTransform GetRectTransform(GameObject go)
		{
			return go.transform as RectTransform;
		}

		/// <summary>
		/// Gets the margin top.
		/// </summary>
		public float GetMarginTop()
		{
			return Symmetric ? Margin.y : MarginTop;
		}
		
		/// <summary>
		/// Gets the margin bottom.
		/// </summary>
		public float GetMarginBottom()
		{
			return Symmetric ? Margin.y : MarginBottom;
		}

		/// <summary>
		/// Gets the margin left.
		/// </summary>
		public float GetMarginLeft()
		{
			return Symmetric ? Margin.x : MarginLeft;
		}

		/// <summary>
		/// Gets the margin right.
		/// </summary>
		public float GetMarginRight()
		{
			return Symmetric ? Margin.y : MarginRight;
		}

		static void ReverseList(List<LayoutElementInfo> list)
		{
			list.Reverse();
		}

		List<List<LayoutElementInfo>> uiElementsGroup = new List<List<LayoutElementInfo>>();
		void GroupUIElements()
		{
			uiElementsGroup.Clear();
			
			var base_length = Stacking==Stackings.Horizontal ? rectTransform.rect.width : rectTransform.rect.height;
			base_length -= (Stacking==Stackings.Horizontal) ? (GetMarginLeft() + GetMarginRight()) : (GetMarginTop() + GetMarginBottom());

			var ui_elements = GetUIElements();

			if (LayoutType==LayoutTypes.Compact)
			{
				EasyLayoutCompact.Group(ui_elements, base_length, this, uiElementsGroup);
			}
			else
			{
				EasyLayoutGrid.Group(ui_elements, base_length, this, uiElementsGroup);
			}

			if (!TopToBottom)
			{
				uiElementsGroup.Reverse();
			}
			
			if (RightToLeft)
			{
				uiElementsGroup.ForEach(ReverseList);
			}

			CalculateSize();

			Resizer.ResizeGroup(uiElementsGroup);

			foreach (var block in uiElementsGroup.ToArray())
			{
				block.ForEach(x => x.ApplyResize());
			}
		}

		/// <summary>
		/// Awake this instance.
		/// </summary>
		protected override void Awake()
		{
			base.Awake();
			Upgrade();
		}

		#if UNITY_EDITOR
		/// <summary>
		/// Update layout when parameters changed.
		/// </summary>
		protected override void OnValidate()
		{
			SetDirty();
		}
		#endif

		[SerializeField]
		int version = 0;

		#pragma warning disable 0618
		/// <summary>
		/// Upgrade to keep compatibility between versions.
		/// </summary>
		public virtual void Upgrade()
		{
			//upgrade to 1.6
			if (version==0)
			{
				if (ControlWidth)
				{
					ChildrenWidth = (MaxWidth) ? ChildrenSize.SetMaxFromPreferred : ChildrenSize.SetPreferred;
				}
				if (ControlHeight)
				{
					ChildrenHeight = (MaxHeight) ? ChildrenSize.SetMaxFromPreferred : ChildrenSize.SetPreferred;
				}
			}
			version = 1;
		}
		#pragma warning restore 0618
	}
}