using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Linq;
using System.Threading;

namespace UIWidgets
{
	/// <summary>
	/// ListView direction.
	/// Direction to scroll items.
	/// </summary>
	public enum ListViewDirection
	{
		/// <summary>
		/// Horizontal scroll direction.
		/// </summary>
		Horizontal = 0,

		/// <summary>
		/// Vertical scroll direction.
		/// </summary>
		Vertical = 1,
	}

	/// <summary>
	/// Custom ListView event.
	/// </summary>
	[Serializable]
	public class ListViewCustomEvent : UnityEvent<int>
	{
		
	}

	/// <summary>
	/// Base class for custom ListViews.
	/// </summary>
	public class ListViewCustom<TComponent,TItem> : ListViewBase where TComponent : ListViewItem
	{
		/// <summary>
		/// The items.
		/// </summary>
		[SerializeField]
		protected List<TItem> customItems = new List<TItem>();

		/// <summary>
		/// Data source.
		/// </summary>
		protected ObservableList<TItem> dataSource;

		/// <summary>
		/// Gets or sets the data source.
		/// </summary>
		/// <value>The data source.</value>
		public virtual ObservableList<TItem> DataSource {
			get {
				if (dataSource==null)
				{
					#pragma warning disable 0618
					dataSource = new ObservableList<TItem>(customItems);
					dataSource.OnChange += UpdateItems;
					customItems = null;
					#pragma warning restore 0618
				}
				return dataSource;
			}
			set {
				if (!isListViewCustomInited)
				{
					Init();
				}

				SetNewItems(value, IsMainThread);

				if (IsMainThread)
				{
					SetScrollValue(0f);
				}
				else
				{
					DataSourceSetted = true;
				}
			}
		}

		/// <summary>
		/// If data source setted?
		/// </summary>
		protected bool DataSourceSetted = false;

		/// <summary>
		/// Is data source changed?
		/// </summary>
		protected bool DataSourceChanged = false;

		/// <summary>
		/// Gets or sets the items.
		/// </summary>
		/// <value>Items.</value>
		[Obsolete("Use DataSource instead.")]
		new public List<TItem> Items {
			get {
				return new List<TItem>(DataSource);
			}
			set {
				DataSource = new ObservableList<TItem>(value);
			}
		}

		[SerializeField]
		[FormerlySerializedAs("defaultItem")]
		[FormerlySerializedAs("DefaultItem")]
		TComponent defaultItem;

		/// <summary>
		/// The default item template.
		/// </summary>
		public TComponent DefaultItem {
			get {
				return defaultItem;
			}
			set {
				SetDefaultItem(value);
			}
		}

		/// <summary>
		/// The components list.
		/// </summary>
		protected List<TComponent> components = new List<TComponent>();

		/// <summary>
		/// The components cache list.
		/// </summary>
		protected List<TComponent> componentsCache = new List<TComponent>();

		Dictionary<int,UnityAction<PointerEventData>> callbacksEnter = new Dictionary<int,UnityAction<PointerEventData>>();

		Dictionary<int,UnityAction<PointerEventData>> callbacksExit = new Dictionary<int,UnityAction<PointerEventData>>();

		/// <summary>
		/// Gets the selected item.
		/// </summary>
		/// <value>The selected item.</value>
		public TItem SelectedItem {
			get {
				if (SelectedIndex==-1)
				{
					return default(TItem);
				}
				return DataSource[SelectedIndex];
			}
		}

		/// <summary>
		/// Gets the selected items.
		/// </summary>
		/// <value>The selected items.</value>
		public List<TItem> SelectedItems {
			get {
				return SelectedIndices.Convert<int,TItem>(GetDataItem);
			}
		}

		/// <summary>
		/// If enabled scroll limited to last item.
		/// </summary>
		[SerializeField]
		public bool LimitScrollValue = true;

		[SerializeField]
		[FormerlySerializedAs("Sort")]
		bool sort = true;
		
		/// <summary>
		/// Sort items.
		/// Advice to use DataSource.Comparison instead Sort and SortFunc.
		/// </summary>
		public bool Sort {
			get {
				return sort;
			}
			set {
				sort = value;
				if (sort && isListViewCustomInited)
				{
					UpdateItems();
				}
			}
		}

		Func<IEnumerable<TItem>,IEnumerable<TItem>> sortFunc;

		/// <summary>
		/// Sort function.
		/// Advice to use DataSource.Comparison instead Sort and SortFunc.
		/// </summary>
		public Func<IEnumerable<TItem>, IEnumerable<TItem>> SortFunc {
			get {
				return sortFunc;
			}
			set {
				sortFunc = value;
				if (Sort && isListViewCustomInited)
				{
					UpdateItems();
				}
			}
		}
		
		/// <summary>
		/// What to do when the object selected.
		/// </summary>
		public ListViewCustomEvent OnSelectObject = new ListViewCustomEvent();
		
		/// <summary>
		/// What to do when the object deselected.
		/// </summary>
		public ListViewCustomEvent OnDeselectObject = new ListViewCustomEvent();
		
		/// <summary>
		/// What to do when the event system send a pointer enter Event.
		/// </summary>
		public ListViewCustomEvent OnPointerEnterObject = new ListViewCustomEvent();
		
		/// <summary>
		/// What to do when the event system send a pointer exit Event.
		/// </summary>
		public ListViewCustomEvent OnPointerExitObject = new ListViewCustomEvent();

		/// <summary>
		/// Callback after UpdateView() call.
		/// </summary>
		public UnityEvent OnUpdateView = new UnityEvent();

		[SerializeField]
		Color defaultBackgroundColor = Color.white;
		
		[SerializeField]
		Color defaultColor = Color.black;
		
		/// <summary>
		/// Default background color.
		/// </summary>
		public Color DefaultBackgroundColor {
			get {
				return defaultBackgroundColor;
			}
			set {
				defaultBackgroundColor = value;
				ComponentsColoring();
			}
		}
		
		/// <summary>
		/// Default text color.
		/// </summary>
		public Color DefaultColor {
			get {
				return defaultColor;
			}
			set {
				defaultColor = value;
				ComponentsColoring();
			}
		}
		
		/// <summary>
		/// Color of background on pointer over.
		/// </summary>
		[SerializeField]
		public Color HighlightedBackgroundColor = new Color(203, 230, 244, 255);
		
		/// <summary>
		/// Color of text on pointer text.
		/// </summary>
		[SerializeField]
		public Color HighlightedColor = Color.black;
		
		[SerializeField]
		Color selectedBackgroundColor = new Color(53, 83, 227, 255);
		
		[SerializeField]
		Color selectedColor = Color.black;
		
		/// <summary>
		/// Background color of selected item.
		/// </summary>
		public Color SelectedBackgroundColor {
			get {
				return selectedBackgroundColor;
			}
			set {
				selectedBackgroundColor = value;
				ComponentsColoring();
			}
		}
		
		/// <summary>
		/// Text color of selected item.
		/// </summary>
		public Color SelectedColor {
			get {
				return selectedColor;
			}
			set {
				selectedColor = value;
				ComponentsColoring();
			}
		}

		/// <summary>
		/// How long a color transition should take.
		/// </summary>
		[SerializeField]
		public float FadeDuration = 0f;

		/// <summary>
		/// The ScrollRect.
		/// </summary>
		[SerializeField]
		protected ScrollRect scrollRect;

		/// <summary>
		/// Gets or sets the ScrollRect.
		/// </summary>
		/// <value>The ScrollRect.</value>
		public ScrollRect ScrollRect {
			get {
				return scrollRect;
			}
			set {
				if (scrollRect!=null)
				{
					var r = scrollRect.GetComponent<ResizeListener>();
					if (r!=null)
					{
						r.OnResize.RemoveListener(SetNeedResize);
					}
					scrollRect.onValueChanged.RemoveListener(OnScrollRectUpdate);
				}
				scrollRect = value;
				if (scrollRect!=null)
				{
					var resizeListener = Utilites.GetOrAddComponent<ResizeListener>(scrollRect);
					resizeListener.OnResize.AddListener(SetNeedResize);

					scrollRect.onValueChanged.AddListener(OnScrollRectUpdate);
				}
			}
		}

		/// <summary>
		/// The height of the DefaultItem.
		/// </summary>
		[SerializeField]
		[Tooltip("Minimal height of item")]
		protected float itemHeight;

		/// <summary>
		/// The width of the DefaultItem.
		/// </summary>
		[SerializeField]
		[Tooltip("Minimal width of item")]
		protected float itemWidth;

		/// <summary>
		/// The height of the ScrollRect.
		/// </summary>
		protected float scrollHeight;

		/// <summary>
		/// The width of the ScrollRect.
		/// </summary>
		protected float scrollWidth;

		/// <summary>
		/// Count of visible items.
		/// </summary>
		protected int maxVisibleItems;

		/// <summary>
		/// Count of visible items.
		/// </summary>
		protected int visibleItems;

		/// <summary>
		/// Count of hidden items by top filler.
		/// </summary>
		protected int topHiddenItems;

		/// <summary>
		/// Count of hidden items by bottom filler.
		/// </summary>
		protected int bottomHiddenItems;

		/// <summary>
		/// The direction.
		/// </summary>
		[SerializeField]
		protected ListViewDirection direction = ListViewDirection.Vertical;

		/// <summary>
		/// Set content size fitter settings?
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("_setContentSizeFitter")]
		protected bool setContentSizeFitter = true;

		/// <summary>
		/// The set ContentSizeFitter parametres according direction.
		/// </summary>
		public bool SetContentSizeFitter {
			get {
				return setContentSizeFitter;
			}
			set {
				setContentSizeFitter = value;
				if (LayoutBridge!=null)
				{
					LayoutBridge.UpdateContentSizeFitter = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the direction.
		/// </summary>
		/// <value>The direction.</value>
		public ListViewDirection Direction {
			get {
				return direction;
			}
			set {
				SetDirection(value, isListViewCustomInited);
			}
		}

		[System.NonSerialized]
		bool isListViewCustomInited = false;

		/// <summary>
		/// The layout.
		/// </summary>
		protected LayoutGroup layout;

		/// <summary>
		/// Gets the layout.
		/// </summary>
		/// <value>The layout.</value>
		public EasyLayout.EasyLayout Layout {
			get {
				return layout as EasyLayout.EasyLayout;
			}
		}

		/// <summary>
		/// Selected items cache (to keep valid selected indices with updates).
		/// </summary>
		protected HashSet<TItem> SelectedItemsCache = new HashSet<TItem>();

		ILayoutBridge layoutBridge;

		/// <summary>
		/// Scroll use unscaled time.
		/// </summary>
		[SerializeField]
		public bool ScrollUnscaledTime = true;

		/// <summary>
		/// Scroll movement curve.
		/// </summary>
		[SerializeField]
		[Tooltip("Requirements: start value should be less than end value; Recommended start value = 0; end value = 1;")]
		public AnimationCurve ScrollMovement = AnimationCurve.EaseInOut(0, 0, 0.25f, 1);

		/// <summary>
		/// The scroll coroutine.
		/// </summary>
		protected IEnumerator ScrollCoroutine;
		
		/// <summary>
		/// LayoutBridge.
		/// </summary>
		protected ILayoutBridge LayoutBridge {
			get {
				if ((layoutBridge==null) && (CanOptimize()))
				{
					if (layout==null)
					{
						layout = Container.GetComponent<LayoutGroup>();
					}

					if (layout is EasyLayout.EasyLayout)
					{
						layoutBridge = new EasyLayoutBridge(layout as EasyLayout.EasyLayout, DefaultItem.transform as RectTransform, setContentSizeFitter);
						layoutBridge.IsHorizontal = IsHorizontal();
					}
					else if (layout is HorizontalOrVerticalLayoutGroup)
					{
						layoutBridge = new StandardLayoutBridge(layout as HorizontalOrVerticalLayoutGroup, DefaultItem.transform as RectTransform, setContentSizeFitter);
					}
				}
				return layoutBridge;
			}
		}

		/// <summary>
		/// The main thread.
		/// </summary>
		protected Thread MainThread;

		/// <summary>
		/// Gets a value indicating whether this instance is executed in main thread.
		/// </summary>
		/// <value><c>true</c> if this instance is executed in main thread; otherwise, <c>false</c>.</value>
		protected bool IsMainThread {
			get {
				return MainThread!=null && MainThread.Equals(Thread.CurrentThread);
			}
		}

		/// <summary>
		/// Is DefaultItem implements IViewData&lt;TItem&gt;.
		/// </summary>
		protected bool CanSetData;

		/// <summary>
		/// Init this instance.
		/// </summary>
		public override void Init()
		{
			if (isListViewCustomInited)
			{
				return ;
			}
			isListViewCustomInited = true;

			MainThread = Thread.CurrentThread;

			base.Init();
			base.Items = new List<ListViewItem>();

			SelectedItemsCache.Clear();
			SelectedItems.ForEach(x => SelectedItemsCache.Add(x));

			SetItemIndices = false;

			DestroyGameObjects = false;

			CanSetData = DefaultItem is IViewData<TItem>;

			DefaultItem.gameObject.SetActive(true);

			if (CanOptimize())
			{
				ScrollRect = scrollRect;

				var scroll_rect_transform = scrollRect.transform as RectTransform;
				scrollHeight = scroll_rect_transform.rect.height;
				scrollWidth = scroll_rect_transform.rect.width;

				layout = Container.GetComponent<LayoutGroup>();

				CalculateItemSize();
				CalculateMaxVisibleItems();

				var resizeListener = Utilites.GetOrAddComponent<ResizeListener>(scrollRect);
				resizeListener.OnResize.AddListener(SetNeedResize);
			}

			SetContentSizeFitter = setContentSizeFitter;
			
			DefaultItem.gameObject.SetActive(false);

			SetDirection(direction, false);

			UpdateItems();
		}

		/// <summary>
		/// Sets the default item.
		/// </summary>
		/// <param name="newDefaultItem">New default item.</param>
		protected virtual void SetDefaultItem(TComponent newDefaultItem)
		{
			if (newDefaultItem==null)
			{
				throw new ArgumentNullException("newDefaultItem");
			}

			if (!isListViewCustomInited)
			{
				defaultItem = newDefaultItem;
				return ;
			}

			// clear previous DefaultItem data
			defaultItem.gameObject.SetActive(false);

			RemoveCallbacks();

			components.ForEach(DeactivateComponent);
			components.Clear();

			componentsCache.ForEach(DestroyComponent);
			componentsCache.Clear();

			// set new DefaultItem data
			defaultItem = newDefaultItem;
			defaultItem.gameObject.SetActive(true);

			CanSetData = newDefaultItem is IViewData<TItem>;

			CalculateItemSize(true);
			CalculateMaxVisibleItems();

			defaultItem.Owner = this;
			defaultItem.gameObject.SetActive(false);

			UpdateItems();

			if (scrollRect!=null)
			{
				var resizeListener = scrollRect.GetComponent<ResizeListener>();
				if (resizeListener!=null)
				{
					resizeListener.OnResize.Invoke();
				}
			}
		}

		/// <summary>
		/// Destroy the component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected void DestroyComponent(TComponent component)
		{
			Destroy(component.gameObject);
		}

		/// <summary>
		/// Gets the layout margin.
		/// </summary>
		/// <returns>The layout margin.</returns>
		public override Vector4 GetLayoutMargin()
		{
			return LayoutBridge.GetMarginSize();
		}

		/// <summary>
		/// Sets the direction.
		/// </summary>
		/// <param name="newDirection">New direction.</param>
		/// <param name="isInited">If set to <c>true</c> is inited.</param>
		protected virtual void SetDirection(ListViewDirection newDirection, bool isInited = true)
		{
			direction = newDirection;

			if (scrollRect)
			{
				scrollRect.horizontal = IsHorizontal();
				scrollRect.vertical = !IsHorizontal();
			}
			(Container as RectTransform).anchoredPosition = Vector2.zero;
			if (CanOptimize() && (layout is EasyLayout.EasyLayout))
			{
				LayoutBridge.IsHorizontal = IsHorizontal();

				if (isInited)
				{
					CalculateMaxVisibleItems();
				}
			}
			if (isInited)
			{
				UpdateView();
			}
		}

		/// <summary>
		/// Determines whether is sort enabled.
		/// </summary>
		/// <returns><c>true</c> if is sort enabled; otherwise, <c>false</c>.</returns>
		public bool IsSortEnabled()
		{
			if (DataSource.Comparison!=null)
			{
				return true;
			}

			return Sort && SortFunc!=null;
		}

		/// <summary>
		/// Gets the index of the nearest item.
		/// </summary>
		/// <returns>The nearest index.</returns>
		/// <param name="eventData">Event data.</param>
		public virtual int GetNearestIndex(PointerEventData eventData)
		{
			if (IsSortEnabled())
			{
				return -1;
			}

			Vector2 point;
			var rectTransform = Container as RectTransform;
			if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out point))
			{
				return DataSource.Count;
			}
			var rect = rectTransform.rect;
			if (!rect.Contains(point))
			{
				return DataSource.Count;
			}

			return GetNearestIndex(point);
		}

		/// <summary>
		/// Gets the index of the nearest item.
		/// </summary>
		/// <returns>The nearest item index.</returns>
		/// <param name="point">Point.</param>
		public virtual int GetNearestIndex(Vector2 point)
		{
			if (IsSortEnabled())
			{
				return -1;
			}

			var pos = IsHorizontal() ? point.x : -point.y;
			var index = Mathf.RoundToInt(pos / GetItemSize());

			return Mathf.Min(index, DataSource.Count);
		}

		/// <summary>
		/// Gets the spacing between items.
		/// </summary>
		/// <returns>The item spacing.</returns>
		public override float GetItemSpacing()
		{
			return LayoutBridge.GetSpacing();
		}

		/// <summary>
		/// Gets the horizontal spacing between items.
		/// </summary>
		/// <returns>The item spacing.</returns>
		public override float GetItemSpacingX()
		{
			return LayoutBridge.GetSpacingX();
		}

		/// <summary>
		/// Gets the vertical spacing between items.
		/// </summary>
		/// <returns>The item spacing.</returns>
		public override float GetItemSpacingY()
		{
			return LayoutBridge.GetSpacingY();
		}

		/// <summary>
		/// Gets the item.
		/// </summary>
		/// <returns>The item.</returns>
		/// <param name="index">Index.</param>
		protected TItem GetDataItem(int index)
		{
			return DataSource[index];
		}

		/// <summary>
		/// Calculates the size of the item.
		/// <param name="reset">Reset item size.</param>
		/// </summary>
		protected virtual void CalculateItemSize(bool reset=false)
		{
			if (LayoutBridge==null)
			{
				return ;
			}
			var size = LayoutBridge.GetItemSize();

			if ((itemHeight==0f) || reset)
			{
				itemHeight = size.y;
			}
			if ((itemWidth==0f) || reset)
			{
				itemWidth = size.x;
			}
		}

		/// <summary>
		/// Determines whether this instance is horizontal.
		/// </summary>
		/// <returns><c>true</c> if this instance is horizontal; otherwise, <c>false</c>.</returns>
		public override bool IsHorizontal()
		{
			return direction==ListViewDirection.Horizontal;
		}

		/// <summary>
		/// Calculates the max count of visible items.
		/// </summary>
		protected virtual void CalculateMaxVisibleItems()
		{
			if (IsHorizontal())
			{
				maxVisibleItems = Mathf.CeilToInt(scrollWidth / itemWidth);
			}
			else
			{
				maxVisibleItems = Mathf.CeilToInt(scrollHeight / itemHeight);
			}
			maxVisibleItems = Mathf.Max(maxVisibleItems, 1) + 1;

			ValidateContentSize();
		}

		/// <summary>
		/// Validates the content size and item size.
		/// </summary>
		protected virtual void ValidateContentSize()
		{
			var spacing_x = GetItemSpacingX();
			var spacing_y = GetItemSpacingY();

			var height = scrollHeight + spacing_y - LayoutBridge.GetFullMarginY();
			var width = scrollWidth + spacing_x - LayoutBridge.GetFullMarginX();

			int per_block;
			if (IsHorizontal())
			{
				per_block = Mathf.FloorToInt(height / (itemHeight + spacing_y));
				per_block = Mathf.Max(1, per_block);
				per_block = LayoutBridge.ColumnsConstraint(per_block);
			}
			else
			{
				per_block = Mathf.FloorToInt(width / (itemWidth + spacing_x));
				per_block = Mathf.Max(1, per_block);
				per_block = LayoutBridge.RowsConstraint(per_block);
			}

			if (per_block > 1)
			{
				Debug.LogWarning("More that one item per row or column, consider change DefaultItem size or set layout constraint or use TileViewCustom instead ListViewCustom", this);
			}
		}

		/// <summary>
		/// Resize this instance.
		/// </summary>
		public virtual void Resize()
		{
			NeedResize = false;
			
			var scroll_rect_transform = scrollRect.transform as RectTransform;
			scrollHeight = scroll_rect_transform.rect.height;
			scrollWidth = scroll_rect_transform.rect.width;

			CalculateItemSize(true);
			CalculateMaxVisibleItems();
			UpdateView();

			components.Sort(ComponentsComparer);
			components.ForEach(SetComponentAsLastSibling);
		}

		/// <summary>
		/// Determines whether this instance can optimize.
		/// </summary>
		/// <returns><c>true</c> if this instance can optimize; otherwise, <c>false</c>.</returns>
		protected virtual bool CanOptimize()
		{
			var scrollRectSpecified = scrollRect!=null;
			var containerSpecified = Container!=null;
			var currentLayout = containerSpecified ? ((layout!=null) ? layout : Container.GetComponent<LayoutGroup>()) : null;
			var validLayout = currentLayout ? ((currentLayout is EasyLayout.EasyLayout) || (currentLayout is HorizontalOrVerticalLayoutGroup)) : false;
			
			return scrollRectSpecified && validLayout;
		}

		/// <summary>
		/// Invokes the select event.
		/// </summary>
		/// <param name="index">Index.</param>
		protected override void InvokeSelect(int index)
		{
			if (!IsValid(index))
			{
				Debug.LogWarning("Incorrect index: " + index, this);
			}

			var component = GetItem(index);
			var item = DataSource[index];

			base.InvokeSelect(index);

			SelectedItemsCache.Add(item);
			OnSelectObject.Invoke(index);
			
			SelectColoring(component);
		}

		/// <summary>
		/// Invokes the deselect event.
		/// </summary>
		/// <param name="index">Index.</param>
		protected override void InvokeDeselect(int index)
		{
			if (!IsValid(index))
			{
				Debug.LogWarning("Incorrect index: " + index, this);
			}

			var component = GetItem(index);
			var item = DataSource[index];

			base.InvokeDeselect(index);

			SelectedItemsCache.Remove(item);
			OnDeselectObject.Invoke(index);

			DefaultColoring(component);
		}

		/// <summary>
		/// Raises the pointer enter callback event.
		/// </summary>
		/// <param name="item">Item.</param>
		void OnPointerEnterCallback(ListViewItem item)
		{
			OnPointerEnterObject.Invoke(item.Index);

			if (!IsSelected(item.Index))
			{
				HighlightColoring(item);
			}
		}

		/// <summary>
		/// Raises the pointer exit callback event.
		/// </summary>
		/// <param name="item">Item.</param>
		void OnPointerExitCallback(ListViewItem item)
		{
			OnPointerExitObject.Invoke(item.Index);

			if (!IsSelected(item.Index))
			{
				DefaultColoring(item);
			}
		}

		/// <summary>
		/// Set flag to update view when data source changed.
		/// </summary>
		public override void UpdateItems()
		{
			SetNewItems(DataSource, IsMainThread);
			DataSourceChanged = !IsMainThread;
		}

		/// <summary>
		/// Clear items of this instance.
		/// </summary>
		public override void Clear()
		{
			DataSource.Clear();
			SetScrollValue(0f);
		}

		/// <summary>
		/// Add the specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <returns>Index of added item.</returns>
		public virtual int Add(TItem item)
		{
			DataSource.Add(item);
			
			return DataSource.IndexOf(item);
		}
		
		/// <summary>
		/// Remove the specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <returns>Index of removed TItem.</returns>
		public virtual int Remove(TItem item)
		{
			var index = DataSource.IndexOf(item);
			if (index==-1)
			{
				return index;
			}

			DataSource.RemoveAt(index);

			return index;
		}

		/// <summary>
		/// Remove item by specifieitemsex.
		/// </summary>
		/// <returns>Index of removed item.</returns>
		/// <param name="index">Index.</param>
		public virtual void Remove(int index)
		{
			DataSource.RemoveAt(index);		
		}

		/// <summary>
		/// Sets the scroll value.
		/// </summary>
		/// <param name="value">Value.</param>
		/// <param name="callScrollUpdate">Call ScrollUpdate() if position changed.</param>
		protected void SetScrollValue(float value, bool callScrollUpdate=true)
		{
			if (scrollRect.content==null)
			{
				return ;
			}
			var current_position = scrollRect.content.anchoredPosition;
			var new_position = IsHorizontal()
				? new Vector2(-value, current_position.y)
				: new Vector2(current_position.x, value);
			
			var diff_x = IsHorizontal() && Mathf.Abs(current_position.x - new_position.x) > 0.1f;
			var diff_y = !IsHorizontal() && Mathf.Abs(current_position.y - new_position.y) > 0.1f;
			if (diff_x || diff_y)
			{
				scrollRect.content.anchoredPosition = new_position;
				if (callScrollUpdate)
				{
					ScrollUpdate();
				}
			}
		}

		/// <summary>
		/// Gets the scroll value.
		/// </summary>
		/// <returns>The scroll value.</returns>
		protected float GetScrollValue()
		{
			var pos = scrollRect.content.anchoredPosition;
			var result = Mathf.Max(0f, (IsHorizontal()) ? -pos.x : pos.y);
			return float.IsNaN(result) ? 0f : result;
		}

		/// <summary>
		/// Get block index by item index.
		/// </summary>
		/// <param name="index">Item index.</param>
		/// <returns>Block index.</returns>
		protected virtual int GetBlockIndex(int index)
		{
			return index;
		}

		/// <summary>
		/// Scrolls to item with specifid index.
		/// </summary>
		/// <param name="index">Index.</param>
		public override void ScrollTo(int index)
		{
			if (!CanOptimize())
			{
				return ;
			}

			var first_visible = GetFirstVisibleIndex(true);
			var last_visible = GetLastVisibleIndex(true);

			if (first_visible > index)
			{
				SetScrollValue(GetItemPosition(index));
			}
			else if (last_visible < index)
			{
				SetScrollValue(GetItemPositionBottom(index));
			}
		}

		/// <summary>
		/// Scrolls to specified position.
		/// </summary>
		/// <param name="position">Position.</param>
		public virtual void ScrollToPosition(float position)
		{
			if (!CanOptimize())
			{
				return ;
			}

			SetScrollValue(position);
		}

		/// <summary>
		/// Is visible item with specifid index.
		/// </summary>
		/// <param name="index">Index.</param>
		public virtual bool IsVisible(int index)
		{
			if (!CanOptimize())
			{
				return false;
			}

			var first_visible = GetFirstVisibleIndex(true);
			var last_visible = GetLastVisibleIndex(true);

			var block_index = GetBlockIndex(index);
			if (first_visible > block_index)
			{
				return false;
			}
			else if (last_visible < block_index)
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// Starts the scroll coroutine.
		/// </summary>
		/// <param name="coroutine">Coroutine.</param>
		protected virtual void StartScrollCoroutine(IEnumerator coroutine)
		{
			StopScrollCoroutine();
			ScrollCoroutine = coroutine;
			StartCoroutine(ScrollCoroutine);
		}

		/// <summary>
		/// Stops the scroll coroutine.
		/// </summary>
		protected virtual void StopScrollCoroutine()
		{
			if (ScrollCoroutine!=null)
			{
				StopCoroutine(ScrollCoroutine);
			}
		}

		/// <summary>
		/// Stop scrolling.
		/// </summary>
		public virtual void ScrollStop()
		{
			StopScrollCoroutine();
		}

		/// <summary>
		/// Scroll to specified index with time.
		/// </summary>
		/// <param name="index">Index.</param>
		public virtual void ScrollToAnimated(int index)
		{
			StartScrollCoroutine(ScrollToAnimatedCoroutine(index, ScrollUnscaledTime));
		}

		/// <summary>
		/// Scrolls to specified position with time.
		/// </summary>
		/// <param name="position">Position.</param>
		public virtual void ScrollToPositionAnimated(float position)
		{
			StartScrollCoroutine(ScrollToAnimatedCoroutine(position, ScrollUnscaledTime));
		}

		/// <summary>
		/// Scroll to specified index with time coroutine.
		/// </summary>
		/// <returns>The scroll to index with time coroutine.</returns>
		/// <param name="index">Index.</param>
		/// <param name="unscaledTime">Use unscaled time.</param>
		protected virtual IEnumerator ScrollToAnimatedCoroutine(int index, bool unscaledTime)
		{
			var current_position = ScrollRect.content.anchoredPosition;
			var base_position = IsHorizontal() ? Mathf.Abs(current_position.x) : current_position.y;

			var target_pos = GetItemPosition(index);
			if (target_pos > base_position)
			{
				target_pos = GetItemPositionBottom(index);
			}
			return ScrollToAnimatedCoroutine(target_pos, unscaledTime);
		}

		/// <summary>
		/// Scroll to specified position with time coroutine.
		/// </summary>
		/// <returns>The scroll to index with time coroutine.</returns>
		/// <param name="targetPosition">Target position.</param>
		/// <param name="unscaledTime">Use unscaled time.</param>
		protected virtual IEnumerator ScrollToAnimatedCoroutine(float targetPosition, bool unscaledTime)
		{
			var current_position = ScrollRect.content.anchoredPosition;
			var base_position = IsHorizontal() ? Mathf.Abs(current_position.x) : current_position.y;

			float delta;
			var animationLength = ScrollMovement.keys[ScrollMovement.keys.Length - 1].time;
			var startTime = (unscaledTime ? Time.unscaledTime : Time.time);
			do
			{
				delta = ((unscaledTime ? Time.unscaledTime : Time.time) - startTime);
				var value = ScrollMovement.Evaluate(delta);

				var scroll_pos = base_position + ((targetPosition - base_position) * value);

				ScrollRect.content.anchoredPosition = IsHorizontal()
					? new Vector2(-scroll_pos, current_position.y)
					: new Vector2(current_position.x, scroll_pos);
				
				yield return null;
			}
			while (delta < animationLength);

			ScrollRect.content.anchoredPosition = IsHorizontal()
				? new Vector2(-targetPosition, current_position.y)
				: new Vector2(current_position.x, targetPosition);
		}
		
		/// <summary>
		/// Gets the item position by index.
		/// </summary>
		/// <returns>The item position.</returns>
		/// <param name="index">Index.</param>
		public override float GetItemPosition(int index)
		{
			var block_index = GetBlockIndex(index);
			return block_index * GetItemSize();
		}

		/// <summary>
		/// Gets the item middle position by index.
		/// </summary>
		/// <returns>The item middle position.</returns>
		/// <param name="index">Index.</param>
		public virtual float GetItemPositionMiddle(int index)
		{
			return GetItemPosition(index) - (GetScrollSize() / 2) + (GetItemSize() / 2);
		}

		/// <summary>
		/// Gets the item bottom position by index.
		/// </summary>
		/// <returns>The item bottom position.</returns>
		/// <param name="index">Index.</param>
		public virtual float GetItemPositionBottom(int index)
		{
			return GetItemPosition(index) + GetItemSize() - LayoutBridge.GetSpacing() + LayoutBridge.GetMargin() - GetScrollSize();
		}

		/// <summary>
		/// Removes the callbacks.
		/// </summary>
		protected void RemoveCallbacks()
		{
			base.Items.ForEach(RemoveCallback);
		}

		/// <summary>
		/// Adds the callbacks.
		/// </summary>
		protected void AddCallbacks()
		{
			base.Items.ForEach(AddCallback);
		}

		/// <summary>
		/// Removes the callback.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <param name="index">Index.</param>
		protected virtual void RemoveCallback(ListViewItem item, int index)
		{
			if (callbacksEnter.ContainsKey(index))
			{
				if (item!=null)
				{
					item.onPointerEnter.RemoveListener(callbacksEnter[index]);
				}
				callbacksEnter.Remove(index);
			}
			if (callbacksExit.ContainsKey(index))
			{
				if (item!=null)
				{
					item.onPointerExit.RemoveListener(callbacksExit[index]);
				}
				callbacksExit.Remove(index);
			}
		}

		/// <summary>
		/// Adds the callback.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <param name="index">Index.</param>
		protected virtual void AddCallback(ListViewItem item, int index)
		{
			callbacksEnter.Add(index, ev => OnPointerEnterCallback(item));
			callbacksExit.Add(index, ev => OnPointerExitCallback(item));
			
			item.onPointerEnter.AddListener(callbacksEnter[index]);
			item.onPointerExit.AddListener(callbacksExit[index]);
		}

		/// <summary>
		/// Set the specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <param name="allowDuplicate">If set to <c>true</c> allow duplicate.</param>
		/// <returns>Index of item.</returns>
		public int Set(TItem item, bool allowDuplicate=true)
		{
			int index;
			if (!allowDuplicate)
			{
				index = DataSource.IndexOf(item);
				if (index==-1)
				{
					index = Add(item);
				}
			}
			else
			{
				index = Add(item);
			}

			Select(index);
			
			return index;
		}

		/// <summary>
		/// Updates the component layout.
		/// </summary>
		/// <param name="component">Component.</param>
		protected virtual void UpdateComponentLayout(TComponent component)
		{
			var lg = component.GetComponentsInChildren<LayoutGroup>();
			Array.Reverse(lg);
			lg.ForEach(LayoutUtilites.UpdateLayout);

			LayoutUtilites.UpdateLayout(component.GetComponent<LayoutGroup>());
		}

		/// <summary>
		/// Sets component data with specified item.
		/// </summary>
		/// <param name="component">Component.</param>
		/// <param name="item">Item.</param>
		protected virtual void SetData(TComponent component, TItem item)
		{
			if (CanSetData)
			{
				(component as IViewData<TItem>).SetData(item);
			}
		}

		/// <summary>
		/// Updates the components count.
		/// </summary>
		protected void UpdateComponentsCount()
		{
			components.RemoveAll(IsNullComponent);

			if (components.Count==visibleItems)
			{
				return ;
			}

			if (components.Count < visibleItems)
			{
				componentsCache.RemoveAll(IsNullComponent);

				Enumerable.Range(0, visibleItems - components.Count).ForEach(AddComponent);
			}
			else
			{
				var to_cache = components.GetRange(visibleItems, components.Count - visibleItems).OrderByDescending<TComponent,int>(GetComponentIndex);

				to_cache.ForEach(DeactivateComponent);
				componentsCache.AddRange(to_cache);
				components.RemoveRange(visibleItems, components.Count - visibleItems);
			}

			base.Items = components.Convert(x => x as ListViewItem);
		}

		/// <summary>
		/// Is component is null?
		/// </summary>
		/// <param name="component">Component.</param>
		/// <returns>true if component is null; otherwise, false.</returns>
		protected bool IsNullComponent(TComponent component)
		{
			return component==null;
		}

		/// <summary>
		/// Create component instance.
		/// </summary>
		/// <returns>Component instance.</returns>
		protected TComponent CreateComponent()
		{
			TComponent component;
			if (componentsCache.Count > 0)
			{
				component = componentsCache[componentsCache.Count - 1];
				componentsCache.RemoveAt(componentsCache.Count - 1);
			}
			else
			{
				component = Instantiate(DefaultItem) as TComponent;
				component.transform.SetParent(Container, false);
				Utilites.FixInstantiated(DefaultItem, component);
				component.Owner = this;
			}
			component.Index = -1;
			component.transform.SetAsLastSibling();
			component.gameObject.SetActive(true);

			return component;
		}

		/// <summary>
		/// Add new component to components.
		/// </summary>
		/// <param name="index">Index.</param>
		protected void AddComponent(int index)
		{
			components.Add(CreateComponent());
		}

		void DeactivateComponent(TComponent component)
		{
			if (component!=null)
			{
				RemoveCallback(component, component.Index);
				component.MovedToCache();
				component.Index = -1;
				component.gameObject.SetActive(false);
			}
		}

		/// <summary>
		/// Gets the default width of the item.
		/// </summary>
		/// <returns>The default item width.</returns>
		public override float GetDefaultItemWidth()
		{
			return itemWidth;
		}

		/// <summary>
		/// Gets the default height of the item.
		/// </summary>
		/// <returns>The default item height.</returns>
		public override float GetDefaultItemHeight()
		{
			return itemHeight;
		}

		/// <summary>
		/// Gets the size of the item.
		/// </summary>
		/// <returns>The item size.</returns>
		protected float GetItemSize()
		{
			return (IsHorizontal())
				? itemWidth + LayoutBridge.GetSpacing()
				: itemHeight + LayoutBridge.GetSpacing();
		}

		/// <summary>
		/// Gets the size of the scroll.
		/// </summary>
		/// <returns>The scroll size.</returns>
		protected float GetScrollSize()
		{
			return (IsHorizontal()) ? scrollWidth : scrollHeight;
		}

		/// <summary>
		/// Gets the last index of the visible.
		/// </summary>
		/// <returns>The last visible index.</returns>
		/// <param name="strict">If set to <c>true</c> strict.</param>
		protected virtual int GetLastVisibleIndex(bool strict=false)
		{
			var window = GetScrollValue() + GetScrollSize();
			var last_visible_index = (strict)
				? Mathf.FloorToInt(window / GetItemSize())
				: Mathf.CeilToInt(window / GetItemSize());

			return last_visible_index - 1;
		}

		/// <summary>
		/// Gets the first index of the visible.
		/// </summary>
		/// <returns>The first visible index.</returns>
		/// <param name="strict">If set to <c>true</c> strict.</param>
		protected virtual int GetFirstVisibleIndex(bool strict=false)
		{
			var first_visible_index = (strict)
				? Mathf.CeilToInt(GetScrollValue() / GetItemSize())
				: Mathf.FloorToInt(GetScrollValue() / GetItemSize());
			first_visible_index = Mathf.Max(0, first_visible_index);
			if (strict)
			{
				return first_visible_index;
			}

			return Mathf.Min(first_visible_index, Mathf.Max(0, DataSource.Count - visibleItems));
		}

		/// <summary>
		/// On ScrollUpdate.
		/// </summary>
		protected virtual void ScrollUpdate()
		{
			var oldTopHiddenItems = topHiddenItems;
			
			topHiddenItems = GetFirstVisibleIndex();
			bottomHiddenItems = Mathf.Max(0, DataSource.Count - visibleItems - topHiddenItems);
			
			if (oldTopHiddenItems==topHiddenItems)
			{
				//do nothing
			}
			// optimization on +-1 item scroll
			else if (oldTopHiddenItems==(topHiddenItems + 1))
			{
				var bottomComponent = components[components.Count - 1];
				components.RemoveAt(components.Count - 1);
				components.Insert(0, bottomComponent);
				bottomComponent.transform.SetAsFirstSibling();
				
				bottomComponent.Index = topHiddenItems;
				SetData(bottomComponent, DataSource[topHiddenItems]);
				UpdateComponentLayout(bottomComponent);
				Coloring(bottomComponent as ListViewItem);
			}
			else if (oldTopHiddenItems==(topHiddenItems - 1))
			{
				var topComponent = components[0];
				components.RemoveAt(0);
				components.Add(topComponent);
				topComponent.transform.SetAsLastSibling();
				
				topComponent.Index = topHiddenItems + visibleItems - 1;
				SetData(topComponent, DataSource[topHiddenItems + visibleItems - 1]);
				UpdateComponentLayout(topComponent);
				Coloring(topComponent as ListViewItem);
			}
			// all other cases
			else
			{
				var current_visible_range = components.Convert<TComponent,int>(GetComponentIndex);
				var new_visible_range = Enumerable.Range(topHiddenItems, visibleItems).ToArray();

				var new_indices_to_change = new_visible_range.Except(current_visible_range).ToList();
				var components_to_change = new Stack<TComponent>(components.Where(x => !new_visible_range.Contains(x.Index)));

				if (new_indices_to_change.Count > components_to_change.Count)
				{
					for (int i = 0; i < new_indices_to_change.Count - components_to_change.Count; i++)
					{
						var component = CreateComponent();
						components_to_change.Push(component);
						components.Add(component);
					}
				}
				else if (new_indices_to_change.Count < components_to_change.Count)
				{
					for (int i = 0; i < components_to_change.Count - new_indices_to_change.Count; i++)
					{
						var component = components_to_change.Pop();
						DeactivateComponent(component);
						componentsCache.Add(component);
					}
				}

				new_indices_to_change.ForEach(index => {
					var component = components_to_change.Pop();

					component.Index = index;
					SetData(component, DataSource[index]);
					UpdateComponentLayout(component);
					Coloring(component as ListViewItem);
				});

				components.Sort(ComponentsComparer);
				components.ForEach(SetComponentAsLastSibling);
			}
			
			if (LayoutBridge!=null)
			{
				LayoutBridge.SetFiller(CalculateTopFillerSize(), CalculateBottomFillerSize());
				LayoutBridge.UpdateLayout();
			}
		}

		/// <summary>
		/// Compare components by component index.
		/// </summary>
		/// <returns>A signed integer that indicates the relative values of x and y.</returns>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		protected int ComponentsComparer(TComponent x, TComponent y)
		{
			return x.Index.CompareTo(y.Index);
		}

		/// <summary>
		/// Raises the scroll rect update event.
		/// </summary>
		/// <param name="position">Position.</param>
		protected virtual void OnScrollRectUpdate(Vector2 position)
		{
			StartScrolling();
			ScrollUpdate();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		protected void UpdateView()
		{
			RemoveCallbacks();

			if ((CanOptimize()) && (DataSource.Count > 0))
			{
				visibleItems = Mathf.Min(maxVisibleItems, DataSource.Count);
			}
			else
			{
				visibleItems = DataSource.Count;
			}

			if (CanOptimize())
			{
				topHiddenItems = GetFirstVisibleIndex();
				if (topHiddenItems > (DataSource.Count - 1))
				{
					topHiddenItems = Mathf.Max(0, DataSource.Count - 2);
				}
				if ((topHiddenItems + visibleItems) > DataSource.Count)
				{
					visibleItems = DataSource.Count - topHiddenItems;
				}
				bottomHiddenItems = Mathf.Max(0, DataSource.Count - visibleItems - topHiddenItems);
			}
			else
			{
				topHiddenItems = 0;
				bottomHiddenItems = DataSource.Count() - visibleItems;
			}

			UpdateComponentsCount();

			var indices = Enumerable.Range(topHiddenItems, visibleItems).ToArray();
			components.ForEach((x, i) => {
				x.Index = indices[i];
				SetData(x, DataSource[indices[i]]);
				UpdateComponentLayout(x);
				Coloring(x as ListViewItem);
			});
			
			AddCallbacks();
			
			if (LayoutBridge!=null)
			{
				LayoutBridge.SetFiller(CalculateTopFillerSize(), CalculateBottomFillerSize());
				LayoutBridge.UpdateLayout();

				if ((LimitScrollValue) && (ScrollRect!=null))
				{
					var item_ends = (DataSource.Count==0) ? 0f : Mathf.Max(0f, GetItemPositionBottom(DataSource.Count - 1));
					
					if (GetScrollValue() > item_ends)
					{
						SetScrollValue(item_ends);
					}
				}
			}

			OnUpdateView.Invoke();
		}

		/// <summary>
		/// Keep selected items on items update.
		/// </summary>
		[SerializeField]
		protected bool KeepSelection = true;

		/// <summary>
		/// Check if index is valid.
		/// </summary>
		/// <returns><c>true</c>, if index not valid, <c>false</c> otherwise.</returns>
		/// <param name="index">Index.</param>
		protected bool IndexNotFound(int index)
		{
			return index==-1;
		}

		/// <summary>
		/// Updates the items.
		/// </summary>
		/// <param name="newItems">New items.</param>
		/// <param name="updateView">Update view.</param>
		protected virtual void SetNewItems(ObservableList<TItem> newItems, bool updateView=true)
		{
			lock (DataSource)
			{
				DataSource.OnChange -= UpdateItems;
				
				if (Sort && SortFunc!=null)
				{
					newItems.BeginUpdate();
					
					var sorted = SortFunc(newItems).ToArray();
					
					newItems.Clear();
					newItems.AddRange(sorted);
					
					newItems.EndUpdate();
				}

				SilentDeselect(SelectedIndices);
				var new_selected_indices = RecalculateSelectedIndices(newItems);

				dataSource = newItems;

				CalculateMaxVisibleItems();

				if (KeepSelection)
				{
					SilentSelect(new_selected_indices);
				}
				SelectedItemsCache.Clear();
				SelectedItems.ForEach(x => SelectedItemsCache.Add(x));

				if (updateView)
				{
					UpdateView();
				}

				DataSource.OnChange += UpdateItems;
			}
		}

		/// <summary>
		/// Recalculates the selected indices.
		/// </summary>
		/// <returns>The selected indices.</returns>
		/// <param name="newItems">New items.</param>
		protected virtual List<int> RecalculateSelectedIndices(ObservableList<TItem> newItems)
		{
			var new_selected_indices = SelectedItemsCache.Select(x => newItems.IndexOf(x)).ToList();
			new_selected_indices.RemoveAll(IndexNotFound);

			return new_selected_indices;
		}

		/// <summary>
		/// Calculates the size of the bottom filler.
		/// </summary>
		/// <returns>The bottom filler size.</returns>
		protected virtual float CalculateBottomFillerSize()
		{
			if (bottomHiddenItems==0)
			{
				return 0f;
			}
			return Mathf.Max(0, bottomHiddenItems * GetItemSize());
		}

		/// <summary>
		/// Calculates the size of the top filler.
		/// </summary>
		/// <returns>The top filler size.</returns>
		protected virtual float CalculateTopFillerSize()
		{
			if (topHiddenItems==0)
			{
				return 0f;
			}
			return Mathf.Max(0, topHiddenItems * GetItemSize());
		}

		/// <summary>
		/// Determines if item exists with the specified index.
		/// </summary>
		/// <returns><c>true</c> if item exists with the specified index; otherwise, <c>false</c>.</returns>
		/// <param name="index">Index.</param>
		public override bool IsValid(int index)
		{
			return (index >= 0) && (index < DataSource.Count);
		}

		/// <summary>
		/// Coloring the specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected override void Coloring(ListViewItem component)
		{
			if (component==null)
			{
				return ;
			}
			if (IsSelected(component.Index))
			{
				SelectColoring(component);
			}
			else
			{
				DefaultColoring(component);
			}
		}

		/// <summary>
		/// Set highlights colors of specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected override void HighlightColoring(ListViewItem component)
		{
			if (component==null)
			{
				return ;
			}
			if (IsSelected(component.Index))
			{
				return ;
			}
			HighlightColoring(component as TComponent);
		}

		/// <summary>
		/// Set highlights colors of specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected virtual void HighlightColoring(TComponent component)
		{
			if (component==null)
			{
				return ;
			}
			if (IsSelected(component.Index))
			{
				return ;
			}
			
			component.GraphicsColoring(HighlightedColor, HighlightedBackgroundColor, FadeDuration);
		}

		/// <summary>
		/// Set select colors of specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected virtual void SelectColoring(ListViewItem component)
		{
			if (component==null)
			{
				return ;
			}

			SelectColoring(component as TComponent);
		}

		/// <summary>
		/// Set select colors of specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected virtual void SelectColoring(TComponent component)
		{
			if (component==null)
			{
				return ;
			}
			
			component.GraphicsColoring(SelectedColor, SelectedBackgroundColor, FadeDuration);
		}

		/// <summary>
		/// Set default colors of specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected virtual void DefaultColoring(ListViewItem component)
		{
			if (component==null)
			{
				return ;
			}

			DefaultColoring(component as TComponent);
		}

		/// <summary>
		/// Set default colors of specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected virtual void DefaultColoring(TComponent component)
		{
			if (component==null)
			{
				return ;
			}
			
			component.GraphicsColoring(DefaultColor, DefaultBackgroundColor, FadeDuration);
		}

		/// <summary>
		/// Updates the colors.
		/// </summary>
		public override void ComponentsColoring()
		{
			components.ForEach(x => Coloring(x as ListViewItem));
		}

		/// <summary>
		/// This function is called when the MonoBehaviour will be destroyed.
		/// </summary>
		protected override void OnDestroy()
		{	
			layout = null;
			layoutBridge = null;

			ScrollRect = null;

			RemoveCallbacks();
			
			base.OnDestroy();
		}

		/// <summary>
		/// Calls specified function with each component.
		/// </summary>
		/// <param name="func">Func.</param>
		public override void ForEachComponent(Action<ListViewItem> func)
		{
			base.ForEachComponent(func);
			func(DefaultItem);
			componentsCache.Select(x => x as ListViewItem).ForEach(func);
		}

		/// <summary>
		/// Calls specified function with each component.
		/// </summary>
		/// <param name="func">Func.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1061:DoNotHideBaseClassMethods")]
		public virtual void ForEachComponent(Action<TComponent> func)
		{
			base.ForEachComponent<TComponent>(func);
			func(DefaultItem);
			componentsCache.Select(x => x).ForEach(func);
		}

		/// <summary>
		/// Determines whether item visible.
		/// </summary>
		/// <returns><c>true</c> if item visible; otherwise, <c>false</c>.</returns>
		/// <param name="index">Index.</param>
		public bool IsItemVisible(int index)
		{
			return topHiddenItems<=index && index<=(topHiddenItems + visibleItems - 1);
		}

		/// <summary>
		/// Gets the visible indices.
		/// </summary>
		/// <returns>The visible indices.</returns>
		[Obsolete("Use GetVisibleIndices()")]
		public List<int> GetVisibleIndicies()
		{
			return GetVisibleIndices();
		}

		/// <summary>
		/// Gets the visible indices.
		/// </summary>
		/// <returns>The visible indices.</returns>
		public List<int> GetVisibleIndices()
		{
			return Enumerable.Range(topHiddenItems, visibleItems).ToList();
		}

		/// <summary>
		/// Gets the visible components.
		/// </summary>
		/// <returns>The visible components.</returns>
		public List<TComponent> GetVisibleComponents()
		{
			return components.ToList();
		}

		/// <summary>
		/// Gets the item component.
		/// </summary>
		/// <returns>The item component.</returns>
		/// <param name="index">Index.</param>
		public TComponent GetItemComponent(int index)
		{
			return GetItem(index) as TComponent;
		}

		/// <summary>
		/// OnStartScrolling event.
		/// </summary>
		public UnityEvent OnStartScrolling = new UnityEvent();

		/// <summary>
		/// OnEndScrolling event.
		/// </summary>
		public UnityEvent OnEndScrolling = new UnityEvent();

		/// <summary>
		/// Time before raise OnEndScrolling event since last OnScrollRectUpdate event raised.
		/// </summary>
		public float EndScrollDelay = 0.3f;

		/// <summary>
		/// Is ScrollRect now on scrolling state.
		/// </summary>
		protected bool Scrolling;

		/// <summary>
		/// When last scroll event happen?
		/// </summary>
		protected float LastScrollingTime;

		/// <summary>
		/// Update this instance.
		/// </summary>
		protected virtual void Update()
		{
			if (DataSourceSetted || DataSourceChanged)
			{
				var reset_scroll = DataSourceSetted;

				DataSourceSetted = false;
				DataSourceChanged = false;

				lock (DataSource)
				{
					UpdateView();
				}
				if (reset_scroll)
				{
					SetScrollValue(0f);
				}
			}

			if (NeedResize)
			{
				Resize();
			}

			if (IsStopScrolling())
			{
				EndScrolling();
			}
		}

		/// <summary>
		/// This function is called when the object becomes enabled and active.
		/// </summary>
		public virtual void OnEnable()
		{
			StartCoroutine(ForceRebuild());

			var old = FadeDuration;
			FadeDuration = 0f;
			components.ForEach(Coloring);
			FadeDuration = old;
		}

		System.Collections.IEnumerator ForceRebuild()
		{
			yield return null;
			ForEachComponent(MarkLayoutForRebuild);
		}

		void MarkLayoutForRebuild(ListViewItem item)
		{
			if (item!=null)
			{
				LayoutRebuilder.MarkLayoutForRebuild(item.transform as RectTransform);
			}
		}

		/// <summary>
		/// Start to track scrolling event.
		/// </summary>
		protected virtual void StartScrolling()
		{
			LastScrollingTime = Time.unscaledTime;
			if (Scrolling)
			{
				return ;
			}
			Scrolling = true;
			OnStartScrolling.Invoke();
		}

		/// <summary>
		/// Determines whether ScrollRect is stop scrolling.
		/// </summary>
		/// <returns><c>true</c> if ScrollRect is stop scrolling; otherwise, <c>false</c>.</returns>
		protected virtual bool IsStopScrolling()
		{
			if (!Scrolling)
			{
				return false;
			}
			return (LastScrollingTime + EndScrollDelay) <= Time.unscaledTime;
		}

		/// <summary>
		/// Raise OnEndScrolling event.
		/// </summary>
		protected virtual void EndScrolling()
		{
			Scrolling = false;
			OnEndScrolling.Invoke();
		}

		/// <summary>
		/// Is need to handle resize event?
		/// </summary>
		protected bool NeedResize;

		/// <summary>
		/// Sets the need resize.
		/// </summary>
		protected virtual void SetNeedResize()
		{
			if (!CanOptimize())
			{
				return ;
			}
			NeedResize = true;
		}

		#region ListViewPaginator support
		/// <summary>
		/// Gets the ScrollRect.
		/// </summary>
		/// <returns>The ScrollRect.</returns>
		public override ScrollRect GetScrollRect()
		{
			return ScrollRect;
		}

		/// <summary>
		/// Gets the items count.
		/// </summary>
		/// <returns>The items count.</returns>
		public override int GetItemsCount()
		{
			return DataSource.Count;
		}

		/// <summary>
		/// Gets the items per block count.
		/// </summary>
		/// <returns>The items per block.</returns>
		public override int GetItemsPerBlock()
		{
			return 1;
		}

		/// <summary>
		/// Gets the index of the nearest item.
		/// </summary>
		/// <returns>The nearest item index.</returns>
		public override int GetNearestItemIndex()
		{
			return Mathf.Clamp(Mathf.RoundToInt(GetScrollValue() / GetItemSize()), 0, DataSource.Count - 1);
		}
		#endregion
	}
}