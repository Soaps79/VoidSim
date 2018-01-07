using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Serialization;
using System;
using System.Collections;

namespace UIWidgets
{
	/// <summary>
	/// Node toggle type.
	/// </summary>
	public enum NodeToggle
	{
		/// <summary>
		/// Rotate.
		/// </summary>
		Rotate = 0,

		/// <summary>
		/// ChangeSprite.
		/// </summary>
		ChangeSprite = 1,
	}
	
	/// <summary>
	/// Node toggle event.
	/// </summary>
	[Serializable]
	public class NodeToggleEvent : UnityEvent<int>
	{
	}

	/// <summary>
	/// Tree view component base.
	/// </summary>
	public class TreeViewComponentBase<T> : ListViewItem, IViewData<ListNode<T>>
	{
		/// <summary>
		/// Foreground graphics for coloring.
		/// </summary>
		public override Graphic[] GraphicsForeground {
			get {
				return new Graphic[] {Text, };
			}
		}

		/// <summary>
		/// The icon.
		/// </summary>
		public Image Icon;
		
		/// <summary>
		/// The text.
		/// </summary>
		public Text Text;

		/// <summary>
		/// The toggle.
		/// </summary>
		public TreeNodeToggle Toggle;

		Image toggleImage;

		/// <summary>
		/// Gets the toggle image.
		/// </summary>
		/// <value>The toggle image.</value>
		protected Image ToggleImage {
			get {
				if (toggleImage==null)
				{
					toggleImage = Toggle.GetComponent<Image>();
				}
				return toggleImage;
			}
		}

		/// <summary>
		/// The toggle event.
		/// </summary>
		public NodeToggleEvent ToggleEvent = new NodeToggleEvent();

		/// <summary>
		/// Indentation.
		/// </summary>
		[FormerlySerializedAs("Filler")]
		public LayoutElement Indentation;

		/// <summary>
		/// Gets or sets the indentation.
		/// </summary>
		/// <value>The indentation.</value>
		[Obsolete("Use Indentation instead.")]
		public LayoutElement Filler {
			get {
				return Indentation;
			}
			set {
				Indentation = value;
			}
		}

		/// <summary>
		/// The on node expand.
		/// </summary>
		public NodeToggle OnNodeExpand = NodeToggle.Rotate;

		/// <summary>
		/// Is need animate arrow?
		/// </summary>
		public bool AnimateArrow;

		/// <summary>
		/// Sprite when node opened.
		/// </summary>
		public Sprite NodeOpened;

		/// <summary>
		/// Sprite when node closed.
		/// </summary>
		public Sprite NodeClosed;

		/// <summary>
		/// The node.
		/// </summary>
		public TreeNode<T> Node {
			get;
			protected set;
		}

		/// <summary>
		/// The padding per level.
		/// </summary>
		public float PaddingPerLevel = 30;

		/// <summary>
		/// Set icon native size.
		/// </summary>
		public bool SetNativeSize = true;

		/// <summary>
		/// Start this instance.
		/// </summary>
		protected override void Start()
		{
			base.Start();
			Toggle.OnClick.AddListener(ToggleNode);
		}

		/// <summary>
		/// This function is called when the MonoBehaviour will be destroyed.
		/// </summary>
		protected override void OnDestroy()
		{
			if (Toggle!=null)
			{
				Toggle.OnClick.RemoveListener(ToggleNode);
			}

			base.OnDestroy();
		}

		/// <summary>
		/// Toggles the node.
		/// </summary>
		protected virtual void ToggleNode()
		{
			if (AnimationCoroutine!=null)
			{
				StopCoroutine(AnimationCoroutine);
			}
			SetToggle(Node.IsExpanded);

			ToggleEvent.Invoke(Index);

			if (OnNodeExpand==NodeToggle.Rotate)
			{
				if (AnimateArrow)
				{
					AnimationCoroutine = Node.IsExpanded ? CloseCoroutine() : OpenCoroutine();
					StartCoroutine(AnimationCoroutine);
				}
			}
			else
			{
				SetToggle(Node.IsExpanded);
			}
		}

		/// <summary>
		/// Ses the toggle sprite.
		/// </summary>
		/// <param name="isExpanded">If set to <c>true</c> is expanded.</param>
		protected virtual void SetToggleSprite(bool isExpanded)
		{
			ToggleImage.sprite = isExpanded ? NodeOpened : NodeClosed;
		}

		/// <summary>
		/// The animation coroutine.
		/// </summary>
		protected IEnumerator AnimationCoroutine;

		/// <summary>
		/// Animate arrow on open.
		/// </summary>
		/// <returns>The coroutine.</returns>
		protected virtual IEnumerator OpenCoroutine()
		{
			var rect = Toggle.transform as RectTransform;
			yield return StartCoroutine(Animations.RotateZ(rect, 0.2f, -90, 0));
		}

		/// <summary>
		/// Animate arrow on close.
		/// </summary>
		/// <returns>The coroutine.</returns>
		protected virtual IEnumerator CloseCoroutine()
		{
			var rect = Toggle.transform as RectTransform;
			yield return StartCoroutine(Animations.RotateZ(rect, 0.2f, 0, -90));
		}

		/// <summary>
		/// Sets the toggle rotation.
		/// </summary>
		/// <param name="isExpanded">If set to <c>true</c> is expanded.</param>
		protected virtual void SetToggleRotation(bool isExpanded)
		{
			if (Toggle==null)
			{
				return ;
			}
			Toggle.transform.localRotation = Quaternion.Euler(0, 0, (isExpanded) ? -90 : 0);
		}

		/// <summary>
		/// Sets the toggle.
		/// </summary>
		/// <param name="isExpanded">If set to <c>true</c> is expanded.</param>
		protected virtual void SetToggle(bool isExpanded)
		{
			if (OnNodeExpand==NodeToggle.Rotate)
			{
				SetToggleRotation(isExpanded);
			}
			else
			{
				SetToggleSprite(isExpanded);
			}
		}

		#region IViewData implementation
		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="item">Item.</param>
		public void SetData(ListNode<T> item)
		{
			SetData(item.Node, item.Depth);
		}
		#endregion

		/// <summary>
		/// Sets the data.
		/// </summary>
		/// <param name="node">Node.</param>
		/// <param name="depth">Depth.</param>
		public virtual void SetData(TreeNode<T> node, int depth)
		{
			if (node!=null)
			{
				Node = node;
				SetToggle(Node.IsExpanded);
			}
			if (Indentation!=null)
			{
				Indentation.minWidth = depth * PaddingPerLevel;
				Indentation.preferredWidth = depth * PaddingPerLevel;
				Indentation.flexibleWidth = 0;
			}

			if ((Toggle!=null) && (Toggle.gameObject!=null))
			{
				var toggle_active = (node!=null) && (node.Nodes!=null) && (node.Nodes.Count>0);
				if (Toggle.gameObject.activeSelf!=toggle_active)
				{
					Toggle.gameObject.SetActive(toggle_active);
				}
			}
		}
	}
}