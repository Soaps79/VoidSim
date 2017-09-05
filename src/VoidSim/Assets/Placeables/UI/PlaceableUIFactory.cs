using System;
using System.Collections.Generic;
using Assets.Controllers.GUI;
using Assets.Scripts.UI;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace Assets.Placeables.UI
{
	public static class PlaceableUIFactory
	{
		private static readonly Dictionary<string, PlaceableViewModel> _activeViewModels 
			= new Dictionary<string, PlaceableViewModel>();

		private static PlaceablesLookup _scriptable;
		private static Transform _centerPoint;

		/// <summary>
		/// Must be called once in order for object to function
		/// </summary>
		public static void Initialize(Transform centerPoint)
		{
			_centerPoint = centerPoint;
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
			var position = GetUiObjectPosition(placeable.transform);
			viewModelInstance.transform.position = position;
			var close = viewModelInstance.GetComponent<ClosePanelButton>();
			if (close != null)
				close.OnClose += () => { DisableUI(placeable.name); };
		}

		private static Vector2 GetUiObjectPosition(Transform placeableTransform)
		{
			const float distance = 300f;
			var placeablePoint = Camera.main.WorldToScreenPoint(placeableTransform.position);
			var stationPoint = Camera.main.WorldToScreenPoint(_centerPoint.position);

			var direction = placeablePoint - stationPoint;
			return stationPoint + (direction.normalized * distance);
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