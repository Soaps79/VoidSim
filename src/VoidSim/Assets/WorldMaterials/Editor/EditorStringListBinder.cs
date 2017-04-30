using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Assets.WorldMaterials.Editor
{
    /// <summary>
    /// Binds a string value to an editor popup.
    /// Pass in the SerializedProperty, the list of names to choose from, and where to draw.
    /// </summary>
    public class EditorStringListBinder
    {
        private int _nameIndex;

        public void SetStringValueFromList(SerializedProperty nameProperty, string[] names, Rect position)
        {
            if (string.IsNullOrEmpty(nameProperty.stringValue))
                nameProperty.stringValue = names.First();

            // find the object's current name index, get the index from state of the popup
            var currentName = names.Contains(nameProperty.stringValue)
                ? names.ToList().FindIndex(i => i == nameProperty.stringValue) : 0;
            var current = EditorGUI.Popup(position, currentName, names, GUIStyle.none);

            // if they're different, the popup changed, grab its result
            if (current != _nameIndex)
            {
                nameProperty.stringValue = names[current];
                _nameIndex = current;
            }
        }
    }
}