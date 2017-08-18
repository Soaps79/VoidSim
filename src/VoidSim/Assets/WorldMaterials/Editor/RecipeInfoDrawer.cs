using Assets.WorldMaterials.Products;
using UnityEditor;
using UnityEngine;

namespace Assets.WorldMaterials.Editor
{
	[CustomPropertyDrawer(typeof(RecipeInfo))]
	public class RecipeInfoDrawer : PropertyDrawer
	{
		private EditorStringListBinder _productNameBinder = new EditorStringListBinder();
		private EditorStringListBinder _containerNameBinder = new EditorStringListBinder();

		//public class RecipeInfo
		//{
		//    public string ResultProduct;
		//    public int ResultAmount;
		//    public List<IngredientInfo> Ingredients;
		//    public TimeLength TimeLength;
		//    public string ContainerName;
		//}
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);
			EditorGUI.indentLevel = 1;
			var startX = position.x;
			
			var nameLabelRect = new Rect(position.x, position.y, 180, position.height);
			EditorGUI.LabelField(nameLabelRect, "Display Name");
			position.x += 100;

			var nameRect = new Rect(position.x, position.y, 200, position.height);
			EditorGUI.PropertyField(nameRect, property.FindPropertyRelative("DisplayName"), GUIContent.none);
			position.x = startX + 2;
			position.y += position.height + 2;
			GUILayout.Space(20);

			//var productRect = new Rect(position.x, position.y, 110, position.height);
			//_productNameBinder.SetStringValueFromList(property.FindPropertyRelative("ResultProduct"), names, productRect);
			//position.x += 110;

			//var labelRect = new Rect(position.x, position.y, 40, position.height);
			//EditorGUI.LabelField(labelRect, "Qty");
			//position.x += 35;

			//var amountRect = new Rect(position.x, position.y, 40, position.height);
			//EditorGUI.PropertyField(amountRect, property.FindPropertyRelative("ResultAmount"), GUIContent.none);
			//position.x += 40;

			var containerLabelRect = new Rect(position.x, position.y, 100, position.height);
			EditorGUI.LabelField(containerLabelRect, "Container:");
			position.x += 70;

			var names = ProductLookupEditor.ContainerNames;
			var containerRect = new Rect(position.x, position.y, 100, position.height);
			_containerNameBinder.SetStringValueFromList(property.FindPropertyRelative("ContainerName"), names, containerRect);
			position.x += 100;

			//EditorGUI.indentLevel = 1;
			//position.y += position.height + 2;
			//position.x = startX - 4;

			var timeRect = new Rect(position.x, position.y, 200, position.height);
			EditorGUI.PropertyField(timeRect, property.FindPropertyRelative("TimeLength"), GUIContent.none);
			position.x += 140;


			EditorGUI.indentLevel = 2;
			EditorList.Show(property.FindPropertyRelative("Ingredients"), EditorListOption.ListLabel | EditorListOption.Buttons);

			EditorGUI.indentLevel = 2;
			EditorList.Show(property.FindPropertyRelative("Results"), EditorListOption.ListLabel | EditorListOption.Buttons);

			GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
			//GUILayout.Space(10);
			EditorGUI.EndProperty();
		}
	}
}