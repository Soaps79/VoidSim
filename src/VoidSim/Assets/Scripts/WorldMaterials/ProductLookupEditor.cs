using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEditor;

namespace Assets.Scripts.WorldMaterials
{
    /// <summary>
    /// Make editing products and recipes not painful.
    /// </summary>
    [CustomEditor(typeof(ProductEditorViewModel))]
    public class ProductLookupEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var view = (ProductEditorViewModel)this.serializedObject.targetObject;
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

    //[CustomEditor(typeof (ProductLookup))]
    //public class ResourceCustomEditor : Editor
    //{

    //}
}