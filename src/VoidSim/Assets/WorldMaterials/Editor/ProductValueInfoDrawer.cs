﻿using Assets.WorldMaterials.Products;
using UnityEditor;
using UnityEngine;

namespace Assets.WorldMaterials.Editor
{
    [CustomPropertyDrawer(typeof(ProductValueInfo))]
    public class ProductValueInfoDrawer : PropertyDrawer
    {
        private EditorStringListBinder _productNameBinder = new EditorStringListBinder();

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

            _productNameBinder.SetStringValueFromList(nameProperty, names, nameRect);

            EditorGUI.indentLevel = 4;
            position = EditorGUI.IndentedRect(position);

            var categoryRect = new Rect(position.x, position.y, 160, position.height);
            EditorGUI.PropertyField(categoryRect, property.FindPropertyRelative("Value"), GUIContent.none);

            EditorGUI.EndProperty();
        }
    }
}