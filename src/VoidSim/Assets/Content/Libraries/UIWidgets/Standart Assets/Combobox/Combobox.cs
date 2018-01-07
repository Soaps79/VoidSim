﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace UIWidgets
{
	/// <summary>
	/// Combobox.
	/// http://ilih.ru/images/unity-assets/UIWidgets/Combobox.png
	/// </summary>
	[AddComponentMenu("UI/UIWidgets/Combobox")]
	public class Combobox : MonoBehaviour, ISubmitHandler
	{
		[SerializeField]
		ListView listView;

		/// <summary>
		/// Gets or sets the ListView.
		/// </summary>
		/// <value>ListView component.</value>
		public ListView ListView {
			get {
				return listView;
			}
			set {
				SetListView(value);
			}
		}

		[SerializeField]
		Button toggleButton;

		/// <summary>
		/// Gets or sets the toggle button.
		/// </summary>
		/// <value>The toggle button.</value>
		public Button ToggleButton {
			get {
				return toggleButton;
			}
			set {
				SetToggleButton(value);
			}
		}

		[SerializeField]
		bool editable = true;

		/// <summary>
		/// Gets or sets a value indicating whether this is editable and user can add new items.
		/// </summary>
		/// <value><c>true</c> if editable; otherwise, <c>false</c>.</value>
		public bool Editable {
			get {
				return editable;
			}
			set {
				editable = value;
				if (InputProxy!=null)
				{
					InputProxy.interactable = editable;
				}
			}
		}

		/// <summary>
		/// Proxy for InputField.
		/// Required to improve compatibility between different InputFields (like Unity.UI and TextMesh Pro versions).
		/// </summary>
		protected IInputFieldProxy InputProxy;

		/// <summary>
		/// Input field value.
		/// </summary>
		/// <value>The input value.</value>
		public string InputValue {
			get {
				return InputProxy.text;
			}
			set {
				if (InputValue!=null)
				{
					InputProxy.text = value;
				}
			}
		}

		/// <summary>
		/// OnSelect event.
		/// </summary>
		public ListViewEvent OnSelect = new ListViewEvent();

		Transform _listCanvas;

		/// <summary>
		/// Canvas where ListView placed.
		/// </summary>
		protected Transform ListCanvas {
			get {
				if (_listCanvas==null)
				{
					_listCanvas = Utilites.FindTopmostCanvas(listView.transform.parent);
				}
				return _listCanvas;
			}
		}

		/// <summary>
		/// ListView parent.
		/// </summary>
		protected Transform ListViewParent;

		[System.NonSerialized]
		bool isComboboxInited;

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
			if (isComboboxInited)
			{
				return ;
			}
			isComboboxInited = true;

			InitInputFieldProxy();

			SetToggleButton(toggleButton);

			SetListView(listView);

			if (listView!=null)
			{
				listView.gameObject.SetActive(true);
				listView.Init();
				if (listView.SelectedIndex!=-1)
				{
					InputValue = listView.DataSource[listView.SelectedIndex];
				}
				listView.gameObject.SetActive(false);
			}
		}

		/// <summary>
		/// Init InputFieldProxy.
		/// </summary>
		protected virtual void InitInputFieldProxy()
		{
			InputProxy = new InputFieldProxy(GetComponent<InputField>());
			InputProxy.interactable = editable;
			InputProxy.onEndEdit.AddListener(InputItem);
		}

		/// <summary>
		/// Sets the list view.
		/// </summary>
		/// <param name="value">Value.</param>
		protected virtual void SetListView(ListView value)
		{
			if (listView!=null)
			{
				ListViewParent = null;
				
				listView.OnSelectString.RemoveListener(SelectItem);
				listView.OnFocusOut.RemoveListener(onFocusHideList);
				
				listView.onCancel.RemoveListener(OnListViewCancel);
				listView.onItemCancel.RemoveListener(OnListViewCancel);
				
				RemoveDeselectCallbacks();
			}
			listView = value;
			if (listView!=null)
			{
				ListViewParent = listView.transform.parent;
				
				listView.OnSelectString.AddListener(SelectItem);
				listView.OnFocusOut.AddListener(onFocusHideList);
				
				listView.onCancel.AddListener(OnListViewCancel);
				listView.onItemCancel.AddListener(OnListViewCancel);
				
				AddDeselectCallbacks();
			}
		}

		/// <summary>
		/// Sets the toggle button.
		/// </summary>
		/// <param name="value">Value.</param>
		protected virtual void SetToggleButton(Button value)
		{
			if (toggleButton!=null)
			{
				toggleButton.onClick.RemoveListener(ToggleList);
			}
			toggleButton = value;
			if (toggleButton!=null)
			{
				toggleButton.onClick.AddListener(ToggleList);
			}
		}

		/// <summary>
		/// Clear listview and selected item.
		/// </summary>
		public virtual void Clear()
		{
			listView.DataSource.Clear();
			InputValue = string.Empty;
		}

		/// <summary>
		/// Toggles the list visibility.
		/// </summary>
		public virtual void ToggleList()
		{
			if (listView==null)
			{
				return ;
			}
			if (listView.gameObject.activeSelf)
			{
				HideList();
			}
			else
			{
				ShowList();
			}
		}

		/// <summary>
		/// The modal key.
		/// </summary>
		protected int? ModalKey;

		/// <summary>
		/// Shows the list.
		/// </summary>
		public virtual void ShowList()
		{
			if (listView==null)
			{
				return ;
			}

			listView.gameObject.SetActive(true);

			ModalKey = ModalHelper.Open(this, null, new Color(0, 0, 0, 0f), HideList);

			if (ListCanvas!=null)
			{
				ListViewParent = listView.transform.parent;
				listView.transform.SetParent(ListCanvas);
			}

			if (listView.Layout!=null)
			{
				listView.Layout.UpdateLayout();
			}

			if (listView.SelectComponent())
			{
				SetChildDeselectListener(EventSystem.current.currentSelectedGameObject);
			}
			else
			{
				EventSystem.current.SetSelectedGameObject(listView.gameObject);
			}
		}

		/// <summary>
		/// Hides the list.
		/// </summary>
		public virtual void HideList()
		{
			if (ModalKey!=null)
			{
				ModalHelper.Close((int)ModalKey);
				ModalKey = null;
			}

			if (listView==null)
			{
				return ;
			}
			if (ListCanvas!=null)
			{
				listView.transform.SetParent(ListViewParent);
			}
			listView.gameObject.SetActive(false);
		}

		/// <summary>
		/// The children deselect.
		/// </summary>
		protected List<SelectListener> childrenDeselect = new List<SelectListener>();

		/// <summary>
		/// Hide list when focus lost.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		protected void onFocusHideList(BaseEventData eventData)
		{
			if (eventData.selectedObject==gameObject)
			{
				return ;
			}

			var ev_item = eventData as ListViewItemEventData;
			if (ev_item!=null)
			{
				if (ev_item.NewSelectedObject!=null)
				{
					SetChildDeselectListener(ev_item.NewSelectedObject);
				}
				return ;
			}

			var ev = eventData as PointerEventData;
			if (ev==null)
			{
				HideList();
				return ;
			}

			var go = ev.pointerPressRaycast.gameObject;//ev.pointerEnter
			if (go==null)
			{
				return ;
			}

			if (go.Equals(toggleButton.gameObject))
			{
				return ;
			}
			if (go.transform.IsChildOf(listView.transform))
			{
				SetChildDeselectListener(go);
				return ;
			}
			
			HideList();
		}

		/// <summary>
		/// Sets the child deselect listener.
		/// </summary>
		/// <param name="child">Child.</param>
		protected void SetChildDeselectListener(GameObject child)
		{
			var deselectListener = GetDeselectListener(child);
			if (!childrenDeselect.Contains(deselectListener))
			{
				deselectListener.onDeselect.AddListener(onFocusHideList);
				childrenDeselect.Add(deselectListener);
			}
		}

		/// <summary>
		/// Gets the deselect listener.
		/// </summary>
		/// <returns>The deselect listener.</returns>
		/// <param name="go">Go.</param>
		static protected SelectListener GetDeselectListener(GameObject go)
		{
			return Utilites.GetOrAddComponent<SelectListener>(go);
		}

		/// <summary>
		/// Adds the deselect callbacks.
		/// </summary>
		protected void AddDeselectCallbacks()
		{
			if (listView.ScrollRect==null)
			{
				return ;
			}

			if (listView.ScrollRect.verticalScrollbar==null)
			{
				return ;
			}

			var scrollbar = listView.ScrollRect.verticalScrollbar.gameObject;
			var deselectListener = GetDeselectListener(scrollbar);

			deselectListener.onDeselect.AddListener(onFocusHideList);
			childrenDeselect.Add(deselectListener);
		}

		/// <summary>
		/// Removes the deselect callbacks.
		/// </summary>
		protected void RemoveDeselectCallbacks()
		{
			childrenDeselect.ForEach(RemoveDeselectCallback);
			childrenDeselect.Clear();
		}

		/// <summary>
		/// Removes the deselect callback.
		/// </summary>
		/// <param name="listener">Listener.</param>
		protected void RemoveDeselectCallback(SelectListener listener)
		{
			if (listener!=null)
			{
				listener.onDeselect.RemoveListener(onFocusHideList);
			}
		}

		/// <summary>
		/// Set the specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <param name="allowDuplicate">If set to <c>true</c> allow duplicate.</param>
		/// <returns>Index of item.</returns>
		public virtual int Set(string item, bool allowDuplicate=true)
		{
			return listView.Set(item, allowDuplicate);
		}

		/// <summary>
		/// Selects the item.
		/// </summary>
		/// <param name="index">Index.</param>
		/// <param name="text">Text.</param>
		protected virtual void SelectItem(int index, string text)
		{
			InputValue = text;

			HideList();

			if ((EventSystem.current!=null) && (!EventSystem.current.alreadySelecting))
			{
				EventSystem.current.SetSelectedGameObject(toggleButton.gameObject);
			}

			OnSelect.Invoke(index, text);
		}

		/// <summary>
		/// Work with input.
		/// </summary>
		/// <param name="item">Item.</param>
		protected virtual void InputItem(string item)
		{
			if (!editable)
			{
				return ;
			}
			if (string.IsNullOrEmpty(item))
			{
				return ;
			}

			if (!listView.DataSource.Contains(item))
			{
				var index = listView.Add(item);
				listView.Select(index);
			}
			else
			{
				var index = listView.FindIndex(item);
				listView.Select(index);
			}
		}

		/// <summary>
		/// This function is called when the MonoBehaviour will be destroyed.
		/// </summary>
		protected virtual void OnDestroy()
		{
			ListView = null;
			ToggleButton = null;
			if (InputProxy!=null)
			{
				InputProxy.onEndEdit.RemoveListener(InputItem);
			}
		}

		/// <summary>
		/// Hide list view.
		/// </summary>
		protected void OnListViewCancel()
		{
			HideList();
		}

		/// <summary>
		/// Called when OnSubmit event occurs.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnSubmit(BaseEventData eventData)
		{
			ShowList();
		}
	}
}