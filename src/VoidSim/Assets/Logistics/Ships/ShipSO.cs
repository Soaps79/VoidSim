using UnityEditor;
using UnityEngine;

namespace Assets.Logistics.Ships
{
	public class ShipSO : ScriptableObject
	{
		public float MinScale;
		public float MaxScale;
		public float MinTravelTime;
		public float MaxTravelTime;
		public Sprite Sprite;

		public float RandomizedTravelTime
		{
			get { return Random.Range(MinTravelTime, MaxTravelTime); }
		}

		[MenuItem("Assets/WorldMaterials/Ship")]
		public static void CreateMyAsset()
		{
			var asset = CreateInstance<ShipSO>();

			AssetDatabase.CreateAsset(asset, "Assets/Resources/Ships/NewShip.asset");
			AssetDatabase.SaveAssets();

			EditorUtility.FocusProjectWindow();

			Selection.activeObject = asset;
		}
	}
}