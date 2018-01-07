using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;
using System;

namespace UIWidgets
{
	/// <summary>
	/// IResizableItem.
	/// </summary>
	public interface IResizableItem
	{
		/// <summary>
		/// Gets the objects to resize.
		/// </summary>
		/// <value>The objects to resize.</value>
		GameObject[] ObjectsToResize {
			get;
		}
	}

	/// <summary>
	/// Resizable header cell info.
	/// </summary>
	[Serializable]
	public class ResizableHeaderCellInfo {

		/// <summary>
		/// The cell RectTransform component.
		/// </summary>
		public RectTransform Rect;

		/// <summary>
		/// The cell LayoutElement component.
		/// </summary>
		public LayoutElement LayoutElement;

		/// <summary>
		/// The cell position.
		/// </summary>
		public int Position;

		/// <summary>
		/// Gets the cell width.
		/// </summary>
		/// <value>The width.</value>
		public float Width {
			get {
				return Rect.rect.width;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this gameobject active self.
		/// </summary>
		/// <value><c>true</c> if active self; otherwise, <c>false</c>.</value>
		public bool ActiveSelf {
			get {
				return Rect.gameObject.activeSelf;
			}
		}
	}

	/// <summary>
	/// ResizableHeader.
	/// </summary>
	[AddComponentMenu("UI/UIWidgets/ResizableHeader")]
	[RequireComponent(typeof(LayoutGroup))]
	public class ResizableHeader : MonoBehaviour, IDropSupport<ResizableHeaderDragCell>, IPointerEnterHandler, IPointerExitHandler
	{
		/// <summary>
		/// ListView instance.
		/// </summary>
		[SerializeField]
		public ListViewBase List;

		/// <summary>
		/// Allow resize.
		/// </summary>
		[SerializeField]
		public bool AllowResize = true;

		/// <summary>
		/// Allow reorder.
		/// </summary>
		[SerializeField]
		public bool AllowReorder = true;

		/// <summary>
		/// Is now processed cell reorder?
		/// </summary>
		[System.NonSerialized]
		[HideInInspector]
		public bool ProcessCellReorder = false;

		/// <summary>
		/// Update ListView columns width on drag.
		/// </summary>
		[SerializeField]
		public bool OnDragUpdate = true;

		/// <summary>
		/// The active region in points from left or right border where resize allowed.
		/// </summary>
		[SerializeField]
		public float ActiveRegion = 5;

		/// <summary>
		/// The current camera. For Screen Space - Overlay let it empty.
		/// </summary>
		[SerializeField]
		public Camera CurrentCamera;

		/// <summary>
		/// The cursor texture.
		/// </summary>
		[SerializeField]
		public Texture2D CursorTexture;

		/// <summary>
		/// The cursor hot spot.
		/// </summary>
		[SerializeField]
		public Vector2 CursorHotSpot = new Vector2(16, 16);

		/// <summary>
		/// The cursor texture.
		/// </summary>
		[SerializeField]
		public Texture2D AllowDropCursor;
		
		/// <summary>
		/// The cursor hot spot.
		/// </summary>
		[SerializeField]
		public Vector2 AllowDropCursorHotSpot = new Vector2(4, 2);
		
		/// <summary>
		/// The cursor texture.
		/// </summary>
		[SerializeField]
		public Texture2D DeniedDropCursor;
		
		/// <summary>
		/// The cursor hot spot.
		/// </summary>
		[SerializeField]
		public Vector2 DeniedDropCursorHotSpot = new Vector2(4, 2);

		/// <summary>
		/// The default cursor texture.
		/// </summary>
		[SerializeField]
		public Texture2D DefaultCursorTexture;

		/// <summary>
		/// The default cursor hot spot.
		/// </summary>
		[SerializeField]
		public Vector2 DefaultCursorHotSpot;

		RectTransform rectTransform;
		
		/// <summary>
		/// Gets the rect transform.
		/// </summary>
		/// <value>The rect transform.</value>
		public RectTransform RectTransform {
			get {
				if (rectTransform==null)
				{
					rectTransform = transform as RectTransform;
				}
				return rectTransform;
			}
		}

		/// <summary>
		/// The cells info.
		/// </summary>
		protected List<ResizableHeaderCellInfo> CellsInfo = new List<ResizableHeaderCellInfo>();

		/// <summary>
		/// Header cells.
		/// </summary>
		/// <value>The cells.</value>
		public RectTransform[] Cells {
			get {
				return CellsInfo.Select(x => x.Rect).ToArray();
			}
		}

		Canvas canvas;

		RectTransform canvasRect;

		LayoutElement leftTarget;
		LayoutElement rightTarget;
		bool processDrag;

		LayoutGroup layout;

		/// <summary>
		/// Start this instance.
		/// </summary>
		protected virtual void Start()
		{
			Refresh();
		}

		/// <summary>
		/// Raises the initialize potential drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnInitializePotentialDrag(PointerEventData eventData)
		{
			//Init();
		}

		public void RestoreOrder()
		{
			// restore order
			if ((List!=null) && (CellsInfo!=null) && (CellsInfo.Count > 1))
			{
				//restore header cells order
				CellsInfo.ForEach((x, i) => {
					x.Position = i;
					x.Rect.SetSiblingIndex(i);
				});

				// restore list components cells order
				List.Init();
				List.ForEachComponent(component => {
					var resizable_item = component as IResizableItem;
					if (resizable_item==null)
					{
						return ;
					}
					var objects = resizable_item.ObjectsToResize;
					objects.ForEach((x, i) => x.transform.SetSiblingIndex(i));
				});
			}
		}

		/// <summary>
		/// Reinit this instance in case if you remove or add cells manually.
		/// </summary>
		public void Reinit()
		{
			RestoreOrder();

			// clear cells list
			CellsInfo.Clear();

			// clear cell settings and events
			foreach (Transform child in transform)
			{
				transform.gameObject.SetActive(true);

				var cell = Utilites.GetOrAddComponent<ResizableHeaderDragCell>(child);
				cell.Position = -1;

				var events = Utilites.GetOrAddComponent<ResizableHeaderCell>(child);
				events.OnInitializePotentialDragEvent.RemoveListener(OnInitializePotentialDrag);
				events.OnBeginDragEvent.RemoveListener(OnBeginDrag);
				events.OnDragEvent.RemoveListener(OnDrag);
				events.OnEndDragEvent.RemoveListener(OnEndDrag);
			}

			Refresh();
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		public void Init()
		{
			canvasRect = Utilites.FindTopmostCanvas(transform) as RectTransform;
			canvas = canvasRect.GetComponent<Canvas>();
			
			foreach (Transform child in transform)
			{
				var cell = Utilites.GetOrAddComponent<ResizableHeaderDragCell>(child);

				if (cell.Position==-1)
				{
					cell.Position = CellsInfo.Count;
					cell.ResizableHeader = this;
					cell.AllowDropCursor = AllowDropCursor;
					cell.AllowDropCursorHotSpot = AllowDropCursorHotSpot;
					cell.DeniedDropCursor = DeniedDropCursor;
					cell.DeniedDropCursorHotSpot = DeniedDropCursorHotSpot;

					var events = Utilites.GetOrAddComponent<ResizableHeaderCell>(child);
					events.OnInitializePotentialDragEvent.AddListener(OnInitializePotentialDrag);
					events.OnBeginDragEvent.AddListener(OnBeginDrag);
					events.OnDragEvent.AddListener(OnDrag);
					events.OnEndDragEvent.AddListener(OnEndDrag);

					CellsInfo.Add(new ResizableHeaderCellInfo(){
						Rect = child as RectTransform,
						LayoutElement = Utilites.GetOrAddComponent<LayoutElement>(child),
						Position = CellsInfo.Count
					});
				}
			}
		}

		bool inActiveRegion;

		/// <summary>
		/// Gets a value indicating whether mouse position in active region.
		/// </summary>
		/// <value><c>true</c> if in active region; otherwise, <c>false</c>.</value>
		public bool InActiveRegion {
			get {
				return inActiveRegion;
			}
		}

		/// <summary>
		/// Is cursor over gameobject?
		/// </summary>
		protected bool IsCursorOver;

		/// <summary>
		/// Called by a BaseInputModule when an OnPointerEnter event occurs.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnPointerEnter(PointerEventData eventData)
		{
			IsCursorOver = true;
		}

		/// <summary>
		/// Called by a BaseInputModule when an OnPointerExit event occurs.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnPointerExit(PointerEventData eventData)
		{
			IsCursorOver = false;

			cursorChanged = false;
			Cursor.SetCursor(DefaultCursorTexture, DefaultCursorHotSpot, Utilites.GetCursorMode());
		}

		/// <summary>
		/// Is cursor changed?
		/// </summary>
		protected bool cursorChanged;

		/// <summary>
		/// Update cursors.
		/// </summary>
		protected virtual void LateUpdate()
		{
			if (!AllowResize)
			{
				return ;
			}
			if (!IsCursorOver)
			{
				return ;
			}
			if (processDrag || ProcessCellReorder)
			{
				return ;
			}
			if ((CursorTexture==null) || (!Input.mousePresent))
			{
				return ;
			}

			inActiveRegion = CheckInActiveRegion(Input.mousePosition, CurrentCamera);
			if (inActiveRegion)
			{
				Cursor.SetCursor(CursorTexture, CursorHotSpot, Utilites.GetCursorMode());
				cursorChanged = true;
			}
			else if (cursorChanged)
			{
				Cursor.SetCursor(DefaultCursorTexture, DefaultCursorHotSpot, Utilites.GetCursorMode());
				cursorChanged = false;
			}
		}

		/// <summary>
		/// Check if cursor in active region to resize.
		/// </summary>
		/// <param name="position">Cursor position.</param>
		/// <param name="currentCamera">Current camera.</param>
		/// <returns>true if cursor in active region to resize; otherwise, false.</returns>
		protected virtual bool CheckInActiveRegion(Vector2 position, Camera currentCamera)
		{
			Vector2 point;

			if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(RectTransform, position, currentCamera, out point))
			{
				return false;
			}

			var rect = RectTransform.rect;
			if (!rect.Contains(point))
			{
				return false;
			}
			
			point += new Vector2(rect.width * RectTransform.pivot.x, 0);

			bool in_active_region = false;
			int i = 0;
			foreach (var cell in CellsInfo)
			{
				if (!cell.ActiveSelf)
				{
					i++;
					continue;
				}

				var left = GetTargetIndex(i, -1);
				if (left!=-1)
				{
					in_active_region = CheckLeft(cell.Rect, point);
					if (in_active_region)
					{
						break;
					}
				}

				var right = GetTargetIndex(i, +1);
				if (right!=-1)
				{
					in_active_region = CheckRight(cell.Rect, point);
					if (in_active_region)
					{
						break;
					}
				}
				
				i++;
			}

			return in_active_region;
		}

		float widthLimit;

		/// <summary>
		/// Raises the begin drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnBeginDrag(PointerEventData eventData)
		{
			if (!AllowResize)
			{
				return ;
			}
			if (ProcessCellReorder)
			{
				return ;
			}

			Vector2 point;
			processDrag = false;

			if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(RectTransform, eventData.pressPosition, eventData.pressEventCamera, out point))
			{
				return ;
			}
			
			var r = RectTransform.rect;
			point += new Vector2(r.width * RectTransform.pivot.x, 0);
			
			int i = 0;
			foreach (var cell in CellsInfo)
			{
				if (!cell.ActiveSelf)
				{
					i++;
					continue ;
				}

				processDrag = CheckLeft(cell.Rect, point);
				if (processDrag)
				{
					var left = GetTargetIndex(i, -1);
					if (left==-1)
					{
						processDrag = false;
					}
					else
					{
						leftTarget = CellsInfo[left].LayoutElement;
						rightTarget = CellsInfo[i].LayoutElement;
						widthLimit = CellsInfo[left].Width + CellsInfo[i].Width;
						break;
					}
				}

				processDrag = CheckRight(cell.Rect, point);
				if (processDrag)
				{
					var right = GetTargetIndex(i, +1);
					if (right==-1)
					{
						processDrag = false;
					}
					else
					{
						leftTarget = CellsInfo[i].LayoutElement;
						rightTarget = CellsInfo[right].LayoutElement;
						widthLimit = CellsInfo[i].Width + CellsInfo[right].Width;
						break;
					}
				}

				i++;
			}
		}

		int GetTargetIndex(int index, int direction)
		{
			if ((index + direction)==-1)
			{
				return -1;
			}
			if ((index + direction)==CellsInfo.Count)
			{
				return -1;
			}

			var is_active = CellsInfo[index + direction].ActiveSelf;

			var result = is_active
				? index + direction
				: GetTargetIndex(index + direction, direction);
			
			return result;
		}

		/// <summary>
		/// Checks if point in the left region.
		/// </summary>
		/// <returns><c>true</c>, if point in the left region, <c>false</c> otherwise.</returns>
		/// <param name="childRectTransform">RectTransform.</param>
		/// <param name="point">Point.</param>
		bool CheckLeft(RectTransform childRectTransform, Vector2 point)
		{
			var r = childRectTransform.rect;
			r.position += new Vector2(childRectTransform.anchoredPosition.x, 0);
			r.width = ActiveRegion;

			return r.Contains(point);
		}

		/// <summary>
		/// Checks if point in the right region.
		/// </summary>
		/// <returns><c>true</c>, if right was checked, <c>false</c> otherwise.</returns>
		/// <param name="childRectTransform">Child rect transform.</param>
		/// <param name="point">Point.</param>
		bool CheckRight(RectTransform childRectTransform, Vector2 point)
		{
			var r = childRectTransform.rect;
			
			r.position += new Vector2(childRectTransform.anchoredPosition.x, 0);
			r.position = new Vector2(r.position.x + r.width - ActiveRegion - 1, r.position.y);
			r.width = ActiveRegion + 1;
			
			return r.Contains(point);
		}

		/// <summary>
		/// Raises the end drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnEndDrag(PointerEventData eventData)
		{
			if (!processDrag)
			{
				return ;
			}

			Cursor.SetCursor(DefaultCursorTexture, DefaultCursorHotSpot, Utilites.GetCursorMode());

			ResetChildren();
			if (!OnDragUpdate)
			{
				Resize();
			}
			processDrag = false;
		}

		/// <summary>
		/// Raises the drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnDrag(PointerEventData eventData)
		{
			if (!processDrag)
			{
				return ;
			}
			if (canvas==null)
			{
				throw new MissingComponentException(gameObject.name + " not in Canvas hierarchy.");
			}
			Cursor.SetCursor(CursorTexture, CursorHotSpot, Utilites.GetCursorMode());

			Vector2 p1;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(RectTransform, eventData.position, CurrentCamera, out p1);
			Vector2 p2;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(RectTransform, eventData.position - eventData.delta, CurrentCamera, out p2);
			var delta = p1 - p2;

			if (delta.x > 0)
			{
				leftTarget.preferredWidth = Mathf.Min(leftTarget.preferredWidth + delta.x, widthLimit - rightTarget.minWidth);
				rightTarget.preferredWidth = widthLimit - leftTarget.preferredWidth;
			}
			else
			{
				rightTarget.preferredWidth = Mathf.Min(rightTarget.preferredWidth - delta.x, widthLimit - leftTarget.minWidth);
				leftTarget.preferredWidth = widthLimit - rightTarget.preferredWidth;
			}

			LayoutUtilites.UpdateLayout(layout);

			if (OnDragUpdate)
			{
				Resize();
			}
		}

		/// <summary>
		/// Resets the children widths.
		/// </summary>
		void ResetChildren()
		{
			CellsInfo.ForEach(ResetChildrenWidth);
		}

		void ResetChildrenWidth(ResizableHeaderCellInfo cell, int index)
		{
			cell.LayoutElement.preferredWidth = CellsInfo[index].Rect.rect.width;
		}

		/// <summary>
		/// Resize items in ListView.
		/// </summary>
		public void Resize()
		{
			if (List==null)
			{
				return ;
			}
			if (CellsInfo.Count < 2)
			{
				return ;
			}

			List.Init();
			List.ForEachComponent(ResizeComponent);
		}

		/// <summary>
		/// Resizes the game object.
		/// </summary>
		/// <param name="go">Game object.</param>
		/// <param name="index">The index.</param>
		void ResizeGameObject(GameObject go, int index)
		{
			(go.transform as RectTransform).SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, CellsInfo[index].Width);

			var layoutElement = go.GetComponent<LayoutElement>();
			if (layoutElement)
			{
				layoutElement.minWidth = CellsInfo[index].LayoutElement.minWidth;
				layoutElement.flexibleWidth = CellsInfo[index].LayoutElement.flexibleWidth;
				layoutElement.preferredWidth = CellsInfo[index].Width;
			}
		}

		/// <summary>
		/// Resizes the component.
		/// </summary>
		/// <param name="component">Component.</param>
		void ResizeComponent(ListViewItem component)
		{
			var resizable_item = component as IResizableItem;
			if (resizable_item!=null)
			{
				resizable_item.ObjectsToResize.ForEach(ResizeGameObject);
			}
		}

		void ReorderComponent(ListViewItem component)
		{
			var resizable_item = component as IResizableItem;
			if (resizable_item!=null)
			{
				var objects = resizable_item.ObjectsToResize;
				int i = 0;
				foreach (var cell in CellsInfo)
				{
					objects[i].transform.SetSiblingIndex(cell.Position);
					i++;
				}
			}
		}

		/// <summary>
		/// Move column from oldColumnPosition to newColumnPosition.
		/// </summary>
		/// <param name="oldColumnPosition">Old column position.</param>
		/// <param name="newColumnPosition">New column position.</param>
		public void Reorder(int oldColumnPosition, int newColumnPosition)
		{
			CellsInfo[oldColumnPosition].Rect.SetSiblingIndex(CellsInfo[newColumnPosition].Rect.GetSiblingIndex());
			CellsInfo.ForEach(x => x.Position = x.Rect.GetSiblingIndex());

			if (List==null)
			{
				return ;
			}
			if (CellsInfo.Count < 2)
			{
				return ;
			}
			List.Init();
			List.ForEachComponent(ReorderComponent);
		}

		#region IDropSupport<ResizableHeaderCell>
		/// <summary>
		/// Determines whether this instance can receive drop with the specified data and eventData.
		/// </summary>
		/// <returns><c>true</c> if this instance can receive drop with the specified data and eventData; otherwise, <c>false</c>.</returns>
		/// <param name="data">Cell.</param>
		/// <param name="eventData">Event data.</param>
		public bool CanReceiveDrop(ResizableHeaderDragCell data, PointerEventData eventData)
		{
			if (!AllowReorder)
			{
				return false;
			}
			var target = FindTarget(eventData);
			return target!=null && target!=data;
		}

		/// <summary>
		/// Handle dropped data.
		/// </summary>
		/// <param name="data">Cell.</param>
		/// <param name="eventData">Event data.</param>
		public void Drop(ResizableHeaderDragCell data, PointerEventData eventData)
		{
			var target = FindTarget(eventData);

			Reorder(data.Position, target.Position);
		}

		/// <summary>
		/// Handle canceled drop.
		/// </summary>
		/// <param name="data">Cell.</param>
		/// <param name="eventData">Event data.</param>
		public void DropCanceled(ResizableHeaderDragCell data, PointerEventData eventData)
		{
		}

		/// <summary>
		/// Change value position in specified list.
		/// </summary>
		/// <typeparam name="T">Type.</typeparam>
		/// <param name="list">List.</param>
		/// <param name="oldPosition">Old position.</param>
		/// <param name="newPosition">New position.</param>
		protected static void ChangePosition<T>(List<T> list, int oldPosition, int newPosition)
		{
			var item = list[oldPosition];
			list.RemoveAt(oldPosition);
			list.Insert(newPosition, item);
		}

		List<RaycastResult> raycastResults = new List<RaycastResult>();

		/// <summary>
		/// Get ResizableHeaderDragCell in position specified with event data.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		/// <returns>ResizableHeaderDragCell if found; otherwise null.</returns>
		protected ResizableHeaderDragCell FindTarget(PointerEventData eventData)
		{
			raycastResults.Clear();
			
			EventSystem.current.RaycastAll(eventData, raycastResults);
			
			foreach (var raycastResult in raycastResults)
			{
				if (!raycastResult.isValid)
				{
					continue ;
				}
				
				#if UNITY_4_6 || UNITY_4_7
				var target = raycastResult.gameObject.GetComponent(typeof(ResizableHeaderDragCell)) as ResizableHeaderDragCell;
				#else
				var target = raycastResult.gameObject.GetComponent<ResizableHeaderDragCell>();
				#endif
				if ((target!=null) && target.transform.IsChildOf(transform))
				{
					return target;
				}
			}
			return null;
		}
		#endregion

		/// <summary>
		/// Remove listeners.
		/// </summary>
		protected virtual void OnDestroy()
		{
			CellsInfo.Clear();
			foreach (Transform child in transform)
			{
				var events = child.GetComponent<ResizableHeaderCell>();
				if (events==null)
				{
					continue ;
				}
				events.OnInitializePotentialDragEvent.RemoveListener(OnInitializePotentialDrag);
				events.OnBeginDragEvent.RemoveListener(OnBeginDrag);
				events.OnDragEvent.RemoveListener(OnDrag);
				events.OnEndDragEvent.RemoveListener(OnEndDrag);
			}
		}

		/// <summary>
		/// Refresh header.
		/// </summary>
		public void Refresh()
		{
			if (layout==null)
			{
				layout = GetComponent<LayoutGroup>();
			}
			if (layout!=null)
			{
				LayoutUtilites.UpdateLayout(layout);
			}

			Init();
			Resize();
		}

		/// <summary>
		/// Change column state.
		/// </summary>
		/// <param name="index">Index.</param>
		/// <param name="active">If set state to active.</param>
		public void ColumnToggle(int index, bool active)
		{
			CellsInfo[index].Rect.gameObject.SetActive(active);

			List.ForEachComponent(component => {
				var a = component as IResizableItem;
				if (a!=null)
				{
					a.ObjectsToResize[index].SetActive(active);
				}
			});

			List.ComponentsColoring();

			Refresh();
		}

		/// <summary>
		/// Disable column.
		/// </summary>
		/// <param name="index">Index.</param>
		public void ColumnDisable(int index)
		{
			ColumnToggle(index, false);
		}

		/// <summary>
		/// Enable column.
		/// </summary>
		/// <param name="index">Index.</param>
		public void ColumnEnable(int index)
		{
			ColumnToggle(index, true);
		}

		/// <summary>
		/// Add header cell.
		/// </summary>
		/// <param name="cell">Cell.</param>
		public void AddCell(GameObject cell)
		{
			cell.transform.SetParent(transform, false);
			cell.SetActive(true);

			Refresh();
		}

		/// <summary>
		/// Remove header cell.
		/// </summary>
		/// <param name="cell">Cell.</param>
		public void RemoveCell(GameObject cell, RectTransform parent = null)
		{
			var index = CellsInfo.FindIndex(x => x.Rect.gameObject==cell);
			var cell_info = CellsInfo[index];
			if (index==-1)
			{
				Debug.LogWarning("Cell not in header", cell);
				return ;
			}

			cell.SetActive(false);
			cell.transform.SetParent(parent, false);
			if (parent==null)
			{
				Destroy(cell);
			}

			//remove from cells
			CellsInfo.RemoveAt(index);

			//remove events
			var events = Utilites.GetOrAddComponent<ResizableHeaderCell>(cell);
			events.OnInitializePotentialDragEvent.RemoveListener(OnInitializePotentialDrag);
			events.OnBeginDragEvent.RemoveListener(OnBeginDrag);
			events.OnDragEvent.RemoveListener(OnDrag);
			events.OnEndDragEvent.RemoveListener(OnEndDrag);

			//decrease position for cells where >cell_position
			CellsInfo.ForEach(x => {
				if (x.Position > cell_info.Position)
				{
					x.Position -= 1;
				}
			});

			//update list widths
			LayoutUtilites.UpdateLayout(layout);
			Resize();
		}
	}
}