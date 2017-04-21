using System.Linq;
using Assets.Scripts.WorldMaterials;
using Assets.WorldMaterials.Products;
using UnityEditor;

namespace Assets.WorldMaterials.Editor
{
    /// <summary>
    /// Make editing products and recipes not painful.
    /// </summary>
    [CustomEditor(typeof(ProductLookupScriptable))]
    public class ProductLookupEditor : UnityEditor.Editor
    {
        // These are for the editor dropdowns
        // Ingredient PropertyDrawer needs this in both an array and a string
        // it ToLists every draw, maintain both if perf becomes an issue
        public static string[] ProductNames;
        public static string[] ContainerNames;

        private ProductLookupScriptable _lookup;

        
        public override void OnInspectorGUI()
        {
            // do we have to do this every time?
            _lookup = (ProductLookupScriptable)this.serializedObject.targetObject;

            ProductNames = _lookup.GenerateProductNames();
            ContainerNames = _lookup.GenerateContainerNames();
            DrawDefaultInspector();

            // this will make it check every cycle, is this necessary?
            // must be a way to redraw on a real Dirty
            EditorUtility.SetDirty(this);
        }
    }
}