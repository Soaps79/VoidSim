using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using System.Collections.Generic;
using System;
using System.Linq;

namespace UIWidgets
{
	/// <summary>
	/// ListViewBase event.
	/// </summary>
	[Serializable]
	public class ListViewBaseEvent : UnityEvent<int, ListViewItem>
	{
	}

	/// <summary>
	/// ListViewFocus event.
	/// </summary>
	[Serializable]
	public class ListViewFocusEvent : UnityEvent<BaseEventData>
	{

	}

	/// <summary>
	/// ListViewBase.
	/// You can use it for creating custom ListViews.
	/// </summary>
	abstract public class ListViewBase : MonoBehaviour,
			ISelectHandler, IDeselectHandler,
			ISubmitHandler, ICancelHandler
	{

		[SerializeField]
		[HideInInspector]
		List<ListViewItem> items = new List<ListViewItem>();

		List<UnityAction> callbacks = new List<UnityAction>();

		/// <summary>
		/// Gets or sets the items.
		/// </summary>
		/// <value>Items.</value>
		public List<ListViewItem> Items {
			get {
				return new List<ListViewItem>(items);
			}
			set {
				UpdateItems(value);
			}
		}

		/// <summary>
		/// The destroy game objects after setting new items.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		public bool DestroyGameObjects = true;

		[SerializeField]
		[FormerlySerializedAs("Multiple")]
		[FormerlySerializedAs("multiple")]
		bool multipleSelect;

		/// <summary>
		/// Allow select multiple items.
		/// </summary>
		public bool MultipleSelect {
			get {
				return multipleSelect;
			}
			set {
				if (!value && selectedIndices.Count > 1)
				{
					var deselect = SelectedIndices;
					for (int i=0; i < deselect.Count - 1; i++)
					{
						Deselect(deselect[i]);
					}
				}
				multipleSelect = value;
			}
		}

		/// <summary>
		/// Allow select multiple items.
		/// </summary>
		[Obsolete("Use MultipleSelect instead.")]
		public bool Multiple {
			get {
				return MultipleSelect;
			}
			set {
				MultipleSelect = value;
			}
		}

		#if UNITY_EDITOR
		/// <summary>
		/// Validate this instance.
		/// </summary>
		protected virtual void OnValidate()
		{
			MultipleSelect = multipleSelect;
		}
		#endif

		[SerializeField]
		int selectedIndex = -1;

		/// <summary>
		/// Gets or sets the index of the selected item.
		/// </summary>
		/// <value>The index of the selected.</value>
		public int SelectedIndex {
			get {
				return selectedIndex;
			}
			set {
				if (value==-1)
				{
					if (selectedIndex!=-1)
					{
						Deselect(selectedIndex);
					}

					selectedIndex = value;
				}
				else
				{
					Select(value);
				}
			}
		}

		[SerializeField]
		[UnityEngine.Serialization.FormerlySerializedAs("selectedIndicies")]
		LinkedHashSet<int> selectedIndices = new LinkedHashSet<int>();

		/// <summary>
		/// Gets or sets indices of the selected items.
		/// </summary>
		/// <value>The selected indices.</value>
		public List<int> SelectedIndices {
			get {
				return selectedIndices.Items();
			}
			set {
				var deselect = selectedIndices.Except(value).ToArray();
				var select = value.Except(selectedIndices).ToArray();

				deselect.ForEach(Deselect);
				select.ForEach(Select);
			}
		}

		/// <summary>
		/// Gets or sets indices of the selected items.
		/// </summary>
		/// <value>The selected indices.</value>
		[Obsolete("Use SelectedIndices.")]
		public List<int> SelectedIndicies {
			get {
				return SelectedIndices;
			}
			set {
				SelectedIndices = value;
			}
		}

		/// <summary>
		/// Allow navigation.
		/// </summary>
		[SerializeField]
		public bool Navigation = true;

		/// <summary>
		/// OnSelect event.
		/// </summary>
		public ListViewBaseEvent OnSelect = new ListViewBaseEvent();

		/// <summary>
		/// OnDeselect event.
		/// </summary>
		public ListViewBaseEvent OnDeselect = new ListViewBaseEvent();

		/// <summary>
		/// OnSubmit event.
		/// </summary>
		public UnityEvent onSubmit = new UnityEvent();

		/// <summary>
		/// OnCancel event.
		/// </summary>
		public UnityEvent onCancel = new UnityEvent();

		/// <summary>
		/// OnItemSelect event.
		/// </summary>
		public UnityEvent onItemSelect = new UnityEvent();

		/// <summary>
		/// onItemCancel event.
		/// </summary>
		public UnityEvent onItemCancel = new UnityEvent();

		/// <summary>
		/// The container for items objects.
		/// </summary>
		[SerializeField]
		public Transform Container;

		/// <summary>
		/// OnFocusIn event.
		/// </summary>
		public ListViewFocusEvent OnFocusIn = new ListViewFocusEvent();
		
		/// <summary>
		/// OnFocusOut event.
		/// </summary>
		public ListViewFocusEvent OnFocusOut = new ListViewFocusEvent();

		/// <summary>
		/// Set item indices when items updated.
		/// </summary>
		[NonSerialized]
		protected bool SetItemIndices = true;

		GameObject Unused;

		/// <summary>
		/// Awake this instance.
		/// </summary>
		protected virtual void Awake()
		{

		}

		[System.NonSerialized]
		bool isListViewBaseInited;

		/// <summary>
		/// Start this instance.
		/// </summary>
		public virtual void Start()
		{
			Init();
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		public virtual void Init()
		{
			if (isListViewBaseInited)
			{
				return ;
			}
			isListViewBaseInited = true;
			
			Unused = new GameObject("unused base");
			Unused.SetActive(false);
			Unused.transform.SetParent(transform, false);
			
			if ((selectedIndex!=-1) && (selectedIndices.Count==0))
			{
				selectedIndices.Add(selectedIndex);
			}
			selectedIndices.RemoveWhere(NotIsValid);
			if (selectedIndices.Count==0)
			{
				selectedIndex = -1;
			}
		}

		/// <summary>
		/// Determines if item not exists with the specified index.
		/// </summary>
		/// <returns><c>true</c>, if item not exists, <c>false</c> otherwise.</returns>
		/// <param name="index">Index.</param>
		protected bool NotIsValid(int index)
		{
			return !IsValid(index);
		}

		/// <summary>
		/// Updates the items.
		/// </summary>
		public virtual void UpdateItems()
		{
			UpdateItems(items);
		}

		/// <summary>
		/// Determines whether this instance is horizontal. Not implemented for ListViewBase.
		/// </summary>
		/// <returns><c>true</c> if this instance is horizontal; otherwise, <c>false</c>.</returns>
		public virtual bool IsHorizontal()
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Gets the default height of the item. Not implemented for ListViewBase.
		/// </summary>
		/// <returns>The default item height.</returns>
		public virtual float GetDefaultItemHeight()
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Gets the default width of the item. Not implemented for ListViewBase.
		/// </summary>
		/// <returns>The default item width.</returns>
		public virtual float GetDefaultItemWidth()
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Gets the spacing between items. Not implemented for ListViewBase.
		/// </summary>
		/// <returns>The item spacing.</returns>
		public virtual float GetItemSpacing()
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Gets the horizontal spacing between items. Not implemented for ListViewBase.
		/// </summary>
		/// <returns>The item spacing.</returns>
		public virtual float GetItemSpacingX()
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Gets the vertical spacing between items. Not implemented for ListViewBase.
		/// </summary>
		/// <returns>The item spacing.</returns>
		public virtual float GetItemSpacingY()
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Gets the layout margin.
		/// </summary>
		/// <returns>The layout margin.</returns>
		public virtual Vector4 GetLayoutMargin()
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Removes the callback.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <param name="index">Index.</param>
		void RemoveCallback(ListViewItem item, int index)
		{
			if (item == null)
			{
				return;
			}
			if (index < callbacks.Count)
			{
				item.onClick.RemoveListener(callbacks[index]);
			}

			item.onSubmit.RemoveListener(Toggle);
			item.onCancel.RemoveListener(OnItemCancel);

			item.onSelect.RemoveListener(OnItemSelect);
			item.onSelect.RemoveListener(HighlightColoring);
			item.onDeselect.RemoveListener(Coloring);

			item.onMove.RemoveListener(OnItemMove);
		}

		/// <summary>
		/// Raises the item cancel event.
		/// </summary>
		/// <param name="item">Item.</param>
		void OnItemCancel(ListViewItem item)
		{
			if (EventSystem.current.alreadySelecting)
			{
				return;
			}

			EventSystem.current.SetSelectedGameObject(gameObject);

			onItemCancel.Invoke();
		}

		/// <summary>
		/// Removes the callbacks.
		/// </summary>
		void RemoveCallbacks()
		{
			if (callbacks.Count > 0)
			{
				items.ForEach(RemoveCallback);
			}
			callbacks.Clear();
		}

		/// <summary>
		/// Adds the callbacks.
		/// </summary>
		void AddCallbacks()
		{
			items.ForEach(AddCallback);
		}

		/// <summary>
		/// Adds the callback.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <param name="index">Index.</param>
		void AddCallback(ListViewItem item, int index)
		{
			callbacks.Insert(index, () => Toggle(item));

			item.onClick.AddListener(callbacks[index]);

			item.onSubmit.AddListener(OnItemSubmit);
			item.onCancel.AddListener(OnItemCancel);

			item.onSelect.AddListener(OnItemSelect);
			item.onSelect.AddListener(HighlightColoring);
			item.onDeselect.AddListener(Coloring);

			item.onMove.AddListener(OnItemMove);
		}

		/// <summary>
		/// Raises the item select event.
		/// </summary>
		/// <param name="item">Item.</param>
		void OnItemSelect(ListViewItem item)
		{
			onItemSelect.Invoke();
		}

		/// <summary>
		/// Raises the item submit event.
		/// </summary>
		/// <param name="item">Item.</param>
		void OnItemSubmit(ListViewItem item)
		{
			Toggle(item);
			if (!IsSelected(item.Index))
			{
				HighlightColoring(item);
			}
		}

		/// <summary>
		/// Raises the item move event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		/// <param name="item">Item.</param>
		protected virtual void OnItemMove(AxisEventData eventData, ListViewItem item)
		{
			if (!Navigation)
			{
				return ;
			}

			switch (eventData.moveDir)
			{
				case MoveDirection.Left:
					break;
				case MoveDirection.Right:
					break;
				case MoveDirection.Up:
					if (item.Index > 0)
					{
						SelectComponentByIndex(item.Index - 1);
					}
					break;
				case MoveDirection.Down:
					if (IsValid(item.Index + 1))
					{
						SelectComponentByIndex(item.Index + 1);
					}
					break;
			}
		}

		/// <summary>
		/// Scrolls to item with specifid index.
		/// </summary>
		/// <param name="index">Index.</param>
		public virtual void ScrollTo(int index)
		{

		}

		/// <summary>
		/// Add the specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <returns>Index of added item.</returns>
		public virtual int Add(ListViewItem item)
		{
			if (item.transform.parent!=Container)
			{
				item.transform.SetParent(Container, false);
			}
			AddCallback(item, items.Count);

			items.Add(item);
			item.Index = callbacks.Count - 1;

			return callbacks.Count - 1;
		}

		/// <summary>
		/// Clear items of this instance.
		/// </summary>
		public virtual void Clear()
		{
			items.Clear();
			UpdateItems();
		}

		/// <summary>
		/// Remove the specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <returns>Index of removed item.</returns>
		protected virtual int Remove(ListViewItem item)
		{
			RemoveCallbacks();

			var index = item.Index;

			selectedIndices = new LinkedHashSet<int>(selectedIndices.Where(x => x!=index).Select(x => x > index ? x-- : x));
			if (selectedIndex==index)
			{
				Deselect(index);
				selectedIndex = selectedIndices.Count > 0 ? selectedIndices.Last() : -1;
			}
			else if (selectedIndex > index)
			{
				selectedIndex -= 1;
			}

			items.Remove(item);
			Free(item);

			AddCallbacks();

			return index;
		}

		/// <summary>
		/// Free the specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		void Free(Component item)
		{
			if (item==null)
			{
				return ;
			}
			/*
			if (DestroyGameObjects)
			{
				if (item.gameObject==null)
				{
					return ;
				}
				Destroy(item.gameObject);
			}
			else
			*/
			{
				if ((item.transform==null) || (Unused==null) || (Unused.transform==null))
				{
					return ;
				}
				item.transform.SetParent(Unused.transform, false);
			}
		}

		/// <summary>
		/// Updates the items.
		/// </summary>
		/// <param name="newItems">New items.</param>
		void UpdateItems(List<ListViewItem> newItems)
		{
			RemoveCallbacks();

			items.Where(item => item!=null && !newItems.Contains(item)).ForEach(Free);

			newItems.ForEach(UpdateItem);

			items = newItems;

			AddCallbacks();
		}

		void UpdateItem(ListViewItem item, int index)
		{
			if (item==null)
			{
				return ;
			}
			if (SetItemIndices)
			{
				item.Index = index;
			}
			item.transform.SetParent(Container, false);
		}

		/// <summary>
		/// Determines if item exists with the specified index.
		/// </summary>
		/// <returns><c>true</c> if item exists with the specified index; otherwise, <c>false</c>.</returns>
		/// <param name="index">Index.</param>
		public virtual bool IsValid(int index)
		{
			return (index >= 0) && (index < items.Count);
		}

		/// <summary>
		/// Gets the item.
		/// </summary>
		/// <returns>The item.</returns>
		/// <param name="index">Index.</param>
		protected ListViewItem GetItem(int index)
		{
			return items.Find(x => x.Index==index);
		}

		/// <summary>
		/// Select item by the specified index.
		/// </summary>
		/// <param name="index">Index.</param>
		public void Select(int index)
		{
			Select(index, true);
		}

		/// <summary>
		/// Select item by the specified index.
		/// </summary>
		/// <param name="index">Index.</param>
		/// <param name="raiseEvents">Raise select events?</param>
		public virtual void Select(int index, bool raiseEvents)
		{
			if (index==-1)
			{
				return ;
			}

			if (!IsValid(index))
			{
				var message = string.Format("Index must be between 0 and Items.Count ({0}), but {2}, Gameobject {1}.", items.Count - 1, name, index);
				throw new IndexOutOfRangeException(message);
			}

			if (IsSelected(index))
			{
				return ;
			}

			if (!MultipleSelect)
			{
				if ((selectedIndex!=-1) && (selectedIndex!=index))
				{
					Deselect(selectedIndex);
				}

				selectedIndices.Clear();
			}

			selectedIndices.Add(index);
			selectedIndex = index;

			SelectItem(index);

			if (raiseEvents)
			{
				InvokeSelect(index);
			}
		}

		/// <summary>
		/// Invokes the select event.
		/// </summary>
		/// <param name="index">Index.</param>
		protected virtual void InvokeSelect(int index)
		{
			if (!IsValid(index))
			{
				Debug.LogWarning("Incorrect index: " + index, this);
			}

			OnSelect.Invoke(index, GetItem(index));
		}

		/// <summary>
		/// Invokes the deselect event.
		/// </summary>
		/// <param name="index">Index.</param>
		protected virtual void InvokeDeselect(int index)
		{
			if (!IsValid(index))
			{
				Debug.LogWarning("Incorrect index: " + index, this);
			}

			OnDeselect.Invoke(index, GetItem(index));
		}

		/// <summary>
		/// Deselect specified indices without raising corresponding events (OnDeselect, etc).
		/// </summary>
		/// <param name="indixes">Indices.</param>
		protected virtual void SilentDeselect(IEnumerable<int> indixes)
		{
			if (indixes==null)
			{
				return ;
			}
			indixes.ForEach(x => selectedIndices.Remove(x));

			selectedIndex = (selectedIndices.Count > 0) ? selectedIndices.Last() : -1;
		}

		/// <summary>
		/// Select specified indices without raising corresponding events (OnSelect, etc).
		/// </summary>
		/// <param name="indixes">Indices.</param>
		protected virtual void SilentSelect(IEnumerable<int> indixes)
		{
			if (indixes==null)
			{
				return ;
			}
			indixes.ForEach(selectedIndices.Add);
			selectedIndex = (selectedIndices.Count > 0) ? selectedIndices.Last() : -1;
		}

		/// <summary>
		/// Deselect item by the specified index.
		/// </summary>
		/// <param name="index">Index.</param>
		public void Deselect(int index)
		{
			if (index==-1)
			{
				return ;
			}
			if (!IsSelected(index))
			{
				return ;
			}

			selectedIndices.Remove(index);
			selectedIndex = (selectedIndices.Count > 0) ? selectedIndices.Last() : - 1;

			if (IsValid(index))
			{
				DeselectItem(index);

				InvokeDeselect(index);
			}
		}

		/// <summary>
		/// Determines if item is selected with the specified index.
		/// </summary>
		/// <returns><c>true</c> if item is selected with the specified index; otherwise, <c>false</c>.</returns>
		/// <param name="index">Index.</param>
		public virtual bool IsSelected(int index)
		{
			return selectedIndices.Contains(index);
		}

		/// <summary>
		/// Toggle item by the specified index.
		/// </summary>
		/// <param name="index">Index.</param>
		public void Toggle(int index)
		{
			if (IsSelected(index) && MultipleSelect)
			{
				Deselect(index);
			}
			else
			{
				Select(index);
			}
		}

		/// <summary>
		/// Toggle the specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		void Toggle(ListViewItem item)
		{
			var shift_pressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
			var have_selected = selectedIndices.Count > 0;
			var last_selected = selectedIndex;
			if (MultipleSelect && shift_pressed && have_selected && last_selected!=item.Index)
			{
				// deselect all items except first
				selectedIndices.Items().ForEach(Deselect);
				
				// find min and max indices
				var min = Mathf.Min(last_selected, item.Index);
				var max = Mathf.Max(last_selected, item.Index);
				// select items from min to max
				Enumerable.Range(min, max - min + 1).ForEach(Select);
				return ;
			}

			Toggle(item.Index);
		}

		/// <summary>
		/// Gets the index of the component.
		/// </summary>
		/// <returns>The component index.</returns>
		/// <param name="item">Item.</param>
		protected int GetComponentIndex(ListViewItem item)
		{
			return item.Index;
		}

		/// <summary>
		/// Move the component transform to the end of the local transform list.
		/// </summary>
		/// <param name="item">Item.</param>
		protected void SetComponentAsLastSibling(Component item)
		{
			item.transform.SetAsLastSibling();
		}

		/// <summary>
		/// Called when item selected.
		/// Use it for change visible style of selected item.
		/// </summary>
		/// <param name="index">Index.</param>
		protected virtual void SelectItem(int index)
		{

		}

		/// <summary>
		/// Called when item deselected.
		/// Use it for change visible style of deselected item.
		/// </summary>
		/// <param name="index">Index.</param>
		protected virtual void DeselectItem(int index)
		{
			
		}

		/// <summary>
		/// Updates the colors.
		/// </summary>
		public virtual void ComponentsColoring()
		{
			
		}

		/// <summary>
		/// Coloring the specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected virtual void Coloring(ListViewItem component)
		{
		}
		
		/// <summary>
		/// Set highlights colors of specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected virtual void HighlightColoring(ListViewItem component)
		{
		}

		/// <summary>
		/// This function is called when the MonoBehaviour will be destroyed.
		/// </summary>
		protected virtual void OnDestroy()
		{
			RemoveCallbacks();
			
			items.ForEach(Free);
		}

		/// <summary>
		/// Set EventSystem.current.SetSelectedGameObject with selected or first item.
		/// </summary>
		/// <returns><c>true</c>, if component was selected, <c>false</c> otherwise.</returns>
		public virtual bool SelectComponent()
		{
			if (items.Count==0)
			{
				return false;
			}
			var index = (SelectedIndex!=-1) ? SelectedIndex : 0;
			SelectComponentByIndex(index);

			return true;
		}

		/// <summary>
		/// Selects the component by index.
		/// </summary>
		/// <param name="index">Index.</param>
		protected void SelectComponentByIndex(int index)
		{
			ScrollTo(index);

			var ev = new ListViewItemEventData(EventSystem.current) {
				NewSelectedObject = GetItem(index).gameObject
			};
			ExecuteEvents.Execute<ISelectHandler>(ev.NewSelectedObject, ev, ExecuteEvents.selectHandler);
		}

		/// <summary>
		/// Handle select event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		void ISelectHandler.OnSelect(BaseEventData eventData)
		{
			SelectHandlerOnSelect(eventData);
		}

		/// <summary>
		/// Handle select event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		protected virtual void SelectHandlerOnSelect(BaseEventData eventData)
		{
			if (!EventSystem.current.alreadySelecting)
			{
				EventSystem.current.SetSelectedGameObject(gameObject);
			}
			OnFocusIn.Invoke(eventData);
		}

		/// <summary>
		/// Handle deselect event.
		/// </summary>
		/// <param name="eventData">Current event data.</param>
		void IDeselectHandler.OnDeselect(BaseEventData eventData)
		{
			DeselectHandlerOnDeselect(eventData);
		}

		/// <summary>
		/// Handle deselect event.
		/// </summary>
		/// <param name="eventData">Current event data.</param>
		protected virtual void DeselectHandlerOnDeselect(BaseEventData eventData)
		{
			OnFocusOut.Invoke(eventData);
		}

		/// <summary>
		/// Handle submit event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		void ISubmitHandler.OnSubmit(BaseEventData eventData)
		{
			SubmitHandlerOnSubmit(eventData);
		}

		/// <summary>
		/// Handle submit event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		protected virtual void SubmitHandlerOnSubmit(BaseEventData eventData)
		{
			SelectComponent();
			onSubmit.Invoke();
		}

		/// <summary>
		/// Handle cancel event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		void ICancelHandler.OnCancel(BaseEventData eventData)
		{
			CancelHandlerOnCancel(eventData);
		}

		/// <summary>
		/// Handle cancel event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		protected virtual void CancelHandlerOnCancel(BaseEventData eventData)
		{
			onCancel.Invoke();
		}

		/// <summary>
		/// Calls specified function with each component.
		/// </summary>
		/// <param name="func">Func.</param>
		public virtual void ForEachComponent(Action<ListViewItem> func)
		{
			items.ForEach(func);
		}

		/// <summary>
		/// Calls specified function with each component.
		/// </summary>
		/// <param name="func">Func.</param>
		protected virtual void ForEachComponent<T>(Action<T> func)
			where T : ListViewItem
		{
			items.Select(x => x as T).ForEach(func);
		}
		
		#region ListViewPaginator support
		/// <summary>
		/// Gets the ScrollRect.
		/// </summary>
		/// <returns>The ScrollRect.</returns>
		public virtual ScrollRect GetScrollRect()
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Gets the items count.
		/// </summary>
		/// <returns>The items count.</returns>
		public virtual int GetItemsCount()
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Gets the items per block count.
		/// </summary>
		/// <returns>The items per block.</returns>
		public virtual int GetItemsPerBlock()
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Gets the item position by index.
		/// </summary>
		/// <returns>The item position.</returns>
		/// <param name="index">Index.</param>
		public virtual float GetItemPosition(int index)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Gets the index of the nearest item.
		/// </summary>
		/// <returns>The nearest item index.</returns>
		public virtual int GetNearestItemIndex()
		{
			throw new NotSupportedException();
		}
		#endregion
	}
}