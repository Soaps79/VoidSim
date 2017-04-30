using System.Linq;
using Assets.Scripts.WorldMaterials;
using UnityEditor;
using UnityEngine;

namespace Assets.WorldMaterials.Editor
{
    [CustomPropertyDrawer(typeof(ProductEntryInfo))]
    public class InventoryEntryDrawer : PropertyDrawer
    {
        private readonly EditorStringListBinder _productNameBinder = new EditorStringListBinder();

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            // create a rect for and add the amount
            var amountRect = new Rect(position.x, position.y, 80, position.height);
            EditorGUI.PropertyField(amountRect, property.FindPropertyRelative("Amount"), GUIContent.none);
            position.x += 80;

            var nameRect = new Rect(position.x, position.y, 100, position.height);
            var names = InventoryScriptable.ProductLookup.GenerateProductNames();
            var nameProperty = property.FindPropertyRelative("ProductName");

            _productNameBinder.SetStringValueFromList(nameProperty, names, nameRect);

            EditorGUI.indentLevel = 1;

            EditorGUI.EndProperty();
        }
    }
}
