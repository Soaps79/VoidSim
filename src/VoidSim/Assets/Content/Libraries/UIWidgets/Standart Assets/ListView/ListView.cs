using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading;

namespace UIWidgets
{
	/// <summary>
	/// ListView sources.
	/// </summary>
	public enum ListViewSources
	{
		/// <summary>
		/// Use strings as source for list.
		/// </summary>
		List = 0,

		/// <summary>
		/// Get strings from file, one line per string.
		/// </summary>
		File = 1,
	}

	/// <summary>
	/// List view event.
	/// </summary>
	[Serializable]
	public class ListViewEvent : UnityEvent<int,string>
	{

	}

	/// <summary>
	/// List view.
	/// http://ilih.ru/images/unity-assets/UIWidgets/ListView.png
	/// </summary>
	[AddComponentMenu("UI/UIWidgets/ListView")]
	public class ListView : ListViewBase
	{
		[SerializeField]
		[Obsolete("Use DataSource instead.")]
		List<string> strings = new List<string>();

		/// <summary>
		/// Data source.
		/// </summary>
		protected ObservableList<string> dataSource;

		/// <summary>
		/// Gets or sets the data source.
		/// </summary>
		/// <value>The data source.</value>
		public virtual ObservableList<string> DataSource {
			get {
				if (dataSource==null)
				{
					#pragma warning disable 0618
					dataSource = new ObservableList<string>(strings);
					dataSource.OnChange += UpdateItems;
					strings = null;
					#pragma warning restore 0618
				}
				return dataSource;
			}
			set {
				Init();

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
		/// Gets the strings.
		/// </summary>
		/// <value>The strings.</value>
		[Obsolete("Use DataSource instead.")]
		public List<string> Strings {
			get {
				return new List<string>(DataSource);
			}
			set {
				DataSource = new ObservableList<string>(value);
			}
		}

		/// <summary>
		/// Gets the strings.
		/// </summary>
		/// <value>The strings.</value>
		[Obsolete("Use DataSource instead.")]
		public new List<string> Items {
			get {
				return new List<string>(DataSource);
			}
			set {
				DataSource = new ObservableList<string>(value);
			}
		}

		[SerializeField]
		TextAsset file;

		/// <summary>
		/// Gets or sets the file with strings for ListView. One string per line.
		/// </summary>
		/// <value>The file.</value>
		public TextAsset File {
			get {
				return file;
			}
			set {
				file = value;
				if (file!=null)
				{
					GetItemsFromFile(file);
					SetScrollValue(0f);
				}
			}
		}

		/// <summary>
		/// The comments in file start with specified strings.
		/// </summary>
		[SerializeField]
		public List<string> CommentsStartWith = new List<string>(){"#", "//"};

		/// <summary>
		/// The source.
		/// </summary>
		[SerializeField]
		public ListViewSources Source = ListViewSources.List;

		/// <summary>
		/// Allow only unique strings.
		/// </summary>
		[SerializeField]
		public bool Unique = true;

		/// <summary>
		/// Allow empty strings.
		/// </summary>
		[SerializeField]
		public bool AllowEmptyItems;

		[SerializeField]
		Color backgroundColor = Color.white;

		[SerializeField]
		Color textColor = Color.black;

		/// <summary>
		/// Default background color.
		/// </summary>
		public Color BackgroundColor {
			get {
				return backgroundColor;
			}
			set {
				backgroundColor = value;
				UpdateColors();
			}
		}

		/// <summary>
		/// Default text color.
		/// </summary>
		public Color TextColor {
			get {
				return textColor;
			}
			set {
				textColor = value;
				UpdateColors();
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
		public Color HighlightedTextColor = Color.black;

		[SerializeField]
		Color selectedBackgroundColor = new Color(53, 83, 227, 255);

		[SerializeField]
		Color selectedTextColor = Color.black;

		/// <summary>
		/// Background color of selected item.
		/// </summary>
		public Color SelectedBackgroundColor {
			get {
				return selectedBackgroundColor;
			}
			set {
				selectedBackgroundColor = value;
				UpdateColors();
			}
		}

		/// <summary>
		/// Text color of selected item.
		/// </summary>
		public Color SelectedTextColor {
			get {
				return selectedTextColor;
			}
			set {
				selectedTextColor = value;
				UpdateColors();
			}
		}

		/// <summary>
		/// How long a color transition should take.
		/// </summary>
		[SerializeField]
		public float FadeDuration = 0f;

		[SerializeField]
		[FormerlySerializedAs("defaultItem")]
		ImageAdvanced defaultItem;

		/// <summary>
		/// The default item template.
		/// </summary>
		public ImageAdvanced DefaultItem {
			get {
				return defaultItem;
			}
			set {
				SetDefaultItem(value);
			}
		}

		/// <summary>
		/// The components.
		/// </summary>
		protected List<ListViewStringComponent> components = new List<ListViewStringComponent>();

		/// <summary>
		/// The callbacks enter.
		/// </summary>
		protected List<UnityAction<PointerEventData>> callbacksEnter = new List<UnityAction<PointerEventData>>();

		/// <summary>
		/// The callbacks exit.
		/// </summary>
		protected List<UnityAction<PointerEventData>> callbacksExit = new List<UnityAction<PointerEventData>>();

		/// <summary>
		/// The sort.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("Sort")]
		protected bool sort = true;

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
				if (Sort && isListViewInited && sortFunc!=null)
				{
					UpdateItems();
				}
			}
		}

		/// <summary>
		/// The sort function.
		/// </summary>
		protected Func<IEnumerable<string>,IEnumerable<string>> sortFunc = items => items.OrderBy(x => x);

		/// <summary>
		/// Sort function.
		/// Advice to use DataSource.Comparison instead Sort and SortFunc.
		/// </summary>
		public Func<IEnumerable<string>, IEnumerable<string>> SortFunc {
			get {
				return sortFunc;
			}
			set {
				sortFunc = value;
				if (Sort && isListViewInited && sortFunc!=null)
				{
					UpdateItems();
				}
			}
		}

		/// <summary>
		/// OnSelect event.
		/// </summary>
		public ListViewEvent OnSelectString = new ListViewEvent();

		/// <summary>
		/// OnDeselect event.
		/// </summary>
		public ListViewEvent OnDeselectString = new ListViewEvent();

		[SerializeField]
		ScrollRect scrollRect;
		
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

					scrollRect.onValueChanged.RemoveListener(OnScrollUpdate);
				}
				scrollRect = value;
				if (scrollRect!=null)
				{
					var resizeListener = Utilites.GetOrAddComponent<ResizeListener>(scrollRect);
					resizeListener.OnResize.AddListener(SetNeedResize);

					scrollRect.onValueChanged.AddListener(OnScrollUpdate);
				}
			}
		}
				
		/// <summary>
		/// The height of the DefaultItem.
		/// </summary>
		protected float itemHeight;

		/// <summary>
		/// The width of the DefaultItem.
		/// </summary>
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
		/// Set content size fitter settings?
		/// </summary>
		[SerializeField]
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
		/// The direction.
		/// </summary>
		[SerializeField]
		protected ListViewDirection direction = ListViewDirection.Vertical;

		/// <summary>
		/// Gets or sets the direction.
		/// </summary>
		/// <value>The direction.</value>
		public ListViewDirection Direction {
			get {
				return direction;
			}
			set {
				direction = value;

				(Container as RectTransform).anchoredPosition = Vector2.zero;
				if (scrollRect)
				{
					scrollRect.horizontal = IsHorizontal();
					scrollRect.vertical = !IsHorizontal();
				}
				if (CanOptimize() && (layout is EasyLayout.EasyLayout))
				{
					LayoutBridge.IsHorizontal = IsHorizontal();

					CalculateMaxVisibleItems();
				}
				UpdateView();
			}
		}

		[System.NonSerialized]
		bool isListViewInited = false;

		LayoutGroup layout;

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
		/// LayoutBridge.
		/// </summary>
		ILayoutBridge layoutBridge;

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

		HashSet<string> SelectedItemsCache = new HashSet<string>();

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
		/// Init this instance.
		/// </summary>
		public override void Init()
		{
			if (isListViewInited)
			{
				return ;
			}
			isListViewInited = true;

			MainThread = Thread.CurrentThread;

			base.Init();
			base.Items = new List<ListViewItem>();

			SelectedItemsCache.Clear();
			SelectedIndices.Convert<int,string>(GetDataItem).ForEach(x => SelectedItemsCache.Add(x));

			DestroyGameObjects = false;

			DefaultItem.gameObject.SetActive(true);

			if (CanOptimize())
			{
				ScrollRect = scrollRect;

				var scrollRectTransform = scrollRect.transform as RectTransform;
				scrollHeight = scrollRectTransform.rect.height;
				scrollWidth = scrollRectTransform.rect.width;

				CalculateItemSize();
				CalculateMaxVisibleItems();
			}

			SetContentSizeFitter = setContentSizeFitter;

			DefaultItem.gameObject.SetActive(false);

			UpdateItems();
		}

		/// <summary>
		/// Sets the default item.
		/// </summary>
		/// <param name="newDefaultItem">New default item.</param>
		protected virtual void SetDefaultItem(ImageAdvanced newDefaultItem)
		{
			if (newDefaultItem==null)
			{
				throw new ArgumentNullException("newDefaultItem");
			}

			if (!isListViewInited)
			{
				defaultItem = newDefaultItem;
				return ;
			}

			defaultItem.gameObject.SetActive(false);

			RemoveCallbacks();

			components.ForEach(DeactivateComponent);
			components.Clear();

			componentsCache.ForEach(DestroyComponent);
			componentsCache.Clear();

			defaultItem = newDefaultItem;
			defaultItem.gameObject.SetActive(true);

			CalculateItemSize();
			CalculateMaxVisibleItems();

			defaultItem.gameObject.SetActive(false);

			UpdateView();

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
		protected void DestroyComponent(Component component)
		{
			Destroy(component.gameObject);
		}

		/// <summary>
		/// Calculates the size of the item.
		/// </summary>
		protected virtual void CalculateItemSize()
		{
			if (LayoutBridge==null)
			{
				return ;
			}
			var size = LayoutBridge.GetItemSize();
			itemHeight = size.y;
			itemWidth = size.x;
		}

		/// <summary>
		/// Gets the item.
		/// </summary>
		/// <returns>The item.</returns>
		/// <param name="index">Index.</param>
		protected string GetDataItem(int index)
		{
			return DataSource[index];
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
		/// Gets the default height of the item.
		/// </summary>
		/// <returns>The default item height.</returns>
		public override float GetDefaultItemHeight()
		{
			return itemHeight;
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
		/// Gets the spacing between items. Not implemented for ListViewBase.
		/// </summary>
		/// <returns>The item spacing.</returns>
		public override float GetItemSpacing()
		{
			return LayoutBridge.GetSpacing();
		}

		/// <summary>
		/// Calculates the max count of visible items.
		/// </summary>
		protected virtual void CalculateMaxVisibleItems()
		{
			if (IsHorizontal())
			{
				maxVisibleItems = Mathf.CeilToInt(scrollWidth / itemWidth) + 1;
			}
			else
			{
				maxVisibleItems = Mathf.CeilToInt(scrollHeight / itemHeight) + 1;
			}
		}

		/// <summary>
		/// Handle instance resize.
		/// </summary>
		public virtual void Resize()
		{
			NeedResize = false;

			var scrollRectTransform = scrollRect.transform as RectTransform;
			scrollHeight = scrollRectTransform.rect.height;
			scrollWidth = scrollRectTransform.rect.width;

			CalculateMaxVisibleItems();
			UpdateView();
		}

		/// <summary>
		/// Determines whether this instance can be optimized.
		/// </summary>
		/// <returns><c>true</c> if this instance can be optimized; otherwise, <c>false</c>.</returns>
		protected bool CanOptimize()
		{
			return scrollRect!=null && (layout!=null || Container.GetComponent<EasyLayout.EasyLayout>()!=null);
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
			OnSelectString.Invoke(index, item);
			
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
			OnDeselectString.Invoke(index, item);

			DefaultColoring(component);
		}

		/// <summary>
		/// Updates the items.
		/// </summary>
		public override void UpdateItems()
		{
			if (Source==ListViewSources.List)
			{
				SetNewItems(DataSource, IsMainThread);
				DataSourceChanged = !IsMainThread;
			}
			else
			{
				Source = ListViewSources.List;

				GetItemsFromFile(File);
			}
		}

		/// <summary>
		/// Clear strings list.
		/// </summary>
		public override void Clear()
		{
			DataSource.Clear();
		}

		/// <summary>
		/// Gets the items from file.
		/// </summary>
		public void GetItemsFromFile()
		{
			GetItemsFromFile(File);
		}

		/// <summary>
		/// Trim end of string.
		/// </summary>
		/// <returns>The trimmed string.</returns>
		/// <param name="str">String.</param>
		string StringTrimEnd(string str)
		{
			return str.TrimEnd();
		}

		/// <summary>
		/// Determines whether specified string not empty.
		/// </summary>
		/// <returns><c>true</c> if specified string not empty; otherwise, <c>false</c>.</returns>
		/// <param name="str">String.</param>
		bool IsStringNotEmpty(string str)
		{
			return !string.IsNullOrEmpty(str);
		}

		/// <summary>
		/// Determines whether specified string not comment.
		/// </summary>
		/// <returns><c>true</c>, if string not comment, <c>false</c> otherwise.</returns>
		/// <param name="str">String.</param>
		bool NotComment(string str)
		{
			return !CommentsStartWith.Any(comment => str.StartsWith(comment));
		}

		/// <summary>
		/// Gets the items from file.
		/// </summary>
		/// <param name="sourceFile">Source file.</param>
		public void GetItemsFromFile(TextAsset sourceFile)
		{
			if (file==null)
			{
				return ;
			}

			var new_items = sourceFile.text.Split(new string[] {"\r\n", "\r", "\n"}, StringSplitOptions.None).Select<string,string>(StringTrimEnd);
			if (Unique)
			{
				new_items = new_items.Distinct();
			}
			
			if (!AllowEmptyItems)
			{
				new_items = new_items.Where(IsStringNotEmpty);
			}
			
			if (CommentsStartWith.Count > 0)
			{
				new_items = new_items.Where(NotComment);
			}
			SetNewItems(new_items.ToObservableList(), IsMainThread);
			DataSourceChanged = !IsMainThread;
		}

		/// <summary>
		/// Finds the indices of specified item.
		/// </summary>
		/// <returns>The indices.</returns>
		/// <param name="item">Item.</param>
		[Obsolete("Use FindIndices()")]
		public virtual List<int> FindIndicies(string item)
		{
			return FindIndices(item);
		}

		/// <summary>
		/// Finds the indices of specified item.
		/// </summary>
		/// <returns>The indices.</returns>
		/// <param name="item">Item.</param>
		public virtual List<int> FindIndices(string item)
		{
			return Enumerable.Range(0, DataSource.Count)
				.Where(i => DataSource[i]==item)
				.ToList();
		}

		/// <summary>
		/// Finds the index of specified item.
		/// </summary>
		/// <returns>The index.</returns>
		/// <param name="item">Item.</param>
		public virtual int FindIndex(string item)
		{
			return DataSource.IndexOf(item);
		}

		/// <summary>
		/// Add the specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <returns>Index of added item.</returns>
		public virtual int Add(string item)
		{
			var old_indices = (Sort && SortFunc!=null) ? FindIndices(item) : null;

			DataSource.Add(item);

			if (Sort && SortFunc!=null)
			{
				var new_indices = FindIndices(item);
				
				var diff = new_indices.Except(old_indices).ToArray();
				if (diff.Length > 0)
				{
					return diff[0];
				}
				if (new_indices.Count > 0)
				{
					return new_indices[0];
				}
				return -1;
			}
			else
			{
				return DataSource.Count - 1;
			}
		}

		/// <summary>
		/// Remove the specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <returns>Index of removed item.</returns>
		public virtual int Remove(string item)
		{
			var index = FindIndex(item);
			if (index==-1)
			{
				return index;
			}

			DataSource.Remove(item);

			return index;
		}

		/// <summary>
		/// Removes the callback.
		/// </summary>
		/// <param name="component">Component.</param>
		/// <param name="index">Index.</param>
		void RemoveCallback(ListViewStringComponent component, int index)
		{
			if (component==null)
			{
				return ;
			}
			if (index < callbacksEnter.Count)
			{
				component.onPointerEnter.RemoveListener(callbacksEnter[index]);
			}
			if (index < callbacksExit.Count)
			{
				component.onPointerExit.RemoveListener(callbacksExit[index]);
			}
		}

		/// <summary>
		/// Removes the callbacks.
		/// </summary>
		void RemoveCallbacks()
		{
			components.ForEach(RemoveCallback);
			callbacksEnter.Clear();
			callbacksExit.Clear();
		}

		/// <summary>
		/// Adds the callbacks.
		/// </summary>
		void AddCallbacks()
		{
			components.ForEach(AddCallback);
		}

		/// <summary>
		/// Adds the callback.
		/// </summary>
		/// <param name="component">Component.</param>
		/// <param name="index">Index.</param>
		void AddCallback(ListViewStringComponent component, int index)
		{
			callbacksEnter.Add(ev => OnPointerEnterCallback(component));
			callbacksExit.Add(ev => OnPointerExitCallback(component));
			
			component.onPointerEnter.AddListener(callbacksEnter[index]);
			component.onPointerExit.AddListener(callbacksExit[index]);
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
		/// Raises the pointer enter callback event.
		/// </summary>
		/// <param name="component">Component.</param>
		void OnPointerEnterCallback(ListViewStringComponent component)
		{
			if (!IsValid(component.Index))
			{
				var message = string.Format("Index must be between 0 and Items.Count ({0})", DataSource.Count);
				throw new IndexOutOfRangeException(message);
			}

			if (IsSelected(component.Index))
			{
				return ;
			}

			HighlightColoring(component);
		}

		/// <summary>
		/// Raises the pointer exit callback event.
		/// </summary>
		/// <param name="component">Component.</param>
		void OnPointerExitCallback(ListViewStringComponent component)
		{
			if (!IsValid(component.Index))
			{
				var message = string.Format("Index must be between 0 and Items.Count ({0})", DataSource.Count);
				throw new IndexOutOfRangeException(message);
			}

			if (IsSelected(component.Index))
			{
				return ;
			}

			DefaultColoring(component);
		}

		/// <summary>
		/// Sets the scroll value.
		/// </summary>
		/// <param name="value">Value.</param>
		protected void SetScrollValue(float value)
		{
			var current_position = scrollRect.content.anchoredPosition;
			var new_position = new Vector2(current_position.x, value);
			if (new_position != current_position)
			{
				scrollRect.content.anchoredPosition = new_position;
				ScrollUpdate();
			}
		}

		/// <summary>
		/// Gets the scroll value.
		/// </summary>
		/// <returns>The scroll value.</returns>
		protected float GetScrollValue()
		{
			var pos = scrollRect.content.anchoredPosition;
			return Mathf.Max(0f, (IsHorizontal()) ? -pos.x : pos.y);
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
		/// Gets the item bottom position by index.
		/// </summary>
		/// <returns>The item bottom position.</returns>
		/// <param name="index">Index.</param>
		public virtual float GetItemPositionBottom(int index)
		{
			return GetItemPosition(index) + GetItemSize() - LayoutBridge.GetSpacing() + LayoutBridge.GetMargin() - GetScrollSize();
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
			if (strict)
			{
				return first_visible_index;
			}
			
			return Mathf.Min(first_visible_index, Mathf.Max(0, DataSource.Count - visibleItems));
		}

		/// <summary>
		/// Move first component from top to bottom.
		/// </summary>
		/// <returns>Component.</returns>
		ListViewStringComponent ComponentTopToBottom()
		{
			var bottom = components.Count - 1;

			var bottomComponent = components[bottom];
			components.RemoveAt(bottom);
			components.Insert(0, bottomComponent);
			bottomComponent.transform.SetAsFirstSibling();

			return bottomComponent;
		}

		/// <summary>
		/// Move last component from bottom to top.
		/// </summary>
		/// <returns>Component.</returns>
		ListViewStringComponent ComponentBottomToTop()
		{
			var topComponent = components[0];
			components.RemoveAt(0);
			components.Add(topComponent);
			topComponent.transform.SetAsLastSibling();

			return topComponent;
		}

		/// <summary>
		/// Raises the scroll update event.
		/// </summary>
		/// <param name="position">Position.</param>
		protected virtual void OnScrollUpdate(Vector2 position)
		{
			ScrollUpdate();
		}

		/// <summary>
		/// Update ListView according scroll position.
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
				var bottomComponent = ComponentTopToBottom();
				
				bottomComponent.Index = topHiddenItems;
				bottomComponent.SetData(DataSource[topHiddenItems]);
				Coloring(bottomComponent);
			}
			else if (oldTopHiddenItems==(topHiddenItems - 1))
			{
				var topComponent = ComponentBottomToTop();
				
				var new_index = topHiddenItems + visibleItems - 1;
				topComponent.Index = new_index;
				topComponent.SetData(DataSource[new_index]);
				Coloring(topComponent);
			}
			// all other cases
			else
			{
				var new_indices = Enumerable.Range(topHiddenItems, visibleItems).ToArray();
				components.ForEach((x, i) => {
					x.Index = new_indices[i];
					x.SetData(DataSource[new_indices[i]]);
					Coloring(x);
				});
			}
			
			if (LayoutBridge!=null)
			{
				LayoutBridge.SetFiller(CalculateTopFillerSize(), CalculateBottomFillerSize());
				LayoutBridge.UpdateLayout();
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
			Vector2 point;
			var rectTransform = Container as RectTransform;
			if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out point))
			{
				return -1;
			}
			var rect = rectTransform.rect;
			if (!rect.Contains(point))
			{
				return -1;
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

			return Mathf.Min(index, DataSource.Count - 1);
		}

		/// <summary>
		/// The components cache.
		/// </summary>
		List<ListViewStringComponent> componentsCache = new List<ListViewStringComponent>();

		/// <summary>
		/// Determines whether specified component is null.
		/// </summary>
		/// <returns><c>true</c> if this specified component is null; otherwise, <c>false</c>.</returns>
		/// <param name="component">Component.</param>
		bool IsNullComponent(ListViewStringComponent component)
		{
			return component==null;
		}

		/// <summary>
		/// Gets the new components according max count of visible items.
		/// </summary>
		/// <returns>The new components.</returns>
		List<ListViewStringComponent> GetNewComponents()
		{
			componentsCache.RemoveAll(IsNullComponent);
			var new_components = new List<ListViewStringComponent>();
			DataSource.ForEach((x, i) => {
				if (i >= visibleItems)
				{
					return;
				}
				
				if (components.Count > 0)
				{
					new_components.Add(components[0]);
					components.RemoveAt(0);
				}
				else if (componentsCache.Count > 0)
				{
					componentsCache[0].gameObject.SetActive(true);
					new_components.Add(componentsCache[0]);
					componentsCache.RemoveAt(0);
				}
				else
				{
					#if UNITY_4_6 || UNITY_4_7
					var background = Instantiate(DefaultItem) as ImageAdvanced;
					#else
					var background = Instantiate(DefaultItem);
					#endif

					background.gameObject.SetActive(true);
					
					var component = Utilites.GetOrAddComponent<ListViewStringComponent>(background);
					if (component.Text==null)
					{
						component.Text = background.GetComponentInChildren<Text>();
					}

					Utilites.FixInstantiated(DefaultItem, background);

					new_components.Add(component);
				}
			});
			
			components.ForEach(DeactivateComponent);
			componentsCache.AddRange(components);
			components.Clear();
			
			return new_components;
		}

		void DeactivateComponent(ListViewStringComponent component)
		{
			if (component!=null)
			{
				component.MovedToCache();
				component.Index = -1;
				component.gameObject.SetActive(false);
			}
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		protected void UpdateView()
		{
			RemoveCallbacks();
			
			if ((CanOptimize()) && (DataSource.Count > 0))
			{
				visibleItems = (maxVisibleItems < DataSource.Count) ? maxVisibleItems : DataSource.Count;
			}
			else
			{
				visibleItems = DataSource.Count;
			}

			components = GetNewComponents();

			base.Items = components.Convert(x => x as ListViewItem);

			components.ForEach(SetComponentData);
			
			AddCallbacks();
			
			topHiddenItems = 0;
			bottomHiddenItems = DataSource.Count() - visibleItems;

			if (LayoutBridge!=null)
			{
				LayoutBridge.SetFiller(CalculateTopFillerSize(), CalculateBottomFillerSize());
				LayoutBridge.UpdateLayout();
			}

			if (scrollRect!=null)
			{
				var r = scrollRect.transform as RectTransform;
				r.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, r.rect.width);
			}

			if ((CanOptimize()) && (DataSource.Count > 0))
			{
				ScrollUpdate();
			}
		}

		/// <summary>
		/// Sets the component data.
		/// </summary>
		/// <param name="component">Component.</param>
		/// <param name="index">Index.</param>
		void SetComponentData(ListViewStringComponent component, int index)
		{
			component.Index = index;
			component.SetData(DataSource[index]);
			Coloring(component);
		}

		bool IndexNotFound(int index)
		{
			return index==-1;
		}

		/// <summary>
		/// Updates the items.
		/// </summary>
		/// <param name="newItems">New items.</param>
		/// <param name="updateView">Update view?</param>
		protected virtual void SetNewItems(ObservableList<string> newItems, bool updateView=true)
		{
			lock (DataSource)
			{
				DataSource.OnChange -= UpdateItems;

				if (Unique)
				{
					newItems.BeginUpdate();

					var unique = newItems.Distinct().ToArray();

					if (unique.Length!=newItems.Count)
					{
						newItems.Clear();
						newItems.AddRange(unique);
					}

					newItems.EndUpdate();
				}
				if (Sort && SortFunc!=null)
				{
					newItems.BeginUpdate();

					var sorted = SortFunc(newItems).ToArray();

					newItems.Clear();
					newItems.AddRange(sorted);

					newItems.EndUpdate();
				}

				SilentDeselect(SelectedIndices);
				var new_selected_indices = SelectedItemsCache.Select(x => newItems.IndexOf(x)).ToList();
				new_selected_indices.RemoveAll(IndexNotFound);

				dataSource = newItems;

				SilentSelect(new_selected_indices);
				SelectedItemsCache.Clear();
				SelectedIndices.Convert<int,string>(GetDataItem).ForEach(x => SelectedItemsCache.Add(x));

				if (updateView)
				{
					UpdateView();
				}

				DataSource.OnChange += UpdateItems;
			}
		}

		/// <summary>
		/// Calculates the size of the bottom filler.
		/// </summary>
		/// <returns>The bottom filler size.</returns>
		protected virtual float CalculateBottomFillerSize()
		{
			return (bottomHiddenItems==0) ? 0f : bottomHiddenItems * GetItemSize() - LayoutBridge.GetSpacing();
		}

		/// <summary>
		/// Calculates the size of the top filler.
		/// </summary>
		/// <returns>The top filler size.</returns>
		protected virtual float CalculateTopFillerSize()
		{
			return (topHiddenItems==0) ? 0f : topHiddenItems * GetItemSize() - LayoutBridge.GetSpacing();
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
			if (SelectedIndices.Contains(component.Index))
			{
				SelectColoring(component);
			}
			else
			{
				DefaultColoring(component);
			}
		}

		/// <summary>
		/// Updates the colors of components.
		/// </summary>
		void UpdateColors()
		{
			components.ForEach(Coloring);
		}

		/// <summary>
		/// Gets the component.
		/// </summary>
		/// <returns>The component.</returns>
		/// <param name="index">Index.</param>
		ListViewStringComponent GetComponent(int index)
		{
			return components.Find(x => x.Index==index);
		}

		/// <summary>
		/// Set the specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <param name="allowDuplicate">If set to <c>true</c> allow duplicate.</param>
		/// <returns>Index of item.</returns>
		public int Set(string item, bool allowDuplicate=true)
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
		/// Called when item selected.
		/// Use it for change visible style of selected item.
		/// </summary>
		/// <param name="index">Index.</param>
		protected override void SelectItem(int index)
		{
			SelectColoring(GetComponent(index));
		}

		/// <summary>
		/// Called when item deselected.
		/// Use it for change visible style of deselected item.
		/// </summary>
		/// <param name="index">Index.</param>
		protected override void DeselectItem(int index)
		{
			DefaultColoring(GetComponent(index));
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
			HighlightColoring(component as ListViewStringComponent);
		}

		/// <summary>
		/// Set highlights colors of specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected virtual void HighlightColoring(ListViewStringComponent component)
		{
			if (component==null)
			{
				return ;
			}
			if (IsSelected(component.Index))
			{
				return ;
			}
			
			component.GraphicsColoring(HighlightedTextColor, HighlightedBackgroundColor, FadeDuration);
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
			
			SelectColoring(component as ListViewStringComponent);
		}

		/// <summary>
		/// Set select colors of specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected virtual void SelectColoring(ListViewStringComponent component)
		{
			if (component==null)
			{
				return ;
			}
			
			component.GraphicsColoring(selectedTextColor, selectedBackgroundColor, FadeDuration);
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
			
			DefaultColoring(component as ListViewStringComponent);
		}

		/// <summary>
		/// Set default colors of specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected virtual void DefaultColoring(ListViewStringComponent component)
		{
			if (component==null)
			{
				return ;
			}
			
			component.GraphicsColoring(textColor, backgroundColor, FadeDuration);
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
		/// This function is called when the MonoBehaviour will be destroyed.
		/// </summary>
		protected override void OnDestroy()
		{
			ScrollRect = null;

			RemoveCallbacks();

			base.OnDestroy();
		}

		/// <summary>
		/// Is need to handle resize event?
		/// </summary>
		protected bool NeedResize;

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
		}

		/// <summary>
		/// This function is called when the object becomes enabled and active.
		/// </summary>
		public virtual void OnEnable()
		{
			var old = FadeDuration;
			FadeDuration = 0f;
			components.ForEach(Coloring);
			FadeDuration = old;
		}

		void SetNeedResize()
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
		/// Gets the item position by index.
		/// </summary>
		/// <returns>The item position.</returns>
		/// <param name="index">Index.</param>
		public override float GetItemPosition(int index)
		{
			return index * GetItemSize() - GetItemSpacing();
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