﻿using UnityEngine;
using UIWidgets;
using UnityEngine.Serialization;

namespace UIWidgetsSamples
{
	/// <summary>
	/// Test Paginator.
	/// </summary>
	public class TestPaginator : MonoBehaviour
	{
		/// <summary>
		/// ScrollRectPaginator.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("paginator")]
		protected ScrollRectPaginator Paginator;

		/// <summary>
		/// Test.
		/// </summary>
		public void Test()
		{
			// pages count
			Debug.Log(Paginator.Pages);

			// navigate to page
			Paginator.CurrentPage = 2;
		}
	}
}