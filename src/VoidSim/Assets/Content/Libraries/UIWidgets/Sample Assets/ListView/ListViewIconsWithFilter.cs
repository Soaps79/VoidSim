using UnityEngine;
using System.Collections.Generic;
using UIWidgets;

namespace UIWidgetsSamples
{
	/// <summary>
	/// Sample ListViewIcons with filter.
	/// </summary>
	public class ListViewIconsWithFilter : ListViewIcons
	{
		[SerializeField]
		List<ListViewIconsItemDescription> _originalItems = new List<ListViewIconsItemDescription>();
		
		ObservableList<ListViewIconsItemDescription> originalItems;
		
		/// <summary>
		/// All items.
		/// </summary>
		public ObservableList<ListViewIconsItemDescription> OriginalItems {
			get {
				if (originalItems==null)
				{
					originalItems = new ObservableList<ListViewIconsItemDescription>(_originalItems);
					originalItems.OnChange += Filter;
				}
				return originalItems;
			}
			set {
				if (originalItems!=null)
				{
					originalItems.OnChange -= Filter;
				}
				originalItems = value;
				if (originalItems!=null)
				{
					originalItems.OnChange += Filter;
				}
			}
		}
		
		/// <summary>
		/// Search string.
		/// </summary>
		protected string Search = "";

		/// <summary>
		/// Filter data using specified search string.
		/// </summary>
		/// <param name="search">Search string.</param>
		public void Filter(string search)
		{
			Search = search;
			Filter();
		}
		
		/// <summary>
		/// Copy items from OriginalItems to DataSource if it's match specified string.
		/// </summary>
		protected void Filter()
		{
			DataSource.BeginUpdate();
			DataSource.Clear();
			
			if (string.IsNullOrEmpty(Search))
			{
				// if search string not specified add all items
				DataSource.AddRange(OriginalItems);
			}
			else
			{
				// else add items with name startswith specified string
				var finded = OriginalItems.FindAll(x => x.Name.StartsWith(Search));
				DataSource.AddRange(finded);
			}
			
			DataSource.EndUpdate();
		}
		
		/// <summary>
		/// Init this instance.
		/// </summary>
		public override void Init()
		{
			base.Init();
			
			// call Filter() to set initial DataSource
			Filter();
		}
	}
}