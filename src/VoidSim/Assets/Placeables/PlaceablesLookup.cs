using System.Collections.Generic;
using UnityEngine;

namespace Assets.Placeables
{
	public class PlaceablesLookup : ScriptableObject
	{
		public PlaceableViewModel ViewModel;
        public List<PlaceableScriptable> Placeables;
    }
}
