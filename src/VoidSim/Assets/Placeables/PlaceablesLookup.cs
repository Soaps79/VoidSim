using System.Collections.Generic;
using UnityEngine;

namespace Assets.Placeables
{
	public class PlaceablesLookup : ScriptableObject
	{
		[SerializeField] private PlaceableViewModel _viewModel;
        public List<PlaceableScriptable> Placeables;

		void OnEnable()
		{
			foreach (var placeable in Placeables)
			{
				placeable.ViewModel = _viewModel;
			}
		}
    }
}
