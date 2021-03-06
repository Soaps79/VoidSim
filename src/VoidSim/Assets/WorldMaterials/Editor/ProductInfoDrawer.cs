﻿using System.Linq;
using Assets.Scripts.WorldMaterials;
using Assets.WorldMaterials.Products;
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

            // get the product name, then handle moving the cursor
            var nameRect = new Rect(position.x, position.y, 140, position.height);
            EditorGUI.PropertyField(nameRect, property.FindPropertyRelative("Name"), GUIContent.none);
            position.x += 140;

            var categoryRect = new Rect(position.x, position.y, 70, position.height);
            EditorGUI.PropertyField(categoryRect, property.FindPropertyRelative("Category"), GUIContent.none);

            position.x += 60;

            var colorRect = new Rect(position.x, position.y, 100, position.height);
            EditorGUI.PropertyField(colorRect, property.FindPropertyRelative("Color"), GUIContent.none);

            position.x += 100;

            var iconRect = new Rect(position.x, position.y, 80, position.height);
            EditorGUI.PropertyField(iconRect, property.FindPropertyRelative("IconSprite"), GUIContent.none);

            EditorGUI.EndProperty();
        }
    }
}