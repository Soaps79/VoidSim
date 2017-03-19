using System.IO;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.WorldMaterials
{
    [CustomEditor(typeof(InventoryEditorViewModel))]
    public class InventoryEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var view = (InventoryEditorViewModel)this.serializedObject.targetObject;
            DrawDefaultInspector();

            // add a button to serialize
            if (GUILayout.Button("Serialize"))
            {
                view.SerializeAll();
            }

        }

        private void SerializeCollection(string name, string json)
        {
            using (FileStream fs = File.Open(string.Format("Resources/{0}.json", name), FileMode.Create, FileAccess.Write))
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.WriteLine(json);
            }
        }
    }
}
