using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.Logistics.Ships
{
	[Serializable]
	public class TrafficScheduleEntry
	{
		public float InitialDelay;
		public float RegularDelay;
		public string SOName;
	}

	public class ShipSchedule : ScriptableObject
	{
		public List<TrafficScheduleEntry> Entries;

		[MenuItem("Assets/WorldMaterials/TrafficSchedule")]
		public static void CreateMyAsset()
		{
			var asset = ScriptableObject.CreateInstance<ShipSchedule>();

			AssetDatabase.CreateAsset(asset, "Assets/Resources/Scriptables/NewTradeRequestSO.asset");
			AssetDatabase.SaveAssets();

			EditorUtility.FocusProjectWindow();
			Selection.activeObject = asset;
		}
	}
}