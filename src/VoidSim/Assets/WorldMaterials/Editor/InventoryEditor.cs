using System.IO;
using Assets.Scripts.WorldMaterials;
using UnityEditor;
using UnityEngine;

namespace Assets.WorldMaterials.Editor
{
    [CustomEditor(typeof(InventoryScriptable))]
    public class InventoryEditor : UnityEditor.Editor
    {
        //public int ProductMaxAmount;
        //public List<string> Placeables;
        //public List<ProductCategory> ProductsToIgnore;
        //public List<ProductEntryInfo> Products;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorList.Show(serializedObject.FindProperty("Products"), EditorListOption.Buttons | EditorListOption.ListLabel);
            EditorList.Show(serializedObject.FindProperty("ProductsToIgnore"), EditorListOption.Buttons | EditorListOption.ListLabel);
            EditorList.Show(serializedObject.FindProperty("Placeables"), EditorListOption.Buttons | EditorListOption.ListLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("ProductMaxAmount"), new GUIContent("Default Maximum"));

            serializedObject.ApplyModifiedProperties();
        }
    }
}
