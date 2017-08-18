using Assets.WorldMaterials.Products;
using UnityEditor;
using UnityEngine;

namespace Assets.WorldMaterials.Editor
{
	[CustomPropertyDrawer(typeof(ResultInfo))]
	public class ResultInfoDrawer : PropertyDrawer
	{
			private readonly EditorStringListBinder _productNameBinder = new EditorStringListBinder();

			public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
			{
				EditorGUI.BeginProperty(position, label, property);
				EditorGUI.indentLevel = 2;
				position = EditorGUI.IndentedRect(position);

				// create a rect for and add the amount
				var amountRect = new Rect(position.x, position.y, 65, position.height);
				EditorGUI.PropertyField(amountRect, property.FindPropertyRelative("Quantity"), GUIContent.none);
				position.x += 65;

				// grab the current name and list
				var names = ProductLookupEditor.ProductNames;
				var nameProperty = property.FindPropertyRelative("ProductName");
				var nameRect = new Rect(position.x, position.y, 100, position.height);

				_productNameBinder.SetStringValueFromList(nameProperty, names, nameRect);

				EditorGUI.EndProperty();
			}
	}
}