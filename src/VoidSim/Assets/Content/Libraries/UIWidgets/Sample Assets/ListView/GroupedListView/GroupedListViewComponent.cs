using System;
using UIWidgets;
using UnityEngine;
using System.Collections.Generic;

namespace UIWidgetsSamples
{
	/// <summary>
	/// Interface for GroupedListViewComponent.
	/// </summary>
	public interface IGroupedListViewComponent
	{
		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="item">Item.</param>
		void SetData(IGroupedListItem item);

		/// <summary>
		/// Graphics coloring.
		/// </summary>
		/// <param name="foreground">Foreground color.</param>
		/// <param name="background">Background color.</param>
		/// <param name="fadeDuration">Fade duration.</param>
		void GraphicsColoring(Color foreground, Color background, float fadeDuration);

		/// <summary>
		/// Create component instance.
		/// </summary>
		/// <param name="parent">New parent.</param>
		/// <returns>GroupedListViewComponent instance.</returns>
		IGroupedListViewComponent IInstance(Transform parent);

		/// <summary>
		/// Return instance to cache.
		/// </summary>
		/// <param name="parent">New parent.</param>
		void Free(Transform parent);
	}

	/// <summary>
	/// GroupedListViewComponent.
	/// </summary>
	public class GroupedListViewComponent : ListViewItem, IViewData<IGroupedListItem>
	{
		/// <summary>
		/// Item group template.
		/// </summary>
		[SerializeField]
		protected GroupedListViewComponentGroup GroupTemplate;

		/// <summary>
		/// Item template.
		/// </summary>
		[SerializeField]
		protected GroupedListViewComponentItem ItemTemplate;

		/// <summary>
		/// Items parent.
		/// </summary>
		[SerializeField]
		protected Transform ComponentParent;

		IGroupedListViewComponent CurrentComponent;

		/// <summary>
		/// Templates.
		/// </summary>
		static protected Dictionary<Type,IGroupedListViewComponent> Templates;

		/// <summary>
		/// Init templates.
		/// </summary>
		protected override void Awake()
		{
			base.Awake();

			if (Templates==null)
			{
				GroupTemplate.gameObject.SetActive(false);
				ItemTemplate.gameObject.SetActive(false);

				Templates = new Dictionary<Type,IGroupedListViewComponent>();
				Templates.Add(typeof(GroupedListGroup), GroupTemplate);
				Templates.Add(typeof(GroupedListItem), ItemTemplate);
			}
		}

		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="item">Item.</param>
		public void SetData(IGroupedListItem item)
		{
			MovedToCache();

			CurrentComponent = Templates[item.GetType()].IInstance(ComponentParent);
			CurrentComponent.SetData(item);
		}

		/// <summary>
		/// Is graphics colors setted?
		/// </summary>
		protected bool IsColorSetted;

		/// <summary>
		/// Set graphics colors.
		/// </summary>
		/// <param name="foregroundColor">Foreground color.</param>
		/// <param name="backgroundColor">Background color.</param>
		/// <param name="fadeDuration">Fade duration.</param>
		public override void GraphicsColoring(Color foregroundColor, Color backgroundColor, float fadeDuration=0f)
		{
			base.GraphicsColoring(foregroundColor, backgroundColor, fadeDuration);

			if (CurrentComponent!=null)
			{
				CurrentComponent.GraphicsColoring(foregroundColor, backgroundColor, fadeDuration);
			}
		}

		/// <summary>
		/// Free current component.
		/// </summary>
		public override void MovedToCache()
		{
			if (CurrentComponent!=null)
			{
				CurrentComponent.Free(GroupTemplate.transform.parent);
				CurrentComponent = null;
			}
		}
	}
}