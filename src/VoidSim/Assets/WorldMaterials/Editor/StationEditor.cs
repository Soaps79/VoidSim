using UnityEditor;
using UnityEngine;
using Assets.Station;

namespace Assets.WorldMaterials.Editor
{
	[CustomEditor(typeof(Station.Station))]
	public class StationEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			var station = (Station.Station)target;
			if (GUILayout.Button("Toggle Pop Mood"))
			{
				station.TogglePopMood();
			}
		}
	}
}