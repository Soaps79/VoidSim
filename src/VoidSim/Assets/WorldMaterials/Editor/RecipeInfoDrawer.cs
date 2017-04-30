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
            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.indentLevel = 1;
            var startX = position.x;

            var names = ProductLookupEditor.ProductNames;
            var productRect = new Rect(position.x, position.y, 140, position.height);
            _productNameBinder.SetStringValueFromList(property.FindPropertyRelative("ResultProduct"), names, productRect);
            position.x += 140;

            var amountRect = new Rect(position.x, position.y, 40, position.height);
            EditorGUI.PropertyField(amountRect, property.FindPropertyRelative("ResultAmount"), GUIContent.none);
            position.x += 40;

            names = ProductLookupEditor.ContainerNames;
            var containerRect = new Rect(position.x, position.y, 140, position.height);
            _containerNameBinder.SetStringValueFromList(property.FindPropertyRelative("ContainerName"), names, containerRect);
            position.x += 140;

            EditorGUI.indentLevel = 1;
            position.y += position.height + 2;
            position.x = startX;

            var timeRect = new Rect(position.x, position.y, 200, position.height);
            EditorGUI.PropertyField(timeRect, property.FindPropertyRelative("TimeLength"), GUIContent.none);
            position.x += 140;


            EditorGUI.indentLevel = 11;
            //EditorGUI.LabelField(position, "Result:");
            EditorList.Show(property.FindPropertyRelative("Ingredients"), EditorListOption.ListLabel | EditorListOption.Buttons);

            //var ingRect = new Rect(position.x + 50, position.y, position.width, position.height);
            //var ing = property.FindPropertyRelative("Ingredients");
            //EditorGUI.PropertyField(ingRect, ing, true);
        }
    }
}