﻿using UnityEngine;
using UIWidgets;

namespace UIWidgetsSamples
{
	/// <summary>
	/// Sample script how to add localization for ListView.
	/// </summary>
	public class LocalizationSampleListView : MonoBehaviour
	{
		[SerializeField]
		ListView targetListView;

		/// <summary>
		/// Target ListView.
		/// </summary>
		public ListView TargetListView
		{
			get
			{
				if (targetListView == null)
				{
					targetListView = GetComponent<ListView>();
				}
				return targetListView;
			}
		}

		/// <summary>
		/// Original DataSource.
		/// </summary>
		protected ObservableList<string> OriginalDataSource;

		/// <summary>
		/// Start this instance and add listeners.
		/// </summary>
		protected virtual void Start()
		{
			TargetListView.Init();

			// save original data
			OriginalDataSource = TargetListView.DataSource;

			Localize();

			// Add callback on language change, if localization system support this.
			//LocalizationSystem.OnLanguageChange += Localize;
			//LocalizationSystem.OnLanguageChange.AddListener(Localize);
		}

		/// <summary>
		/// Localize.
		/// </summary>
		public void Localize()
		{
			TargetListView.DataSource = OriginalDataSource.Convert(x => GetLocalizedString(x));
		}

		static string GetLocalizedString(string str)
		{
			// return localized string
			return str;
		}

		/// <summary>
		/// Remove listeners.
		/// </summary>
		protected virtual void OnDestroy()
		{
			// Remove callback on language change, if localization system support this.
			//LocalizationSystem.OnLanguageChange -= Localize;
			//LocalizationSystem.OnLanguageChange.RemoveListener(Localize);
		}
	}
}