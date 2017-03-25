using System.Linq;
using Assets.Scripts.WorldMaterials;
using UnityEditor;

namespace Assets.WorldMaterials.Editor
{
    /// <summary>
    /// Make editing products and recipes not painful.
    /// </summary>
    [CustomEditor(typeof(ProductLookupScriptable))]
    public class ProductLookupEditor : UnityEditor.Editor
    {
        public static string[] ProductNames;
        public static string[] ContainerNames;

        private void BindProductNames()
        {
            ProductNames = _lookup.Products != null && _lookup.Products.Any()
                ? _lookup.Products.Select(i => i.Name).ToArray()
                : new string[1] { "Empty" };
        }

        private void BindContainerNames()
        {
            ContainerNames = _lookup.Containers != null && _lookup.Containers.Any()
                ? _lookup.Containers.Select(i => i.Name).ToArray()
                : new string[1] { "Empty" };
        }

        private ProductLookupScriptable _lookup;

        public override void OnInspectorGUI()
        {
            _lookup = (ProductLookupScriptable)this.serializedObject.targetObject;
            BindProductNames();
            BindContainerNames();



            DrawDefaultInspector();
            EditorUtility.SetDirty(this);

            // add a button to serialize
            //if (GUILayout.Button("Serialize"))
            //{
            //    view.SerializeAll();
            //}

        }
    }

    //[CustomEditor(typeof (ProductLookup))]
    //public class ResourceCustomEditor : Editor
    //{

    //}
}