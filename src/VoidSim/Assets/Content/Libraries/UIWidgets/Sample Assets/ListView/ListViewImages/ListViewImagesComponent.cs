﻿using UnityEngine;
using UnityEngine.UI;
using UIWidgets;
using System.Collections.Generic;
using System.Collections;

namespace UIWidgetsSamples
{
	/// <summary>
	/// ListViewImages component.
	/// </summary>
	public class ListViewImagesComponent : ListViewItem, IViewData<ListViewImagesItem>
	{
		/// <summary>
		/// Url.
		/// </summary>
		[SerializeField]
		public Text Url;

		/// <summary>
		/// Image.
		/// </summary>
		[SerializeField]
		public RawImage Image;

		/// <summary>
		/// Image LayoutElement.
		/// </summary>
		[SerializeField]
		protected LayoutElement ImageLayoutElement;

		/// <summary>
		/// Current item.
		/// </summary>
		protected ListViewImagesItem Item;

		/// <summary>
		/// Loaded images cache.
		/// </summary>
		protected static Dictionary<string,Texture2D> Cache = new Dictionary<string, Texture2D>();

		/// <summary>
		/// Is image loading?
		/// </summary>
		protected bool IsLoading;

		/// <summary>
		/// Load coroutine.
		/// </summary>
		protected IEnumerator LoadCoroutine;

		/// <summary>
		/// Handle OnEnable event.
		/// </summary>
		protected override void OnEnable()
		{
			base.OnEnable();
			if (IsLoading)
			{
				return;
			}
			if ((Image.texture==null) && (Item!=null) && (string.IsNullOrEmpty(Item.Url)))
			{
				LoadCoroutine = Load();
				StartCoroutine(LoadCoroutine);
			}
		}

		/// <summary>
		/// Handle OnDisable event.
		/// </summary>
		protected override void OnDisable()
		{
			base.OnDisable();
			if (IsLoading)
			{
				IsLoading = false;
				StopCoroutine(LoadCoroutine);
			}
		}

		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="item">Item.</param>
		public void SetData(ListViewImagesItem item)
		{
			// save item so later can fix item.Height to actual value
			Item = item;

			Url.text = (string.IsNullOrEmpty(Item.Url)) ? Item.Url : "No image";

			if (Cache.ContainsKey(Item.Url))
			{
				SetImage();
			}
			else
			{
				// reset images parameter
				Image.texture = null;
				ImageLayoutElement.preferredHeight = -1;
				ImageLayoutElement.preferredWidth = -1;

				if ((string.IsNullOrEmpty(Item.Url)) && (Item.Url!=null))
				{
					Image.color = Color.white;
					ImageLayoutElement.minHeight = 100;
					ImageLayoutElement.minWidth = 100;

					LoadCoroutine = Load();
					StartCoroutine(LoadCoroutine);
				}
				else
				{
					Image.color = Color.clear;
					ImageLayoutElement.minHeight = -1;
					ImageLayoutElement.minWidth = -1;
				}
			}
		}

		void SetImage()
		{
			Image.color = Color.white;
			Image.texture = Cache[Item.Url];
			ImageLayoutElement.preferredHeight = Cache[Item.Url].height;
			ImageLayoutElement.preferredWidth = Cache[Item.Url].width;
		}

		IEnumerator Load()
		{
			if (!string.IsNullOrEmpty(Item.Url))
			{
				IsLoading = true;

				var url = Item.Url;

				yield return null;

				var www = new WWW(url);

				yield return www;
				if (!Cache.ContainsKey(url))
				{
					Cache.Add(url, www.texture);
				}
				if (Cache.ContainsKey(Item.Url))
				{
					SetImage();
				}

				www.Dispose();

				IsLoading = false;
			}
		}
	}
}