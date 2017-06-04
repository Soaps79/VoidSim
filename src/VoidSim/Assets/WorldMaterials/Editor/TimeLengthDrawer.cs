using Assets.Scripts;
using Assets.Scripts.WorldMaterials;
using UnityEditor;
using UnityEngine;

namespace Assets.WorldMaterials.Editor
{
    [CustomPropertyDrawer(typeof(TimeLength))]
    public class TimeLengthDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            //// this looks good local, scaling not yet implemented
            EditorGUI.indentLevel = 1;
            position = EditorGUI.IndentedRect(position);
            position.x -= 12;

            EditorGUI.LabelField(position, "Time:");
            position.x += 40;

            // create a rect for and add the amount
            var amountRect = new Rect(position.x, position.y, 40, position.height);
            EditorGUI.PropertyField(amountRect, property.FindPropertyRelative("Length"), GUIContent.none); 
            position.x += 40;

            var nameRect = new Rect(position.x, position.y, 60, position.height);
            EditorGUI.PropertyField(nameRect, property.FindPropertyRelative("TimeUnit"), GUIContent.none);

            EditorGUI.EndProperty();
        }
    }
}