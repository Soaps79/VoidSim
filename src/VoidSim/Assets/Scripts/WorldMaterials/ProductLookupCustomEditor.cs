using UnityEngine;
using UnityEditor;

namespace Assets.Scripts.WorldMaterials
{
	[CustomEditor(typeof(ProductLookup))]
	public class ProductLookupCustomEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();
			var script = (ProductLookup) target;
			if (GUILayout.Button("Serialize"))
			{
				script.SerializeData();
			}

		}
	}
}