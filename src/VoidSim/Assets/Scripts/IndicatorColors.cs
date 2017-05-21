using UnityEditor;
using UnityEngine;

namespace Assets.Scripts
{
	public class IndicatorColors : ScriptableObject
	{
		public Color Go;
		public Color Stop;
		public Color Caution;

		[MenuItem("Assets/IndicatorColors")]
		public static void CreateMyAsset()
		{
			var asset = ScriptableObject.CreateInstance<IndicatorColors>();

			AssetDatabase.CreateAsset(asset, "Assets/Resources/NewIndicatorColors.asset");
			AssetDatabase.SaveAssets();

			EditorUtility.FocusProjectWindow();
			Selection.activeObject = asset;
		}
	}
}
