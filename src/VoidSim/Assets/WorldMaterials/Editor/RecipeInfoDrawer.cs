using Assets.Scripts.WorldMaterials;
using UnityEditor;
using UnityEngine;

namespace Assets.WorldMaterials.Editor
{
    //[CustomPropertyDrawer(typeof (RecipeInfo))]
    //public class RecipeInfoDrawer : PropertyDrawer
    //{
    //    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    //    {
    //        // Using BeginProperty / EndProperty on the parent property means that
    //        // prefab override logic works on the entire property.
    //        EditorGUI.BeginProperty(position, label, property);

    //        EditorGUI.LabelField(position, "Result:");

    //        var ingRect = new Rect(position.x + 50, position.y, position.width, position.height);
    //        var ing = property.FindPropertyRelative("Ingredients");
    //        EditorGUI.PropertyField(ingRect, ing, true);
    //    }
    //}
}