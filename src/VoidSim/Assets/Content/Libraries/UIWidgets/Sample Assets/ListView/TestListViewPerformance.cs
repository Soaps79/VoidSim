﻿using UnityEngine;
using System.Linq;
using UIWidgets;
using UnityEngine.Serialization;

namespace UIWidgetsSamples
{
	/// <summary>
	/// Test ListView performance.
	/// </summary>
	public class TestListViewPerformance : MonoBehaviour
	{
		/// <summary>
		/// ListView.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("lv")]
		protected ListView ListView;

		/// <summary>
		/// ListViewIcons.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("lvi")]
		protected ListViewIcons ListViewIcons;

		/// <summary>
		/// Clear ListViewIcons.
		/// </summary>
		public void ListClear()
		{
			ListViewIcons.DestroyGameObjects = true;
			ListViewIcons.DataSource.Clear();
		}

		/// <summary>
		/// Set new list to ListView with specified items count.
		/// </summary>
		/// <param name="n">Items count.</param>
		public void TestN(int n)
		{
			ListView.DataSource = Enumerable.Range(1, n).Select(x => x.ToString("00000")).ToObservableList();
		}

		/// <summary>
		/// Set new list with 2 items.
		/// </summary>
		public void Test2()
		{
			TestN(2);
		}

		/// <summary>
		/// Set new list with 5 items.
		/// </summary>
		public void Test5()
		{
			TestN(5);
		}

		/// <summary>
		/// Set new list with 10 items.
		/// </summary>
		public void Test10()
		{
			TestN(10);
		}

		/// <summary>
		/// Set new list with 100 items.
		/// </summary>
		public void Test100()
		{
			TestN(100);
		}

		/// <summary>
		/// Set new list with 1000 items.
		/// </summary>
		public void Test1000()
		{
			TestN(1000);
		}

		/// <summary>
		/// Set new list with 10000 items.
		/// </summary>
		public void Test10000()
		{
			TestN(10000);
		}

		/// <summary>
		/// Set new list to ListViewIcons with specified items count.
		/// </summary>
		/// <param name="n">Items count.</param>
		public void TestiN(int n)
		{
			var data = Enumerable.Range(1, n).Select(x => new ListViewIconsItemDescription() {
				Name = x.ToString("00000")
			}).ToObservableList();
			ListViewIcons.DataSource = data;
		}

		/// <summary>
		/// Set new list with 2 items.
		/// </summary>
		public void Testi2()
		{
			TestiN(2);
		}

		/// <summary>
		/// Set new list with 5 items.
		/// </summary>
		public void Testi5()
		{
			TestiN(5);
		}

		/// <summary>
		/// Set new list with 1000 items.
		/// </summary>
		public void Testi1000()
		{
			ListViewIcons.SortFunc = null;
			TestiN(1000);
		}

		/// <summary>
		/// Set new list with 10000 items.
		/// </summary>
		public void Testi10000()
		{
			ListViewIcons.SortFunc = null;
			TestiN(10000);
		}
	}
}