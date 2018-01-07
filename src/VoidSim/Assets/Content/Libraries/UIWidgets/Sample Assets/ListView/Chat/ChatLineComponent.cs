using UnityEngine;
using UnityEngine.UI;
using UIWidgets;
using System.Collections.Generic;

namespace UIWidgetsSamples
{
	/// <summary>
	/// ChatLineComponent interface.
	/// </summary>
	public interface IChatLineComponent
	{
		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="item">Item.</param>
		void SetData(ChatLine item);

		/// <summary>
		/// Create component instance.
		/// </summary>
		/// <param name="parent">New parent.</param>
		/// <returns>ChatLineComponent instance.</returns>
		IChatLineComponent IInstance(Transform parent);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="parent">New parent.</param>
		void Free(Transform parent);
	}

	/// <summary>
	/// ChatLine component.
	/// </summary>
	public class ChatLineComponent : ListViewItem, IViewData<ChatLine>
	{
		/// <summary>
		/// Foreground graphics for coloring.
		/// </summary>
		public override Graphic[] GraphicsForeground {
			get {
				return new Graphic[] { };
			}
		}

		/// <summary>
		/// Background graphics for coloring.
		/// </summary>
		public override Graphic[] GraphicsBackground {
			get {
				return new Graphic[] { };
			}
		}

		/// <summary>
		/// Template for incoming message.
		/// </summary>
		[SerializeField]
		protected ChatLineIncoming IncomingTemplate;

		/// <summary>
		/// Template for outgoing message.
		/// </summary>
		[SerializeField]
		protected ChatLineOutgoing OutgoingTemplate;

		/// <summary>
		/// Current component.
		/// </summary>
		public IChatLineComponent CurrentComponent;

		/// <summary>
		/// Templates.
		/// </summary>
		static protected Dictionary<int,IChatLineComponent> Templates = new Dictionary<int, IChatLineComponent>();

		/// <summary>
		/// Init templates.
		/// </summary>
		protected override void Awake()
		{
			base.Awake();

			if (Templates.Count==0)
			{
				Templates.Add((int)ChatLineType.Incoming, IncomingTemplate);
				Templates.Add((int)ChatLineType.Outgoing, OutgoingTemplate);

				IncomingTemplate.gameObject.SetActive(false);
				OutgoingTemplate.gameObject.SetActive(false);
			}
		}

		/// <summary>
		/// Current message type.
		/// </summary>
		protected int CurrentItemType = -1;

		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="item">Item.</param>
		public void SetData(ChatLine item)
		{
			if (CurrentItemType!=(int)item.Type)
			{
				MovedToCache();

				CurrentItemType = (int)item.Type;
				CurrentComponent = Templates[CurrentItemType].IInstance(transform);
			}

			CurrentComponent.SetData(item);
		}

		/// <summary>
		/// Free current component.
		/// </summary>
		public override void MovedToCache()
		{
			if (CurrentComponent!=null)
			{
				CurrentComponent.Free(IncomingTemplate.transform.parent);
				CurrentComponent = null;
			}
		}
	}
}