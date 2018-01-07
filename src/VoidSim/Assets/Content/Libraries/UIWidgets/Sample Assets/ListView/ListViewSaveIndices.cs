using UnityEngine;
using UIWidgets;
using System.Collections.Generic;
using System.Linq;
 
namespace UIWidgetsSamples
{
	/// <summary>
	/// How to save selected indicies for ListView.
	/// </summary>
	[RequireComponent(typeof(ListViewBase))]
	public class ListViewSaveIndices : MonoBehaviour
	{
		/// <summary>
		/// Key.
		/// </summary>
		[SerializeField]
		public string Key = "Unique Key";
		
		[SerializeField]
		ListViewBase list;
		
		/// <summary>
		/// Load saved indicies and add listeners.
		/// </summary>
		protected virtual void Start()
		{
			list = GetComponent<ListViewBase>();
			list.Init();
			
			LoadIndices();
			
			list.OnSelect.AddListener(SaveIndices);
			list.OnDeselect.AddListener(SaveIndices);
		}

		/// <summary>
		/// Load indicies.
		/// </summary>
		protected virtual void LoadIndices()
		{
			if (PlayerPrefs.HasKey(Key))
			{
				var indices = String2Indices(PlayerPrefs.GetString(Key));
				indices.RemoveAll(x => !list.IsValid(x));
				list.SelectedIndices = indices;
			}
		}

		/// <summary>
		/// Save indicies.
		/// </summary>
		/// <param name="index">Index.</param>
		/// <param name="component">Component.</param>
		protected virtual void SaveIndices(int index, ListViewItem component)
		{
			PlayerPrefs.SetString(Key, Indices2String(list.SelectedIndices));
		}
		
		static List<int> String2Indices(string str)
		{
			if (string.IsNullOrEmpty(str))
			{
				return new List<int>();
			}
			return str.Split(',').Select(x => int.Parse(x)).ToList();
		}
		
		static string Indices2String(List<int> indices)
		{
			if ((indices==null) || (indices.Count==0))
			{
				return string.Empty;
			}
			return string.Join(",", indices.Select(x => x.ToString()).ToArray());
		}
	}
}