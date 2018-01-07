using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace UIWidgets
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(Splitter), true)]
	public class SplitterEditor : Editor {

		protected Dictionary<string,SerializedProperty> SerializedProperties = new Dictionary<string,SerializedProperty>();
		protected Dictionary<string,SerializedProperty> SerializedCursors = new Dictionary<string,SerializedProperty>();
		
		protected List<string> Properties = new List<string>();
		protected List<string> Cursors = new List<string>(){
			"CurrentCamera",
			"CursorTexture",
			"CursorHotSpot",
			"DefaultCursorTexture",
			"DefaultCursorHotSpot",
		};
		
		protected virtual void OnEnable()
		{
			Properties.Clear();
			SerializedProperties.Clear();
			SerializedCursors.Clear();
			
			var property = serializedObject.GetIterator();
			property.NextVisible(true);
			while (property.NextVisible(false))
			{
				AddProperty(property);
			}
			
			Properties.ForEach(x => {
				SerializedProperties.Add(x, serializedObject.FindProperty(x));
			});
			Cursors.ForEach(x => {
				SerializedCursors.Add(x, serializedObject.FindProperty(x));
			});
		}
		
		void AddProperty(SerializedProperty property)
		{
			if (!Cursors.Contains(property.name))
			{
				Properties.Add(property.name);
			}
		}
		
		protected bool ShowCursors;
		
		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUILayout.PropertyField(SerializedProperties["Type"], true);
			EditorGUILayout.PropertyField(SerializedProperties["UpdateRectTransforms"], true);
			EditorGUILayout.PropertyField(SerializedProperties["UpdateLayoutElements"], true);
			EditorGUILayout.PropertyField(SerializedProperties["mode"], true);
			if (SerializedProperties["mode"].enumValueIndex==1)// compact layout
			{
				EditorGUILayout.PropertyField(SerializedProperties["leftTarget"], true);
				EditorGUILayout.PropertyField(SerializedProperties["rightTarget"], true);
			}
			
			EditorGUILayout.BeginVertical();
			ShowCursors = GUILayout.Toggle(ShowCursors, "Cursors", "Foldout", GUILayout.ExpandWidth(true));
			if (ShowCursors)
			{
				SerializedCursors.ForEach(x => EditorGUILayout.PropertyField(x.Value, true));
			}
			EditorGUILayout.EndVertical();
			
			serializedObject.ApplyModifiedProperties();
		}

	}
}