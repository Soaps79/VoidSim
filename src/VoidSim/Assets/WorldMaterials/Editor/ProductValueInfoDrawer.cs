using System.Linq;
using Assets.Scripts.WorldMaterials;
using Assets.WorldMaterials.Products;
using UnityEditor;
using UnityEngine;

namespace Assets.WorldMaterials.Editor
{
    [CustomPropertyDrawer(typeof(ProductValueInfo))]
    public class ProductValueInfoDrawer : PropertyDrawer
    {
        int _nameIndex;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            //// this looks good local, scaling not yet implemented
            EditorGUI.indentLevel = 1;
            position = EditorGUI.IndentedRect(position);
            position.x -= 12;
            var nameRect = new Rect(position.x, position.y, 140, position.height);

            // grab the current name and list
            var names = ProductValueScriptable.ProductLookup.GenerateProductNames();
            var nameProperty = property.FindPropertyRelative("ProductName");
            if (string.IsNullOrEmpty(nameProperty.stringValue))
                nameProperty.stringValue = names.First();


            // make this better and extract it
            // find the object's current name index, get the index from state of the popup
            var currentName = names.ToList().FindIndex(i => i == nameProperty.stringValue);
            var current = EditorGUI.Popup(nameRect, currentName, names, GUIStyle.none);

            // if they're different, the popup changed, grab its result
            if (current != _nameIndex)
            {
                nameProperty.stringValue = names[current];
                _nameIndex = current;
            }

            EditorGUI.indentLevel = 4;
            position = EditorGUI.IndentedRect(position);

            var categoryRect = new Rect(position.x, position.y, 160, position.height);
            EditorGUI.PropertyField(categoryRect, property.FindPropertyRelative("Value"), GUIContent.none);

            EditorGUI.EndProperty();
        }
    }
}