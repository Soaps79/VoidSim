using System.Linq;
using Assets.Scripts.WorldMaterials;
using UnityEditor;
using UnityEngine;

namespace Assets.WorldMaterials.Editor
{
    [CustomPropertyDrawer(typeof(ProductInfo))]
    public class ProductInfoDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            //// this looks good local, scaling not yet implemented
            EditorGUI.indentLevel = 1;
            position = EditorGUI.IndentedRect(position);
            position.x -= 12;

            // create a rect for and add the amount
            var amountRect = new Rect(position.x, position.y, 140, position.height);
            EditorGUI.PropertyField(amountRect, property.FindPropertyRelative("Name"), GUIContent.none);

            position.x += 140;
            var nameRect = new Rect(position.x, position.y, 80, position.height);
            EditorGUI.PropertyField(nameRect, property.FindPropertyRelative("Category"), GUIContent.none);

            // Saving for 
            //// grab the current name and list
            //var names = ProductLookupEditor.ProductNames;


            //// make this better and extract it
            //// find the object's current name index, get the index from state of the popup
            //var currentName = names.ToList().FindIndex(i => i == nameProperty.stringValue);
            //var current = EditorGUI.Popup(nameRect, currentName, names, GUIStyle.none);

            // if they're different, the popup changed, grab its result
            //if (current != _nameIndex)
            //{
            //    nameProperty.stringValue = names[current];
            //    _nameIndex = current;
            //}

            EditorGUI.EndProperty();
        }
    }
}