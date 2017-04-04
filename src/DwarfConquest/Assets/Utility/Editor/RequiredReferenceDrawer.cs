using Assets.Utility.Attributes;
using UnityEditor;
using UnityEngine;

namespace Assets.Utility.Editor
{
    [CustomPropertyDrawer(typeof(RequireReferenceAttribute))]
    public class RequireReferenceDrawer : PropertyDrawer
    {
        private bool _didShowError = false;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.ObjectReference)
            {
                if (!_didShowError)
                {
                    var errorStr = string.Format("The [RequireReference] attribute must be used with an object reference. See the '{0}' field on object '{1}", property.displayName, property.serializedObject.targetObject.name);
                    Debug.LogError(errorStr);
                    _didShowError = true;
                }
            }
            else if (property.objectReferenceValue == null)
            {
                var originalColor = GUI.color;

                var labelPos = position;
                labelPos.xMax = labelPos.xMin + EditorGUIUtility.labelWidth;

                var requireAttribute = (RequireReferenceAttribute)base.attribute;

                GUI.color = requireAttribute.Highlight;
                GUI.Box(labelPos, string.Empty);
                GUI.color = originalColor;
            }

            EditorGUI.PropertyField(position, property, label);
        }
    }
}