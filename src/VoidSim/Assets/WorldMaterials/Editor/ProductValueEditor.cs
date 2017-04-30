using Assets.WorldMaterials.Products;
using UnityEditor;

namespace Assets.WorldMaterials.Editor
{
    [CustomEditor(typeof(ProductValueScriptable))]
    public class ProductValueEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorList.Show(serializedObject.FindProperty("Products"), EditorListOption.Buttons | EditorListOption.ListLabel);
            serializedObject.ApplyModifiedProperties();
        }
    }
}