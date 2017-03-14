using UnityEngine;
using UnityEditor;

namespace Assets.Scripts.WorldMaterials
{
	/// <summary>
	/// Make editing products and recipes not painful.
	/// </summary>
	[CustomEditor(typeof(ProductLookup))]
	public class ProductLookupCustomEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			// add a button to serialize
			var script = (ProductLookup) target;
			if (GUILayout.Button("Serialize"))
			{
				script.SerializeData();
			}

		}
	}
}