﻿using UnityEngine;
using System.Collections.Generic;
using UIWidgets;
using UnityEngine.Serialization;

namespace UIWidgetsSamples
{
	/// <summary>
	/// Test combobox.
	/// </summary>
	public class TestCombobox : MonoBehaviour
	{
		/// <summary>
		/// Combobox.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("combobox")]
		protected Combobox Combobox;

		/// <summary>
		/// Get selected item.
		/// </summary>
		public void GetSelected()
		{
			//Get last selected index
			Debug.Log(Combobox.ListView.SelectedIndex);
			
			//Get selected indices
			var indices = Combobox.ListView.SelectedIndices;
			Debug.Log(string.Join(", ", indices.Convert(x => x.ToString()).ToArray()));
			
			//Get last selected string
			if (Combobox.ListView.SelectedIndex!=-1)
			{
				Debug.Log(Combobox.ListView.DataSource[Combobox.ListView.SelectedIndex]);
			}
			
			//Get selected strings
			var selected_strings = Combobox.ListView.SelectedIndices.Convert(x => Combobox.ListView.DataSource[x]);
			Debug.Log(string.Join(", ", selected_strings.ToArray()));
		}

		/// <summary>
		/// Remove item.
		/// </summary>
		public void Remove()
		{
			//Deleting specified string
			var strings = Combobox.ListView.DataSource;
			strings.RemoveAt(0);
		}

		/// <summary>
		/// Clear item list.
		/// </summary>
		public void Clear()
		{
			//Clear list
			Combobox.Clear();
		}

		/// <summary>
		/// Add item.
		/// </summary>
		public void AddItem()
		{
			//Add string
			Combobox.ListView.DataSource.Add("test string");
		}

		/// <summary>
		/// Add items.
		/// </summary>
		public void AddItems()
		{
			//Add strings
			var new_strings = new List<string>() {
				"test string 1",
				"test string 2",
				"test string 2",
			};
			Combobox.ListView.DataSource.AddRange(new_strings);
			//combobox.Set("aaa");
		}

		/// <summary>
		/// Select item.
		/// </summary>
		public void UpdateSelect()
		{
			//Set selected index
			Combobox.ListView.SelectedIndex = 1;
			//or
			Combobox.ListView.Select(1);
		}
	}
}