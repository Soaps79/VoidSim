﻿using UnityEngine;
using System.Collections;
using UIWidgets;

namespace UIWidgetsSamples
{
	/// <summary>
	/// Autocomplete for ListViewIcons.
	/// </summary>
	[RequireComponent(typeof(ListViewIcons))]
	public class AutocompleteRemote : AutocompleteCustom<ListViewIconsItemDescription,ListViewIconsItemComponent,ListViewIcons>
	{
		/// <summary>
		/// Load data from web.
		/// </summary>
		/// <returns>Yield instruction.</returns>
		protected override IEnumerator Search()
		{
			if (SearchDelay > 0)
			{
				if (UnscaledTime)
				{
					yield return StartCoroutine(Utilites.WaitForSecondsUnscaled(SearchDelay));
				}
				else
				{
					yield return new WaitForSeconds(SearchDelay);
				}
			}

			WWW www = new WWW("http://example.com/?search=" + WWW.EscapeURL(Query));
			yield return www;

			DisplayListView.DataSource = Text2List(www.text);

			www.Dispose();

			if (DisplayListView.DataSource.Count > 0)
			{
				ShowOptions();
				DisplayListView.SelectedIndex = 0;
			}
			else
			{
				HideOptions();
			}
		}

		/// <summary>
		/// Convert raw data to list.
		/// </summary>
		/// <param name="text">Raw data.</param>
		/// <returns>Data list.</returns>
		static protected ObservableList<ListViewIconsItemDescription> Text2List(string text)
		{
			var result = new ObservableList<ListViewIconsItemDescription>();

			//convert text to items and add items to list
			foreach (var line in text.Split(new string[] { "\r\n", "\r", "\n" }, System.StringSplitOptions.None))
			{
				result.Add(new ListViewIconsItemDescription() { Name = line.TrimEnd() });
			}

			return result;
		}

		/// <summary>
		/// Determines whether the beginnig of value matches the Input.
		/// </summary>
		/// <param name="value">Value.</param>
		/// <returns>true if beginnig of value matches the Input; otherwise, false.</returns>
		public override bool Startswith(ListViewIconsItemDescription value)
		{
			if (CaseSensitive)
			{
				return value.Name.StartsWith(Query) || (value.LocalizedName!=null && value.LocalizedName.StartsWith(Query));
			}
			return value.Name.ToLower().StartsWith(Query.ToLower()) || (value.LocalizedName!=null && value.LocalizedName.ToLower().StartsWith(Query.ToLower()));
		}

		/// <summary>
		/// Returns a value indicating whether Input occurs within specified value.
		/// </summary>
		/// <param name="value">Value.</param>
		/// <returns>true if the Input occurs within value parameter; otherwise, false.</returns>
		public override bool Contains(ListViewIconsItemDescription value)
		{
			if (CaseSensitive)
			{
				return value.Name.Contains(Query) || (value.LocalizedName!=null && value.LocalizedName.Contains(Query));
			}
			return value.Name.ToLower().Contains(Query.ToLower()) || (value.LocalizedName!=null && value.LocalizedName.ToLower().Contains(Query.ToLower()));
		}

		/// <summary>
		/// Convert value to string.
		/// </summary>
		/// <returns>The string value.</returns>
		/// <param name="value">Value.</param>
		protected override string GetStringValue(ListViewIconsItemDescription value)
		{
			return value.LocalizedName ?? value.Name;
		}
	}
}