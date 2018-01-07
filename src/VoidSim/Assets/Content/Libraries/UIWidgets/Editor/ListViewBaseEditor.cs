using UnityEditor;
using System.Collections.Generic;
using System;

namespace UIWidgets
{
	[CanEditMultipleObjects]
	//[CustomEditor(typeof(ListViewBase), false)]
	public class ListViewBaseEditor : Editor
	{
		Dictionary<string,SerializedProperty> serializedProperties = new Dictionary<string,SerializedProperty>();
		
		string[] properties = new string[]{
			"items",
			"DestroyGameObjects",
			"multipleSelect",
			"selectedIndex",

			"Container",

			"Navigation",
		};
		
		protected virtual void OnEnable()
		{
			Array.ForEach(properties, x => {
				serializedProperties.Add(x, serializedObject.FindProperty(x));
			});
		}
		
		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			serializedProperties.ForEach(x => EditorGUILayout.PropertyField(x.Value, true));

			serializedObject.ApplyModifiedProperties();

			//Array.ForEach(targets, x => ((ListViewBase)x).UpdateItems());
		}
	}
}