using System.Linq;
using Assets.WorldMaterials.Trade;
using UnityEditor;
using UnityEngine;

namespace Assets.WorldMaterials.Editor
{
    [CustomPropertyDrawer(typeof(TradeRequestInfo))]
    public class TradeRequestInfoDrawer : PropertyDrawer
    {
        private int _nameIndex;

        //public TimeUnit Frequency;
        //public string Product;
        //public int Amount;
        //public bool isSelling;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            //// this looks good local, scaling not yet implemented
            EditorGUI.indentLevel = 0;
            position = EditorGUI.IndentedRect(position);

            EditorGUI.LabelField(position, "Sell");
            position.x += 30;

            var sellRect = new Rect(position.x, position.y, 20, position.height);
            EditorGUI.PropertyField(sellRect, property.FindPropertyRelative("isSelling"), GUIContent.none);
            position.x += 30;

            var nameRect = new Rect(position.x, position.y, 100, position.height);
            var names = TraderRequestsSO.ProductLookup.GenerateProductNames();
            var nameProperty = property.FindPropertyRelative("Product");
            if (string.IsNullOrEmpty(nameProperty.stringValue))
                nameProperty.stringValue = names.First();


            // make this better and extract it
            // find the object's current name index, get the index from state of the popup
            var currentName = names.Contains(nameProperty.stringValue)
                ? names.ToList().FindIndex(i => i == nameProperty.stringValue) : 0;
            var current = EditorGUI.Popup(nameRect, currentName, names, GUIStyle.none);

            // if they're different, the popup changed, grab its result
            if (current != _nameIndex)
            {
                nameProperty.stringValue = names[current];
                _nameIndex = current;
            }
            position.x += 100;

            var amountRect = new Rect(position.x, position.y, 50, position.height);
            EditorGUI.PropertyField(amountRect, property.FindPropertyRelative("Amount"), GUIContent.none);
            position.x += 60;

            var timeRect = new Rect(position.x, position.y, 50, position.height);
            EditorGUI.PropertyField(timeRect, property.FindPropertyRelative("Frequency"), GUIContent.none);

            
            

            EditorGUI.EndProperty();
        }
    }
}