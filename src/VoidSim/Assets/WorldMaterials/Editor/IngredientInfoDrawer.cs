using System.Linq;
using Assets.Scripts.WorldMaterials;
using Assets.WorldMaterials.Products;
using UnityEditor;
using UnityEngine;

namespace Assets.WorldMaterials.Editor
{
    [CustomPropertyDrawer(typeof(IngredientInfo))]
    public class IngredientInfoDrawer : PropertyDrawer
    {
        int _nameIndex;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            // Draw label
            //position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // this looks good local, scaling not yet implemented
            EditorGUI.indentLevel = 2;
            position = EditorGUI.IndentedRect(position);
            position.x -= 12;

            // create a rect for and add the amount
            var amountRect = new Rect(position.x, position.y, 65, position.height);
            EditorGUI.PropertyField(amountRect, property.FindPropertyRelative("Quantity"), GUIContent.none);
            
            // prepare rect for product selector
            // learn how to snap right
            EditorGUI.indentLevel = 4;
            position.x -= 18;
            position = EditorGUI.IndentedRect(position);
            var nameRect = new Rect(position.x, position.y, 100, position.height);

            // grab the current name and list
            var names = ProductLookupEditor.ProductNames;
            var nameProperty = property.FindPropertyRelative("ProductId");


            // make this better and extract it
            // find the object's current name index, get the index from state of the popup
            var currentName = names.ToList().FindIndex(i => i == nameProperty.stringValue);
            var current  = EditorGUI.Popup(nameRect, currentName, names, GUIStyle.none);

            // if they're different, the popup changed, grab its result
            if (current != _nameIndex)
            {
                nameProperty.stringValue = names[current];
                _nameIndex = current;
            }

            EditorGUI.EndProperty();
        }
    }
}