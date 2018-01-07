using UnityEngine;
using System;
using System.Collections.Generic;

namespace UIWidgetsSamples
{
	/// <summary>
	/// TreeViewSampleMultipleComponent interface.
	/// </summary>
	public interface ITreeViewSampleMultipleComponent
	{
		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="item">Item.</param>
		void SetData(ITreeViewSampleItem item);

		/// <summary>
		/// Set graphics colors.
		/// </summary>
		/// <param name="foregroundColor">Foreground color.</param>
		/// <param name="backgroundColor">Background color.</param>
		/// <param name="fadeDuration">Fade duration.</param>
		void GraphicsColoring(Color foregroundColor, Color backgroundColor, float fadeDuration);

		/// <summary>
		/// Create component instance.
		/// </summary>
		/// <param name="parent">New parent.</param>
		/// <returns>GroupedListViewComponent instance.</returns>
		ITreeViewSampleMultipleComponent IInstance(Transform parent);

		/// <summary>
		/// Return instance to cache.
		/// </summary>
		/// <param name="parent">New parent.</param>
		void Free(Transform parent);
	}

	/// <summary>
	/// TreeViewSample component multiple.
	/// </summary>
	public class TreeViewSampleComponentMultiple : TreeViewSampleComponent
	{
		/// <summary>
		/// Continent template.
		/// </summary>
		[SerializeField]
		protected TreeViewSampleComponentContinent ContinentTemplate;

		/// <summary>
		/// Country template.
		/// </summary>
		[SerializeField]
		protected TreeViewSampleComponentCountry CountryTemplate;

		/// <summary>
		/// Items component parent.
		/// </summary>
		[SerializeField]
		protected Transform ComponentParent;

		ITreeViewSampleMultipleComponent CurrentComponent;

		/// <summary>
		/// Components templates.
		/// </summary>
		static protected Dictionary<Type,ITreeViewSampleMultipleComponent> Templates;

		/// <summary>
		/// Handle awake event.
		/// </summary>
		protected override void Awake()
		{
			base.Awake();

			if (Templates==null)
			{
				Templates = new Dictionary<Type,ITreeViewSampleMultipleComponent>();
				Templates.Add(typeof(TreeViewSampleItemContinent), ContinentTemplate);
				Templates.Add(typeof(TreeViewSampleItemCountry), CountryTemplate);
			}
		}

		/// <summary>
		/// Update view.
		/// </summary>
		protected override void UpdateView()
		{
			MovedToCache();

			CurrentComponent = Templates[Item.GetType()].IInstance(ComponentParent);
			CurrentComponent.SetData(Item);
		}

		/// <summary>
		/// Graphics coloring.
		/// </summary>
		/// <param name="foregroundColor">Foreground color.</param>
		/// <param name="backgroundColor">Background color.</param>
		/// <param name="fadeDuration">Fade duration.</param>
		public override void GraphicsColoring(Color foregroundColor, Color backgroundColor, float fadeDuration = 0f)
		{
			base.GraphicsColoring(foregroundColor, backgroundColor, fadeDuration);

			if (CurrentComponent!=null)
			{
				CurrentComponent.GraphicsColoring(foregroundColor, backgroundColor, fadeDuration);
			}
		}

		/// <summary>
		/// Called when item moved to cache, you can use it free used resources.
		/// </summary>
		public override void MovedToCache()
		{
			if (CurrentComponent!=null)
			{
				CurrentComponent.Free(ContinentTemplate.transform.parent);
				CurrentComponent = null;
			}
		}
	}
}