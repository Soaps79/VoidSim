using System.Collections.Generic;
using Assets.Controllers.GUI;
using UnityEngine;
using Zenject;

namespace Assets.Placeables.UI
{
	public static class PlaceableUIFactory
	{
		private static readonly Dictionary<string, PlaceableViewModel> _activeViewModels 
			= new Dictionary<string, PlaceableViewModel>();

		private static PlaceablesLookup _scriptable;

		/// <summary>
		/// Must be called once in order for object to function
		/// </summary>
		public static void Initialize()
		{
			_scriptable = Object.Instantiate(Resources.Load("placeables_lookup")) as PlaceablesLookup;
		}

		// Placeables themselves will call this when they are clicked
		public static void ToggleUI(Placeable placeable)
		{
			if (!_activeViewModels.ContainsKey(placeable.name))
				EnableUI(placeable);
			else
				DisableUI(placeable.name);
		}

		// create the UI item
		private static void EnableUI(Placeable placeable)
		{
			// create view model
			var canvas = GameObject.Find("InfoCanvas");
			var viewModelInstance = GameObject.Instantiate(_scriptable.ViewModel, canvas.transform, false);
			viewModelInstance.Bind(placeable);
			var close = viewModelInstance.GetComponent<ClosePanelButton>();
			if (close != null)
				close.OnClose += () => { DisableUI(placeable.name); };
		}

		// destroy the UI item
		private static void DisableUI(string placeableName)
		{
			if (!_activeViewModels.ContainsKey(placeableName))
				return;

			var viewModel = _activeViewModels[placeableName];
			GameObject.Destroy(viewModel.gameObject);
			_activeViewModels.Remove(placeableName);
		}
	}
}