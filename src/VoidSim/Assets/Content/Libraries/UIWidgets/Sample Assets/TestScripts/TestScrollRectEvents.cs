﻿using UnityEngine;
using System.Collections;
using UIWidgets;

namespace UIWidgetsSamples
{
	/// <summary>
	/// Test ScrollRect Events.
	/// </summary>
	[RequireComponent(typeof(ScrollRectEvents))]
	public class TestScrollRectEvents : MonoBehaviour
	{
		/// <summary>
		/// ListView.
		/// </summary>
		[SerializeField]
		public ListViewIcons ListView;

		/// <summary>
		/// Data
		/// </summary>
		protected ObservableList<ListViewIconsItemDescription> Data;

		bool isInited = false;

		/// <summary>
		/// Start this instance.
		/// </summary>
		protected virtual void Start()
		{
			Init();
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		protected virtual void Init()
		{
			if (isInited)
			{
				return ;
			}
			isInited = true;

			ListView.Sort = false;
			Data = ListView.DataSource;
			Data.Comparison = null;
			ListView.Init();

			var scrollRectEvents = GetComponent<ScrollRectEvents>();
			if (scrollRectEvents!=null)
			{
				scrollRectEvents.OnPullUp.AddListener(Refresh);
				scrollRectEvents.OnPullDown.AddListener(LoadMore);
			}
		}

		/// <summary>
		/// Handle enable event.
		/// </summary>
		protected virtual void OnEnable()
		{
			Init();
			StartCoroutine(LoadData(0));
		}

		/// <summary>
		/// Load data from url.
		/// </summary>
		/// <param name="start">Start index.</param>
		/// <returns>Coroutine.</returns>
		protected virtual IEnumerator LoadData(int start)
		{
			if (start==0)
			{
				Data.Clear();
			}

			WWW www = new WWW("https://ilih.ru/steamspy/?start=" + start.ToString());
			yield return www;
			
			var lines = www.text.Split('\n');

			www.Dispose();

			Data.BeginUpdate();

			lines.ForEach(ParseLine);
			
			Data.EndUpdate();
		}

		/// <summary>
		/// Parse line.
		/// </summary>
		/// <param name="line">Line.</param>
		protected virtual void ParseLine(string line)
		{
			if (string.IsNullOrEmpty(line))
			{
				return ;
			}
			var info = line.Split('\t');
			
			var item = new ListViewIconsItemDescription(){
				Name = string.Format("{0}. {1}", Data.Count + 1, info[0]),
			};
			Data.Add(item);
		}

		/// <summary>
		/// Load initial data.
		/// </summary>
		public void Refresh()
		{
			StartCoroutine(LoadData(0));
		}

		/// <summary>
		/// Load more data.
		/// </summary>
		public void LoadMore()
		{
			StartCoroutine(LoadData(Data.Count));
		}

		/// <summary>
		/// Remove listeners.
		/// </summary>
		protected virtual void OnDestroy()
		{
			var scrollRectEvents = GetComponent<ScrollRectEvents>();
			if (scrollRectEvents!=null)
			{
				scrollRectEvents.OnPullUp.RemoveListener(Refresh);
				scrollRectEvents.OnPullDown.RemoveListener(LoadMore);
			}
		}
	}
}