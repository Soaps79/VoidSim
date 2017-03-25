using System.IO;
using Assets.Scripts.WorldMaterials;
using UnityEditor;
using UnityEngine;

namespace Assets.WorldMaterials.Editor
{
    [CustomEditor(typeof(InventoryScriptable))]
    public class InventoryEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var view = (InventoryScriptable)this.serializedObject.targetObject;
            DrawDefaultInspector();
        }
    }
}
