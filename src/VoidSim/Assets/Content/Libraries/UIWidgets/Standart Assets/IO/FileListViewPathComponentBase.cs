using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace UIWidgets
{
	/// <summary>
	/// Base class for FileListViewPathComponent.
	/// </summary>
	[RequireComponent(typeof(Image))]
	abstract public class FileListViewPathComponentBase : ComponentPool<FileListViewPathComponentBase>, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
	{
		/// <summary>
		/// Path to displayed directory.
		/// </summary>
		public string FullName {
			get;
			protected set;
		}

		/// <summary>
		/// Parent FileListViewPath.
		/// </summary>
		[HideInInspector]
		public FileListViewPath Owner;

		/// <summary>
		/// Set path.
		/// </summary>
		/// <param name="path">Path.</param>
		abstract public void SetPath(string path);

		/// <summary>
		/// OnPointerDown event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnPointerDown(PointerEventData eventData)
		{
		}

		/// <summary>
		/// OnPointerUp event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnPointerUp(PointerEventData eventData)
		{
		}

		/// <summary>
		/// OnPointerClick event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnPointerClick(PointerEventData eventData)
		{
			if (eventData.button==PointerEventData.InputButton.Left)
			{
				Owner.Open(FullName);
			}
		}
	}
}